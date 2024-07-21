using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class CheckInvitation : MonoBehaviour
{
    private string currentPlayerID;

    void Start()
    {
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
        currentPlayerID = result.PlayFabId;
        CheckInvitationCount();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("PlayFab login failed: " + error.GenerateErrorReport());
    }

    private void CheckInvitationCount()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest
        {
            PlayFabId = currentPlayerID
        }, userDataResult =>
        {
            int currentInvitations = 0;
            if (userDataResult.Data != null && userDataResult.Data.ContainsKey("Invitations"))
            {
                int.TryParse(userDataResult.Data["Invitations"].Value, out currentInvitations);
            }

            if (currentInvitations >= 1)
            {
                gameObject.SetActive(false);
            }
        }, error =>
        {
            Debug.LogError("Failed to get current player data: " + error.GenerateErrorReport());
        });
    }
}
