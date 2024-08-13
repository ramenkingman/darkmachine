using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class KeyboardOpener : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button openKeyboardButton;
    private bool hasOpenedOnce = false; // フラグを追加

    void Start()
    {
        if (inputField == null)
        {
            Debug.LogError("Input Field is not assigned.");
            return;
        }

        if (openKeyboardButton == null)
        {
            Debug.LogError("Open Keyboard Button is not assigned.");
            return;
        }

        openKeyboardButton.onClick.AddListener(OpenAndCloseKeyboard);
    }

    void OnDestroy()
    {
        if (openKeyboardButton != null)
        {
            openKeyboardButton.onClick.RemoveListener(OpenAndCloseKeyboard);
        }
    }

    private void OpenAndCloseKeyboard()
    {
        if (!hasOpenedOnce)
        {
            CoroutineManager.Instance.StartManagedCoroutine(OpenKeyboardTemporarily());
            hasOpenedOnce = true; // フラグを設定
        }
        else
        {
            Debug.Log("Keyboard has already been opened once, not opening again.");
        }
    }

    private IEnumerator OpenKeyboardTemporarily()
    {
        // Open the keyboard
        TouchScreenKeyboard keyboard = TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.Default, autocorrection: false, multiline: false, secure: false, alert: false, inputField.placeholder.GetComponent<TMP_Text>().text);

        // Wait for 0.1 seconds
        yield return new WaitForSeconds(0.1f);

        // Close the keyboard
        if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Visible)
        {
            keyboard.active = false;
        }
    }
}
