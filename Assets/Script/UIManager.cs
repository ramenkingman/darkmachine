using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour
{
    public Button copyButton;
    public string displayName = "YourDisplayName";

    private OpenXURL openXURL;

    private void Start()
    {
        openXURL = FindObjectOfType<OpenXURL>();
        copyButton.onClick.AddListener(() => openXURL.CopyDisplayNameToClipboard(displayName));
    }
}
