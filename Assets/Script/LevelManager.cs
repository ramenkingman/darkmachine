using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public int PlayerLevel { get; private set; } = 0;
    public int[] LevelUpCosts = { 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000 };
    public int ScoreIncreaseAmount { get; private set; } = 1;
    private TextMeshProUGUI costText;

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

    private void Start()
    {
        FindCostText();
        UpdateLevelUpCost();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindCostText();
        UpdateLevelUpCost();
    }

    private void FindCostText()
    {
        if (SceneManager.GetActiveScene().name == "Mine")
        {
            GameObject costTextObject = GameObject.Find("CostText");
            if (costTextObject != null)
            {
                costText = costTextObject.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogError("CostTextオブジェクトが見つかりません。オブジェクト名を確認してください。");
            }
        }
        else
        {
            costText = null;
        }
    }

    public bool CanLevelUp()
    {
        if (PlayerLevel >= LevelUpCosts.Length) return false;
        return ScoreManager.Instance.Score >= LevelUpCosts[PlayerLevel];
    }

    public bool IsMaxLevel()
    {
        return PlayerLevel >= LevelUpCosts.Length;
    }

    public void LevelUp()
    {
        if (CanLevelUp())
        {
            ScoreManager.Instance.AddScore(-LevelUpCosts[PlayerLevel]);
            PlayerLevel++;
            UpdateScoreIncreaseAmount();
            UpdateLevelUpCost();

            // レベルアップしたときにデータを保存
            int xFollow = PlayFabManager.Instance.XFollowToSave;
            int invitation = PlayFabManager.Instance.InvitationToSave;
            int currentEnergy = EnergyManager.Instance.CurrentEnergy; // 追加
            PlayFabManager.Instance.SavePlayerData(ScoreManager.Instance.Score, PlayerLevel, xFollow, invitation, BotManager.Instance.GetBotLevels(), currentEnergy); // 追加
        }
    }

    private void UpdateScoreIncreaseAmount()
    {
        ScoreIncreaseAmount = PlayerLevel + 1;
    }

    private void UpdateLevelUpCost()
    {
        if (PlayerLevel < LevelUpCosts.Length)
        {
            if (LevelUpButton.Instance != null)
            {
                LevelUpButton.Instance.UpdateCost(LevelUpCosts[PlayerLevel]);
            }
            if (costText != null)
            {
                costText.text = LevelUpCosts[PlayerLevel].ToString("N0");
            }
        }
        else
        {
            if (costText != null)
            {
                costText.text = "Max Level";
            }
        }
    }

    public void SetPlayerLevel(int level)
    {
        PlayerLevel = level;
        UpdateScoreIncreaseAmount();
        UpdateLevelUpCost();
        Debug.Log($"Player level set to {level}");
    }
}
