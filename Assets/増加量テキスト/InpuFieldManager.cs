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
        if (keyboard == null || !keyboard.active)
        {
            keyboard = TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.Default);
        }
    }


    void Update()
    {
        if (keyboard != null && keyboard.active)
        {
            inputField.text = keyboard.text;
        }
    }
}



