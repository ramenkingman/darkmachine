using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CopyTextToClipboard : MonoBehaviour
{
    public Button copyButton;
    public TextMeshProUGUI textToCopy;
    public TextMeshProUGUI statusText;

    void Start()
    {
        if (copyButton == null || textToCopy == null || statusText == null)
        {
            Debug.LogError("CopyButton, TextToCopy, or StatusText is not assigned. Please assign them in the Inspector.");
            return;
        }

        copyButton.onClick.AddListener(CopyToClipboard);
    }

    private void CopyToClipboard()
    {
        string text = textToCopy.text;

        if (!string.IsNullOrEmpty(text))
        {
            // コピー前のクリップボードの文字列をログで表示
            Debug.Log($"コピー前 : {GUIUtility.systemCopyBuffer}");
            
            // クリップボードへ文字を設定(コピー)
            GUIUtility.systemCopyBuffer = text;
            
            // コピー後のクリップボードの文字列をログで表示
            Debug.Log($"コピー後 : {GUIUtility.systemCopyBuffer}");

            statusText.text = "Text copied to clipboard!";
        }
        else
        {
            statusText.text = "Text is not available.";
        }
    }
}
