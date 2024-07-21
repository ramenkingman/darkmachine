using UnityEngine;
using UnityEngine.UI;

public class MobileInputField : MonoBehaviour
{
    private InputField inputField;
    private TouchScreenKeyboard keyboard;

    void Start()
    {
        inputField = GetComponent<InputField>();
        if (inputField == null)
        {
            Debug.LogError("InputField component not found.");
            return;
        }

        inputField.onEndEdit.AddListener(OnEndEdit);
    }

    void OnDestroy()
    {
        if (inputField != null)
        {
            inputField.onEndEdit.RemoveListener(OnEndEdit);
        }
    }

    public void OnEndEdit(string text)
    {
        ShowKeyboard();
    }

    public void ShowKeyboard()
    {
        if (keyboard == null || !keyboard.active)
        {
            keyboard = TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.Default);
        }
    }

    void Update()
    {
        if (keyboard != null && inputField != null)
        {
            if (keyboard.status == TouchScreenKeyboard.Status.Visible)
            {
                inputField.text = keyboard.text;
            }
        }
    }
}
