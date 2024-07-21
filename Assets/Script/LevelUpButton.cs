using UnityEngine;
using UnityEngine.UI;

public class LevelUpButton : MonoBehaviour
{
    public static LevelUpButton Instance { get; private set; }
    public Button levelUpButton;
    public Image buttonImage;
    public Sprite levelUpSprite;
    public Sprite comingSoonSprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        levelUpButton.onClick.AddListener(OnLevelUpButtonClicked);
        UpdateButton();
    }

    private void Update()
    {
        UpdateButton();
    }

    private void UpdateButton()
    {
        if (LevelManager.Instance.IsMaxLevel())
        {
            buttonImage.sprite = comingSoonSprite;
            levelUpButton.interactable = false;
        }
        else if (LevelManager.Instance.CanLevelUp())
        {
            buttonImage.sprite = levelUpSprite;
            levelUpButton.interactable = true;
        }
        else
        {
            buttonImage.sprite = levelUpSprite;
            levelUpButton.interactable = false;
        }
    }

    private void OnLevelUpButtonClicked()
    {
        LevelManager.Instance.LevelUp();
    }

    public void UpdateCost(int cost)
    {
        // ボタンのテキストを更新する場合のコードを追加
    }
}
