using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonTextColorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Button button;
    public Image buttonImage; // 追加: ボタンの画像
    public TextMeshProUGUI buttonText;

    public Color normalTextColor = Color.white;
    public Color pressedTextColor = Color.red;
    public Color selectedTextColor = Color.green;
    public Color disabledTextColor = Color.gray;

    public Sprite normalSprite; // 追加: 通常時のスプライト
    public Sprite pressedSprite; // 追加: 押下時のスプライト
    public Sprite selectedSprite; // 追加: 選択時のスプライト
    public Sprite disabledSprite; // 追加: 無効時のスプライト

    public AudioClip tapSound; // 追加: タップ音のクリップ
    private AudioSource audioSource; // 追加: タップ音を再生するAudioSource

    private bool isPressed = false;
    private bool isSelected = false;

    private static ButtonTextColorChanger currentlySelectedButton;

    void Start()
    {
        // Nullチェックを追加
        if (button == null || buttonImage == null || buttonText == null)
        {
            Debug.LogError("Missing component references. Please assign all required components in the Inspector.");
            return;
        }

        button.onClick.AddListener(OnButtonClicked);
        UpdateButtonAppearance(normalTextColor, normalSprite);

        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSourceコンポーネントを追加
        audioSource.clip = tapSound;
    }

    void Update()
    {
        if (!button.interactable)
        {
            UpdateButtonAppearance(disabledTextColor, disabledSprite);
        }
        else if (isSelected)
        {
            UpdateButtonAppearance(selectedTextColor, selectedSprite);
        }
        else if (isPressed)
        {
            UpdateButtonAppearance(pressedTextColor, pressedSprite);
        }
        else
        {
            UpdateButtonAppearance(normalTextColor, normalSprite);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable && !isPressed && !isSelected)
        {
            UpdateButtonAppearance(selectedTextColor, selectedSprite);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable && !isPressed && !isSelected)
        {
            UpdateButtonAppearance(normalTextColor, normalSprite);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable)
        {
            isPressed = true;
            UpdateButtonAppearance(pressedTextColor, pressedSprite);
            PlayTapSound(); // ボタンが押されたときにサウンドを再生
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (button.interactable)
        {
            isPressed = false;
            if (!isSelected)
            {
                UpdateButtonAppearance(normalTextColor, normalSprite);
            }
        }
    }

    void OnButtonClicked()
    {
        if (currentlySelectedButton != null && currentlySelectedButton != this)
        {
            currentlySelectedButton.Deselect();
        }
        currentlySelectedButton = this;
        isSelected = true;
        UpdateButtonAppearance(selectedTextColor, selectedSprite);
    }

    public void Deselect()
    {
        isSelected = false;
        UpdateButtonAppearance(normalTextColor, normalSprite);
    }

    void UpdateButtonAppearance(Color textColor, Sprite sprite)
    {
        if (buttonText != null)
        {
            buttonText.color = textColor;
        }

        if (buttonImage != null)
        {
            buttonImage.sprite = sprite;
        }
    }

    void PlayTapSound() // 追加: タップ音を再生するメソッド
    {
        if (tapSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(tapSound);
        }
    }
}
