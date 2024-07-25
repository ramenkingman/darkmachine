using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance { get; private set; }

    private bool _shouldCreateAccount = true;
    private string _customID;
    private string _titlePlayerID;
    private int _scoreToSave;
    private int _playerLevelToSave;
    private int _xFollowToSave;
    private int _invitationToSave;
    private Dictionary<string, int> _botLevelsToSave = new Dictionary<string, int>();
    private bool _isSavingData = false;
    private bool _isDataLoaded = false;
    private string _playFabId;
    private int _currentEnergyToSave;
    private bool _dataChanged = false;

    public int XFollowToSave => _xFollowToSave;
    public int InvitationToSave => _invitationToSave;
    public int CurrentEnergyToSave => _currentEnergyToSave;

    // カスタムID保存キー
    private static readonly string CUSTOM_ID_SAVE_KEY = "CUSTOM_ID_SAVE_KEY_GAME2";

    private string lastSessionEndTimeKey = "LastSessionEndTime";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _isDataLoaded = false; // 初期化
        Login();
        StartCoroutine(WaitForLoginThenCalculateOfflineTime());
        StartCoroutine(AutoSaveData());
    }

    private IEnumerator WaitForLoginThenCalculateOfflineTime()
    {
        while (!_isDataLoaded)
        {
            yield return null;
        }
        CalculateOfflineTime();
    }

    private void OnApplicationQuit()
    {
        SaveLastSessionEndTime();
        SavePlayerDataImmediate(ScoreManager.Instance.Score, LevelManager.Instance.PlayerLevel, XFollowToSave, InvitationToSave, BotManager.Instance.GetBotLevels(), EnergyManager.Instance.CurrentEnergy);
        Debug.Log("Data saved on application quit.");
    }

    private void SaveLastSessionEndTime()
    {
        DateTime currentTime = DateTime.UtcNow;
        PlayerPrefs.SetString(lastSessionEndTimeKey, currentTime.ToString());
        PlayerPrefs.Save();
        Debug.Log($"Saved last session end time: {currentTime}");
    }

    private void CalculateOfflineTime()
    {
        if (PlayerPrefs.HasKey(lastSessionEndTimeKey))
        {
            DateTime lastSessionEndTime = DateTime.Parse(PlayerPrefs.GetString(lastSessionEndTimeKey));
            TimeSpan offlineDuration = DateTime.UtcNow - lastSessionEndTime;

            Debug.Log($"Offline Duration: {offlineDuration.TotalSeconds} seconds");

            int energyToAdd = CalculateEnergyForOfflineDuration(offlineDuration);
            Debug.Log($"Energy to add: {energyToAdd} (Recovery Rate: {LevelManager.Instance.ScoreIncreaseAmount} per second)");
            EnergyManager.Instance.IncreaseEnergy(energyToAdd);
            Debug.Log($"New Energy Level: {EnergyManager.Instance.CurrentEnergy}");

            CalculateBotEarnings(offlineDuration);
        }
        else
        {
            Debug.Log("No previous session end time found.");
        }
    }

    private int CalculateEnergyForOfflineDuration(TimeSpan offlineDuration)
    {
        // プレイヤーレベルに基づいたスタミナ回復量を計算
        int energyRecoveryRate = LevelManager.Instance.ScoreIncreaseAmount; // スコア増加量と同じ
        int energyToAdd = (int)(offlineDuration.TotalSeconds * energyRecoveryRate);
        Debug.Log($"Calculated energy to add: {energyToAdd} for offline duration: {offlineDuration.TotalSeconds} seconds");
        return energyToAdd;
    }

    private void CalculateBotEarnings(TimeSpan offlineDuration)
    {
        int totalCoinsToAdd = 0;
        var bots = BotManager.Instance.GetBots();
        foreach (var bot in bots)
        {
            // 元の1時間単位に戻す
            int botCoins = bot.GetCurrentScorePerHour() * (int)offlineDuration.TotalHours;
            totalCoinsToAdd += botCoins;
            Debug.Log($"Bot: {bot.Name}, Level: {bot.Level}, Coins Added: {botCoins}");
        }

        ScoreManager.Instance.AddScore(totalCoinsToAdd);
        Debug.Log($"Total coins added from bots: {totalCoinsToAdd}");
    }

    private void Login()
    {
        _customID = LoadCustomID();
        Debug.Log("CustomID: " + _customID);
        var request = new LoginWithCustomIDRequest { CustomId = _customID, CreateAccount = _shouldCreateAccount };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        if (_shouldCreateAccount && !result.NewlyCreated)
        {
            Debug.LogWarning("CustomId :" + _customID + "は既に使われています。");
            // カスタムIDの再生成や他の対応をここで行う
            return;
        }

        if (result.NewlyCreated)
        {
            SaveCustomID();
            Debug.Log("新規作成成功");
        }

        Debug.Log("ログイン成功!!");
        _playFabId = result.PlayFabId;

        // タイトルプレイヤーIDを取得
        _titlePlayerID = result.EntityToken.Entity.Id;

        LoadPlayerData();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("PlayFabのログインに失敗\n" + error.GenerateErrorReport());
    }

    private string LoadCustomID()
    {
        string id = PlayerPrefs.GetString(CUSTOM_ID_SAVE_KEY);
        _shouldCreateAccount = string.IsNullOrEmpty(id);

        if (_shouldCreateAccount)
        {
            id = GenerateCustomID();
            _customID = id;
            SaveCustomID();
        }
        else
        {
            _customID = id; // 修正：既存のカスタムIDをセットする
        }

        return id;
    }

    private void SaveCustomID()
    {
        PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, _customID);
        PlayerPrefs.Save();
    }

    private string GenerateCustomID()
    {
        Guid guid = Guid.NewGuid();
        return guid.ToString("N");
    }

    public void MarkDataChanged()
    {
        _dataChanged = true;
    }

    public void SavePlayerData(int score, int playerLevel, int xFollow, int invitation, Dictionary<string, int> botLevels, int currentEnergy)
    {
        _scoreToSave = score;
        _playerLevelToSave = playerLevel;
        _xFollowToSave = xFollow;
        _invitationToSave = invitation;
        _botLevelsToSave = botLevels;
        _currentEnergyToSave = currentEnergy;

        _dataChanged = true;
    }

    public void SavePlayerDataImmediate(int score, int playerLevel, int xFollow, int invitation, Dictionary<string, int> botLevels, int currentEnergy)
    {
        if (!_isDataLoaded)
        {
            Debug.LogWarning("データがロードされるまで保存を待機しています");
            return;
        }

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Score", score.ToString() },
                { "PlayerLevel", playerLevel.ToString() },
                { "XFollow", xFollow.ToString() },
                { "Invitation", invitation.ToString() },
                { "CurrentEnergy", currentEnergy.ToString() }
            }
        };

        foreach (var botLevel in botLevels)
        {
            request.Data.Add(botLevel.Key, botLevel.Value.ToString());
        }

        Debug.Log("Saving player data:");
        foreach (var data in request.Data)
        {
            Debug.Log($"Key: {data.Key}, Value: {data.Value}");
        }

        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    private IEnumerator SavePlayerDataCoroutine()
    {
        _isSavingData = true;

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Score", _scoreToSave.ToString() },
                { "PlayerLevel", _playerLevelToSave.ToString() },
                { "XFollow", _xFollowToSave.ToString() },
                { "Invitation", _invitationToSave.ToString() },
                { "CurrentEnergy", _currentEnergyToSave.ToString() }
            }
        };

        foreach (var botLevel in _botLevelsToSave)
        {
            request.Data.Add(botLevel.Key, botLevel.Value.ToString());
        }

        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);

        yield return new WaitForSeconds(30); // 30秒ごとにデータを保存

        _isSavingData = false;
    }

    private void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("プレイヤーデータが正常に保存されました！");
        _isSavingData = false;
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("プレイヤーデータの保存中にエラーが発生しました: " + error.GenerateErrorReport());
        _isSavingData = false;
    }

    private IEnumerator AutoSaveData()
    {
        while (true)
        {
            yield return new WaitForSeconds(30); // 30秒ごとにデータを自動保存

            // データがロードされているかどうかを確認
            if (_isDataLoaded && _dataChanged && !_isSavingData)
            {
                SavePlayerDataImmediate(_scoreToSave, _playerLevelToSave, _xFollowToSave, _invitationToSave, _botLevelsToSave, _currentEnergyToSave);
                _dataChanged = false;
            }
        }
    }

    public void LoadPlayerData()
    {
        var request = new GetUserDataRequest
        {
            Keys = null
        };

        PlayFabClientAPI.GetUserData(request, OnDataReceived, OnError);
    }

    private void OnDataReceived(GetUserDataResult result)
    {
        Debug.Log("OnDataReceived called");

        if (result.Data != null)
        {
            foreach (var kvp in result.Data)
            {
                Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value.Value}");
            }

            if (ScoreManager.Instance != null && result.Data.TryGetValue("Score", out var scoreData))
            {
                int score;
                if (int.TryParse(scoreData.Value, out score))
                {
                    ScoreManager.Instance.SetScore(score);
                }
                else
                {
                    Debug.LogWarning("Scoreの値が不正です: " + scoreData.Value);
                }
            }

            if (result.Data.TryGetValue("PlayerLevel", out var playerLevelData))
            {
                int playerLevel;
                if (int.TryParse(playerLevelData.Value, out playerLevel))
                {
                    Debug.Log($"Loading PlayerLevel: {playerLevel}");
                    LevelManager.Instance.SetPlayerLevel(playerLevel);
                }
                else
                {
                    Debug.LogWarning("PlayerLevelの値が不正です: " + playerLevelData.Value);
                }
            }

            if (result.Data.TryGetValue("XFollow", out var xFollowData))
            {
                int xFollow;
                if (int.TryParse(xFollowData.Value, out xFollow))
                {
                    _xFollowToSave = xFollow;
                }
                else
                {
                    Debug.LogWarning("XFollowの値が不正です: " + xFollowData.Value);
                    _xFollowToSave = 0;
                }
            }
            else
            {
                _xFollowToSave = 0; // デフォルト値
            }

            if (result.Data.TryGetValue("Invitation", out var invitationData))
            {
                int invitation;
                if (int.TryParse(invitationData.Value, out invitation))
                {
                    _invitationToSave = invitation;
                }
                else
                {
                    Debug.LogWarning("Invitationの値が不正です: " + invitationData.Value);
                    _invitationToSave = 0;
                }
            }
            else
            {
                _invitationToSave = 0; // デフォルト値
            }

            if (result.Data.TryGetValue("CurrentEnergy", out var currentEnergyData))
            {
                int loadedEnergy;
                if (int.TryParse(currentEnergyData.Value, out loadedEnergy))
                {
                    Debug.Log($"Loaded energy: {loadedEnergy}");

                    // オフライン期間に基づいたスタミナ回復を適用
                    if (PlayerPrefs.HasKey(lastSessionEndTimeKey))
                    {
                        DateTime lastSessionEndTime = DateTime.Parse(PlayerPrefs.GetString(lastSessionEndTimeKey));
                        TimeSpan offlineDuration = DateTime.UtcNow - lastSessionEndTime;
                        int energyToAdd = CalculateEnergyForOfflineDuration(offlineDuration);
                        loadedEnergy += energyToAdd;
                        Debug.Log($"Energy after offline recovery: {loadedEnergy}");
                    }

                    EnergyManager.Instance.SetCurrentEnergy(loadedEnergy);
                }
                else
                {
                    Debug.LogWarning("CurrentEnergyの値が不正です: " + currentEnergyData.Value);
                }
            }

            if (BotManager.Instance != null)
            {
                foreach (var data in result.Data)
                {
                    if (data.Key.StartsWith("Bot_"))
                    {
                        int level;
                        if (int.TryParse(data.Value.Value, out level))
                        {
                            BotManager.Instance.SetBotLevel(data.Key, level);
                        }
                        else
                        {
                            Debug.LogWarning($"Bot levelの値が不正です: Key: {data.Key}, Value: {data.Value.Value}");
                        }
                    }
                }
            }

            _isDataLoaded = true; // データロード完了フラグを設定
            Debug.Log("プレイヤーデータが正常に読み込まれました！");
        }
        else
        {
            Debug.Log("プレイヤーデータが見つかりません。");
            _isDataLoaded = true; // データロード完了フラグを設定
        }
    }

    public bool IsDataLoaded()
    {
        return _isDataLoaded;
    }

    public string GetMasterPlayerAccountID()
    {
        return _playFabId;
    }

    public void IncreaseXFollow(int amount)
    {
        _xFollowToSave += amount;
    }
}
