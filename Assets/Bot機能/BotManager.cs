using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BotManager : MonoBehaviour
{
    public static BotManager Instance { get; private set; }

    public Bot JrDealer;
    public Bot SrDealer;

    public TextMeshProUGUI profitPerHourText;
    public TextMeshProUGUI hourlyAddedScoreText;

    private DateTime _lastScoreAddedTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeBots();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("BotManager Start: Starting IncreaseScoreOverTime coroutine");
        if (profitPerHourText == null)
        {
            profitPerHourText = GameObject.Find("ProfitPerHourText").GetComponent<TextMeshProUGUI>();
        }
        if (hourlyAddedScoreText == null)
        {
            hourlyAddedScoreText = GameObject.Find("HourlyAddedScoreText").GetComponent<TextMeshProUGUI>();
        }
        UpdateBotUI();
        StartCoroutine(IncreaseScoreOverTime());
    }

    private void InitializeBots()
    {
        JrDealer = new Bot("Jr Dealer", new int[] { 1000, 1200, 1400, 1600 }, new int[] { 10, 15, 20, 25 });
        SrDealer = new Bot("Sr Dealer", new int[] { 1500, 2000, 2500, 3000 }, new int[] { 22, 28, 34, 40 });

        JrDealer.InitializeLevel();
        SrDealer.InitializeLevel();

        Debug.Log("JrDealer初期レベル: " + JrDealer.Level);
        Debug.Log("SrDealer初期レベル: " + SrDealer.Level);
    }

    public void LevelUpBot(Bot bot)
    {
        if (ScoreManager.Instance.Score >= bot.GetCurrentCost() && bot.CanLevelUp(ScoreManager.Instance.Score))
        {
            ScoreManager.Instance.AddScore(-bot.GetCurrentCost());
            bot.LevelUp();
            UpdateBotUI();
            SaveBotData();
        }
    }

    public void UpdateBotUI()
    {
        if (profitPerHourText == null)
        {
            profitPerHourText = GameObject.Find("ProfitPerHourText").GetComponent<TextMeshProUGUI>();
        }
        if (hourlyAddedScoreText == null)
        {
            hourlyAddedScoreText = GameObject.Find("HourlyAddedScoreText").GetComponent<TextMeshProUGUI>();
        }

        int totalScoresPerHour = JrDealer.GetCurrentScorePerHour() + SrDealer.GetCurrentScorePerHour();
        profitPerHourText.text = totalScoresPerHour.ToString();
    }

    private IEnumerator IncreaseScoreOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(3600);
            DateTime currentTime = DateTime.UtcNow;
            if (_lastScoreAddedTime != default(DateTime))
            {
                TimeSpan elapsed = currentTime - _lastScoreAddedTime;
                int secondsElapsed = (int)elapsed.TotalSeconds;

                AddScoresBasedOnElapsedTime(secondsElapsed);
            }
            _lastScoreAddedTime = currentTime;
        }
    }

    public void AddScoresBasedOnElapsedTime(int secondsElapsed)
    {
        int scoresPerSecond = (JrDealer.GetCurrentScorePerHour() + SrDealer.GetCurrentScorePerHour()) / 3600;
        int scoresToAdd = scoresPerSecond * secondsElapsed;

        if (hourlyAddedScoreText != null)
        {
            hourlyAddedScoreText.text = $"Added Score: {scoresToAdd}";
        }

        ScoreManager.Instance.AddScore(scoresToAdd); // スコアを追加
        Debug.Log($"経過秒ごとに追加されたスコア: {scoresToAdd}");
    }

    public void SetBotLevel(string botName, int level)
    {
        if (botName == "Bot_JrDealer")
        {
            JrDealer.SetLevel(level);
        }
        else if (botName == "Bot_SrDealer")
        {
            SrDealer.SetLevel(level);
        }
        UpdateBotUI();
    }

    private void SaveBotData()
    {
        int currentScore = ScoreManager.Instance.Score;
        int currentLevel = LevelManager.Instance.PlayerLevel;
        int xFollow = PlayFabManager.Instance.XFollowToSave;
        int invitation = PlayFabManager.Instance.InvitationToSave;
        int currentEnergy = EnergyManager.Instance.CurrentEnergy;

        PlayFabManager.Instance.SavePlayerData(currentScore, currentLevel, xFollow, invitation, GetBotLevels(), currentEnergy);
    }

    public Dictionary<string, int> GetBotLevels()
    {
        return new Dictionary<string, int>
        {
            { "Bot_JrDealer", JrDealer.Level },
            { "Bot_SrDealer", SrDealer.Level }
        };
    }

    public List<Bot> GetBots()
    {
        return new List<Bot> { JrDealer, SrDealer };
    }

    public void SetLastScoreAddedTime(DateTime time)
    {
        _lastScoreAddedTime = time;
    }
}