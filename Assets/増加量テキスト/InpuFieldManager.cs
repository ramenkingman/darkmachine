using UnityEngine;
using TMPro;

public class KeyboardInputHandler : MonoBehaviour
{
    public TMP_InputField inputField;
    private TouchScreenKeyboard keyboard;

    void Start()
    {
        if (inputField == null)
        {
            Debug.LogError("Input Field is not assigned.");
            return;
        }

        inputField.onSelect.AddListener(OnInputFieldSelected);
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
        if (keyboard == null || keyboard.status != TouchScreenKeyboard.Status.Visible)
        {
            keyboard = TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.Default, autocorrection: false, multiline: false, secure: false, alert: false, inputField.placeholder.GetComponent<TMP_Text>().text);
        }
    }

    void Update()
    {
        if (keyboard != null)
        {
            if (keyboard.status == TouchScreenKeyboard.Status.Visible)
            {
                inputField.text = keyboard.text;
            }
            else if (keyboard.status == TouchScreenKeyboard.Status.Done || keyboard.status == TouchScreenKeyboard.Status.Canceled)
            {
                keyboard = null;
            }
        }
    }
}