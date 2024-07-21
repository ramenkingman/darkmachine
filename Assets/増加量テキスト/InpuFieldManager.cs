using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KeyboardInputHandler : MonoBehaviour
{
    public TMP_InputField inputField;
    private TouchScreenKeyboard keyboard;
    private RectTransform canvasRectTransform;

    void Start()
    {
        if (inputField == null)
        {
            Debug.LogError("Input Field is not assigned.");
            return;
        }

        inputField.onSelect.AddListener(OnInputFieldSelected);
        canvasRectTransform = inputField.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    void OnDestroy()
    {
        if (inputField != null)
        {
            inputField.onSelect.RemoveListener(OnInputFieldSelected);
        }
    }

    private void OnInputFieldSelected(string text)
    {
        if (keyboard == null || !keyboard.active)
        {
            keyboard = TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.Default);
            // キーボードが表示された際にスクロール位置を固定する
            AdjustScreenForKeyboard();
        }
    }

    void Update()
    {
        if (keyboard != null && keyboard.active)
        {
            inputField.text = keyboard.text;
        }
    }

    private void AdjustScreenForKeyboard()
    {
        // 現在のスクロール位置を取得し、キーボードが表示されたときに変更されないようにする
        if (canvasRectTransform != null)
        {
            // 高さの制御（必要に応じて調整）
            canvasRectTransform.offsetMin = new Vector2(canvasRectTransform.offsetMin.x, 0);
        }
    }
}
