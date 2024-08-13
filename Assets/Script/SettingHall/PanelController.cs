using UnityEngine;

public class PanelController : MonoBehaviour
{
    public string panelName;
    private UIManager uiManager;

    void Start()
    {
        // パネルを初期状態で非表示にする
        gameObject.SetActive(false);

        uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.RegisterPanel(panelName, gameObject);
            Debug.Log("PanelController: UIManager found and panel registered as " + panelName);
        }
        else
        {
            Debug.LogError("UIManager not found in the scene.");
        }
    }

    public void ShowPanel()
    {
        Debug.Log("ShowPanel method called in PanelController for panel: " + panelName);
        if (uiManager != null)
        {
            uiManager.ShowPanel(panelName);
        }
    }

    public void HidePanel()
    {
        Debug.Log("HidePanel method called in PanelController for panel: " + panelName);
        if (uiManager != null)
        {
            uiManager.HidePanel(panelName);
        }
    }
}