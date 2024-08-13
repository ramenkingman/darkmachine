using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BotUIManager : MonoBehaviour
{
    public TextMeshProUGUI jrDealerCostText;
    public Button jrDealerLevelUpButton;
    public GameObject jrDealerPopup;
    public Button jrDealerGoAheadButton;
    public TextMeshProUGUI jrDealerLevelText;
    public TextMeshProUGUI jrDealerProfitText;

    public TextMeshProUGUI srDealerCostText;
    public Button srDealerLevelUpButton;
    public GameObject srDealerPopup;
    public Button srDealerGoAheadButton;
    public TextMeshProUGUI srDealerLevelText;
    public TextMeshProUGUI srDealerProfitText;

    public TextMeshProUGUI jrDealerLevelupPopupText;
    public TextMeshProUGUI srDealerLevelupPopupText;

    public TextMeshProUGUI profitOnJrDealerPopup;
    public TextMeshProUGUI costOnJrDealerPopup;
    public TextMeshProUGUI profitOnSrDealerPopup;
    public TextMeshProUGUI costOnSrDealerPopup;

    public Sprite maxLevelSprite;

    private void OnEnable()
    {
        ScoreManager.Instance.OnScoreChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        ScoreManager.Instance.OnScoreChanged -= OnScoreChanged;
    }

    private void Start()
    {
        jrDealerLevelUpButton.onClick.AddListener(() => ShowBotPopup(jrDealerPopup));
        srDealerLevelUpButton.onClick.AddListener(() => ShowBotPopup(srDealerPopup));

        jrDealerGoAheadButton.onClick.AddListener(() => LevelUpBot(BotManager.Instance.JrDealer, jrDealerLevelupPopupText));
        srDealerGoAheadButton.onClick.AddListener(() => LevelUpBot(BotManager.Instance.SrDealer, srDealerLevelupPopupText));

        UpdateBotUI();
    }

    private void ShowBotPopup(GameObject popup)
    {
        popup.SetActive(true);
    }

    private void LevelUpBot(Bot bot, TextMeshProUGUI levelupPopupText)
    {
        int previousProfit = bot.GetCurrentScorePerHour();
        BotManager.Instance.LevelUpBot(bot);
        UpdateBotUI();

        SetLevelupPopupText(bot, previousProfit, levelupPopupText);
    }

    private void SetLevelupPopupText(Bot bot, int previousProfit, TextMeshProUGUI levelupPopupText)
    {
        if (levelupPopupText == null)
        {
            Debug.LogError("LevelupPopupText is not set.");
            return;
        }

        int currentProfit = bot.GetCurrentScorePerHour();
        levelupPopupText.text = $"{previousProfit}→{currentProfit}";
    }

    private void OnScoreChanged(int newScore)
    {
        UpdateBotUI();
    }

    public void UpdateBotUI()
    {
        if (jrDealerCostText == null || srDealerCostText == null)
        {
            jrDealerCostText = GameObject.Find("CostOnRabitPopup1").GetComponent<TextMeshProUGUI>();
            srDealerCostText = GameObject.Find("CostOnRabitPopup2").GetComponent<TextMeshProUGUI>();
            jrDealerLevelUpButton = GameObject.Find("LevelUp1Button").GetComponent<Button>();
            srDealerLevelUpButton = GameObject.Find("LevelUp2Button").GetComponent<Button>();
            jrDealerPopup = GameObject.Find("JrDealerPopup");
            srDealerPopup = GameObject.Find("SrDealerPopup");
            jrDealerGoAheadButton = GameObject.Find("GoAheadButton1").GetComponent<Button>();
            srDealerGoAheadButton = GameObject.Find("GoAheadButton2").GetComponent<Button>();
            jrDealerLevelText = GameObject.Find("LevelOnRabitPopup1").GetComponent<TextMeshProUGUI>();
            srDealerLevelText = GameObject.Find("LevelOnRabitPopup2").GetComponent<TextMeshProUGUI>();
            jrDealerProfitText = GameObject.Find("ProfitOnRabitPopup1").GetComponent<TextMeshProUGUI>();
            srDealerProfitText = GameObject.Find("ProfitOnRabitPopup2").GetComponent<TextMeshProUGUI>();
            profitOnJrDealerPopup = GameObject.Find("ProfitOnJrDealerPopup").GetComponent<TextMeshProUGUI>();
            costOnJrDealerPopup = GameObject.Find("CostOnJrDealerPopup").GetComponent<TextMeshProUGUI>();
            profitOnSrDealerPopup = GameObject.Find("ProfitOnSrDealerPopup").GetComponent<TextMeshProUGUI>();
            costOnSrDealerPopup = GameObject.Find("CostOnSrDealerPopup").GetComponent<TextMeshProUGUI>();
            jrDealerLevelupPopupText = GameObject.Find("JrDealerLevelupPopupText").GetComponent<TextMeshProUGUI>();
            srDealerLevelupPopupText = GameObject.Find("SrDealerLevelupPopupText").GetComponent<TextMeshProUGUI>();
        }

        jrDealerCostText.text = BotManager.Instance.JrDealer.Level >= BotManager.Instance.JrDealer.GetMaxLevel() ? "-" : BotManager.Instance.JrDealer.GetCurrentCost().ToString();
        if (BotManager.Instance.JrDealer.Level >= BotManager.Instance.JrDealer.GetMaxLevel())
        {
            jrDealerLevelUpButton.interactable = false;
            jrDealerLevelUpButton.GetComponent<Image>().sprite = maxLevelSprite;
        }
        else
        {
            jrDealerLevelUpButton.interactable = BotManager.Instance.JrDealer.CanLevelUp(ScoreManager.Instance.Score);
        }

        srDealerCostText.text = BotManager.Instance.SrDealer.Level >= BotManager.Instance.SrDealer.GetMaxLevel() ? "-" : BotManager.Instance.SrDealer.GetCurrentCost().ToString();
        if (BotManager.Instance.SrDealer.Level >= BotManager.Instance.SrDealer.GetMaxLevel())
        {
            srDealerLevelUpButton.interactable = false;
            srDealerLevelUpButton.GetComponent<Image>().sprite = maxLevelSprite;
        }
        else
        {
            srDealerLevelUpButton.interactable = BotManager.Instance.SrDealer.CanLevelUp(ScoreManager.Instance.Score);
        }

        BotManager.Instance.UpdateBotUI();

        UpdateLevelText(BotManager.Instance.JrDealer, jrDealerLevelText);
        UpdateLevelText(BotManager.Instance.SrDealer, srDealerLevelText);

        UpdateProfitText(BotManager.Instance.JrDealer, jrDealerProfitText);
        UpdateProfitText(BotManager.Instance.SrDealer, srDealerProfitText);

        UpdatePopupText(BotManager.Instance.JrDealer, profitOnJrDealerPopup, costOnJrDealerPopup);
        UpdatePopupText(BotManager.Instance.SrDealer, profitOnSrDealerPopup, costOnSrDealerPopup);
    }

    private void UpdateLevelText(Bot bot, TextMeshProUGUI levelText)
    {
        if (bot.Level >= bot.GetMaxLevel())
        {
            levelText.text = "Level Max";
        }
        else
        {
            levelText.text = "Level " + (bot.Level + 1);
        }
    }

    private void UpdateProfitText(Bot bot, TextMeshProUGUI profitText)
    {
        if (bot.Level >= bot.GetMaxLevel())
        {
            profitText.text = "-";
        }
        else
        {
            profitText.text = bot.GetNextScorePerHour().ToString();
        }
    }

    private void UpdatePopupText(Bot bot, TextMeshProUGUI profitText, TextMeshProUGUI costText)
    {
        if (bot.Level >= bot.GetMaxLevel())
        {
            profitText.text = "-";
            costText.text = "-";
        }
        else
        {
            profitText.text = bot.GetNextScorePerHour().ToString();
            costText.text = bot.GetCurrentCost().ToString();
        }
    }
}
