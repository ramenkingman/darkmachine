using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }
    private TextMeshProUGUI scoreText;
    private bool scoreChanged = false; // スコアが変更されたかどうかを示すフラグ

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
        scoreChanged = true; // スコアが変更されたことを記録
        UpdateScoreText();
        OnScoreChanged?.Invoke(Score);
    }

    public void SetScore(int newScore)
    {
        Score = newScore;
        scoreChanged = true; // スコアが変更されたことを記録
        UpdateScoreText();
        OnScoreChanged?.Invoke(Score);
    }

    public void ResetScore()
    {
        Score = 0;
        scoreChanged = true; // スコアが変更されたことを記録
        UpdateScoreText();
        OnScoreChanged?.Invoke(Score);
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = Score.ToString("N0");
        }
    }

    public bool HasScoreChanged()
    {
        return scoreChanged;
    }

    public void ResetScoreChangedFlag()
    {
        scoreChanged = false;
    }
}
