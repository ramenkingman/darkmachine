using UnityEngine;
using UnityEngine.UI;

public class XFollowButton : MonoBehaviour
{
    public Button xFollowButton; // ボタンをインスペクターで指定

    private void Start()
    {
        if (xFollowButton != null)
        {
            xFollowButton.onClick.AddListener(OnXFollowButtonClicked);
        }
    }

    private void OnXFollowButtonClicked()
    {
        if (PlayFabManager.Instance == null)
        {
            Debug.LogError("PlayFabManagerのインスタンスが見つかりません。");
            return;
        }

        if (!PlayFabManager.Instance.IsDataLoaded())
        {
            Debug.LogWarning("データがロードされていないため、処理を実行できません。");
            return;
        }

        var xFollow = PlayFabManager.Instance.XFollowToSave;
        if (xFollow > 0)
        {
            // xFollowの値を100増加させる
            PlayFabManager.Instance.IncreaseXFollow(100);
            xFollow = PlayFabManager.Instance.XFollowToSave; // 更新後の値を取得

            var currentScore = ScoreManager.Instance.Score + 5000;
            ScoreManager.Instance.SetScore(currentScore);

            var currentLevel = LevelManager.Instance.PlayerLevel;
            var botLevels = BotManager.Instance.GetBotLevels();
            var invitation = PlayFabManager.Instance.InvitationToSave;
            var currentEnergy = EnergyManager.Instance.CurrentEnergy; // 追加

            // データをPlayFabに保存
            PlayFabManager.Instance.SavePlayerData(currentScore, currentLevel, xFollow, invitation, botLevels, currentEnergy); // 追加
        }
        else
        {
            Debug.Log("XFollowの値が0のため、処理を実行しません。");
        }
    }
}
