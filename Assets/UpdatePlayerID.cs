using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;

public class UpdatePlayerID : MonoBehaviour
{
    public TextMeshProUGUI playerIdText;
    private string playerId;
    private bool isUpdating = false;

    void Start()
    {
        if (playerIdText == null)
        {
            Debug.LogError("playerIdText is not assigned. Please assign a TextMeshProUGUI object.");
            return;
        }

        // 初回のログイン
        Login();
    }

    private void Login()
    {
        var customId = SystemInfo.deviceUniqueIdentifier;
        var request = new LoginWithCustomIDRequest { CustomId = customId, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        playerId = result.PlayFabId;
        if (!isUpdating)
        {
            StartCoroutine(UpdatePlayerIDCoroutine());
        }
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("PlayFab login failed: " + error.GenerateErrorReport());
    }

    private IEnumerator UpdatePlayerIDCoroutine()
    {
        isUpdating = true;
        while (true)
        {
            playerIdText.text = "Player ID: " + playerId;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
