using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }
    private TextMeshProUGUI scoreText;

    public delegate void ScoreChanged(int newScore);
    public event ScoreChanged OnScoreChanged;

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

    public void SetScoreText(TextMeshProUGUI text)
    {
        scoreText = text;
        UpdateScoreText();
    }

    public void AddScore(int amount)
    {
        Score += amount;
        UpdateScoreText();
        OnScoreChanged?.Invoke(Score);

        PlayFabManager.Instance.SavePlayerData(Score, LevelManager.Instance.PlayerLevel, PlayFabManager.Instance.XFollowToSave, PlayFabManager.Instance.InvitationToSave, BotManager.Instance.GetBotLevels(), EnergyManager.Instance.CurrentEnergy); // 追加
    }

    public void SetScore(int newScore)
    {
        Score = newScore;
        UpdateScoreText();
        OnScoreChanged?.Invoke(Score);

        PlayFabManager.Instance.SavePlayerData(Score, LevelManager.Instance.PlayerLevel, PlayFabManager.Instance.XFollowToSave, PlayFabManager.Instance.InvitationToSave, BotManager.Instance.GetBotLevels(), EnergyManager.Instance.CurrentEnergy); // 追加
    }

    public void ResetScore()
    {
        Score = 0;
        UpdateScoreText();
        OnScoreChanged?.Invoke(Score);

        PlayFabManager.Instance.SavePlayerData(Score, LevelManager.Instance.PlayerLevel, PlayFabManager.Instance.XFollowToSave, PlayFabManager.Instance.InvitationToSave, BotManager.Instance.GetBotLevels(), EnergyManager.Instance.CurrentEnergy); // 追加
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = Score.ToString("N0");
        }
    }
}
