using UnityEngine;
using UnityEngine.UI;

public class OpenURLButton : MonoBehaviour
{
    [SerializeField] private string url;

    public void OpenURL()
    {
        Application.ExternalEval($"openExternalURL('{url}');");
    }
}
