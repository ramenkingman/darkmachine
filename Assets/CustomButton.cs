using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomButton : MonoBehaviour
{
    public Button actionButton; // ボタンをインスペクターで指定
    public string xAccountURL; // インスペクターで指定するXアカウントのURL

    private void Start()
    {
        if (actionButton != null)
        {
            actionButton.onClick.AddListener(OnActionButtonClicked);
        }
    }

    private void OnActionButtonClicked()
    {
        NavigateToURL();
        IncreaseXFollow();
    }

    private void NavigateToURL()
    {
        if (!string.IsNullOrEmpty(xAccountURL))
        {
            Application.OpenURL(xAccountURL);
        }
        else
        {
            Debug.LogError("XアカウントのURLが指定されていません。");
        }
    }

    private void IncreaseXFollow()
    {
        if (PlayFabManager.Instance == null)
        {
            Debug.LogError("PlayFabManagerのインスタンスが見つかりません。");
            return;
        }

        if (!PlayFabManager.Instance.IsDataLoaded())
        {
            Debug.LogWarning("データがロードされていないため、XFollowを増やすことができません。");
            return;
        }

        if (ScoreManager.Instance == null || LevelManager.Instance == null || BotManager.Instance == null)
        {
            Debug.LogError("必要なマネージャーのインスタンスが見つかりません。");
            return;
        }

        // 現在のデータを取得
        var currentScore = ScoreManager.Instance.Score;
        var currentLevel = LevelManager.Instance.PlayerLevel;
        var botLevels = BotManager.Instance.GetBotLevels();
        var xFollow = PlayFabManager.Instance.XFollowToSave + 1; // プロパティを使用
        var invitation = PlayFabManager.Instance.InvitationToSave; // プロパティを使用
        var currentEnergy = EnergyManager.Instance.CurrentEnergy; // 追加

        // データをPlayFabに保存
        PlayFabManager.Instance.SavePlayerData(currentScore, currentLevel, xFollow, invitation, botLevels, currentEnergy); // 追加
    }
}