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
    private DateTime _lastLoginTime;

    public int XFollowToSave => _xFollowToSave;
    public int InvitationToSave => _invitationToSave;
    public int CurrentEnergyToSave => _currentEnergyToSave;

    private static readonly string CUSTOM_ID_SAVE_KEY = "CUSTOM_ID_SAVE_KEY_GAME2";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Application.focusChanged += OnApplicationFocusChanged;
            Application.quitting += OnApplicationQuit;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Login();
        StartCoroutine(AutoSaveData());
    }

    private void OnApplicationFocusChanged(bool hasFocus)
    {
        if (!hasFocus)
        {
            SavePlayerDataImmediate(ScoreManager.Instance.Score, LevelManager.Instance.PlayerLevel, XFollowToSave, InvitationToSave, BotManager.Instance.GetBotLevels(), EnergyManager.Instance.CurrentEnergy);
            Debug.Log("Data saved on application losing focus.");
        }
    }

    private void OnApplicationQuit()
    {
        SavePlayerDataImmediate(ScoreManager.Instance.Score, LevelManager.Instance.PlayerLevel, XFollowToSave, InvitationToSave, BotManager.Instance.GetBotLevels(), EnergyManager.Instance.CurrentEnergy);
        Debug.Log("Data saved on application quit.");
    }

    private void OnDestroy()
    {
        Application.focusChanged -= OnApplicationFocusChanged;
        Application.quitting -= OnApplicationQuit;
    }

    private void Login()
    {
        _customID = LoadCustomID();
        var request = new LoginWithCustomIDRequest { CustomId = _customID, CreateAccount = _shouldCreateAccount };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        if (_shouldCreateAccount && !result.NewlyCreated)
        {
            Debug.LogWarning("CustomId :" + _customID + "は既に使われています。");
            return;
        }

        if (result.NewlyCreated)
        {
            SaveCustomID();
            Debug.Log("新規作成成功");
        }

        Debug.Log("ログイン成功!!");
        _playFabId = result.PlayFabId;
        _titlePlayerID = result.EntityToken.Entity.Id;

        LoadPlayerData();
        PlayFabClientAPI.GetTime(new GetTimeRequest(), OnGetTimeSuccess, OnGetTimeFailure);
    }

    private void OnGetTimeSuccess(GetTimeResult result)
    {
        DateTime currentTime = result.Time;
        if (PlayerPrefs.HasKey("LastLoginTime"))
        {
            _lastLoginTime = DateTime.Parse(PlayerPrefs.GetString("LastLoginTime"));
            TimeSpan timeDifference = currentTime - _lastLoginTime;
            int secondsElapsed = (int)timeDifference.TotalSeconds;
            EnergyManager.Instance.IncreaseEnergyBasedOnTime(secondsElapsed);
            BotManager.Instance?.AddScoresBasedOnElapsedTime(secondsElapsed); // ここで呼び出し
            Debug.Log($"Energy increased by {secondsElapsed} seconds of offline time.");

            // 前回のスコア加算時間も設定
            BotManager.Instance.SetLastScoreAddedTime(_lastLoginTime);
        }
        PlayerPrefs.SetString("LastLoginTime", currentTime.ToString());
        PlayerPrefs.Save();
    }

    private void OnGetTimeFailure(PlayFabError error)
    {
        Debug.LogError("Failed to get server time: " + error.GenerateErrorReport());
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
            return GenerateCustomID();
        }
        else
        {
            return id;
        }
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

        yield return new WaitForSeconds(2);

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
            yield return new WaitForSeconds(2); // 2秒ごとにデータを保存
            if ((ScoreManager.Instance.HasScoreChanged() || _dataChanged) && !_isSavingData && _isDataLoaded)
            {
                SavePlayerDataImmediate(ScoreManager.Instance.Score, LevelManager.Instance.PlayerLevel, XFollowToSave, InvitationToSave, BotManager.Instance.GetBotLevels(), EnergyManager.Instance.CurrentEnergy);
                ScoreManager.Instance.ResetScoreChangedFlag(); // スコア変更フラグをリセット
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
                int score = int.Parse(scoreData.Value);
                ScoreManager.Instance.SetScore(score);
            }

            if (LevelManager.Instance != null && result.Data.TryGetValue("PlayerLevel", out var playerLevelData))
            {
                int playerLevel = int.Parse(playerLevelData.Value);
                Debug.Log($"Loading PlayerLevel: {playerLevel}");
                LevelManager.Instance.SetPlayerLevel(playerLevel);
            }

            if (result.Data.TryGetValue("XFollow", out var xFollowData))
            {
                _xFollowToSave = int.Parse(xFollowData.Value);
            }
            else
            {
                _xFollowToSave = 0;
            }

            if (result.Data.TryGetValue("Invitation", out var invitationData))
            {
                _invitationToSave = int.Parse(invitationData.Value);
            }
            else
            {
                _invitationToSave = 0;
            }

            if (result.Data.TryGetValue("CurrentEnergy", out var currentEnergyData))
            {
                int loadedEnergy = int.Parse(currentEnergyData.Value);
                Debug.Log($"Loaded energy: {loadedEnergy}");
                EnergyManager.Instance.SetCurrentEnergy(loadedEnergy);
            }
            else
            {
                // ロードされたデータがない場合は、エネルギーをデフォルト値に設定しない
                Debug.LogWarning("No CurrentEnergy data found, keeping the current value.");
            }

            if (BotManager.Instance != null)
            {
                foreach (var data in result.Data)
                {
                    if (data.Key.StartsWith("Bot_"))
                    {
                        int level = int.Parse(data.Value.Value);
                        BotManager.Instance.SetBotLevel(data.Key, level);
                    }
                }
            }

            _isDataLoaded = true;
            Debug.Log("プレイヤーデータが正常に読み込まれました！");
        }
        else
        {
            Debug.Log("プレイヤーデータが見つかりません。");
            _isDataLoaded = true;
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
