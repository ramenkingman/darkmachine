using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class UpdateScoreOnMatch : MonoBehaviour
{
    public Button checkAndIncreaseScoreButton;
    public TextMeshProUGUI inputText;
    public TextMeshProUGUI statusText;
    
    private string currentPlayerID;
    private const int SCORE_INCREMENT = 10000;
    private const int INVITATION_INCREMENT = 1;

    void Start()
    {
        if (checkAndIncreaseScoreButton == null || inputText == null || statusText == null)
        {
            Debug.LogError("CheckAndIncreaseScoreButton, InputText, or StatusText is not assigned. Please assign them in the Inspector.");
            return;
        }

        checkAndIncreaseScoreButton.onClick.AddListener(OnCheckAndIncreaseScoreButtonClick);
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
        statusText.text = "Logged in. Player ID: " + currentPlayerID;
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("PlayFab login failed: " + error.GenerateErrorReport());
        statusText.text = "Login failed. Please try again.";
    }

    private void OnCheckAndIncreaseScoreButtonClick()
    {
        string inputId = inputText.text;

        if (string.IsNullOrEmpty(inputId))
        {
            statusText.text = "Input ID is empty.";
            return;
        }

        statusText.text = "Checking...";

        // Get all player IDs and compare
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = "Score",
            MaxResultsCount = 100 // Adjust this number based on expected number of players
        }, result =>
        {
            bool matchFound = false;

            foreach (var entry in result.Leaderboard)
            {
                if (entry.PlayFabId == inputId && entry.PlayFabId != currentPlayerID)
                {
                    matchFound = true;
                    StartCoroutine(IncreaseScores(entry.PlayFabId));
                    break;
                }
            }

            if (!matchFound)
            {
                statusText.text = "No matching ID found.";
            }

        }, error =>
        {
            Debug.LogError("Failed to get leaderboard: " + error.GenerateErrorReport());
            statusText.text = "Failed to get leaderboard.";
        });
    }

    private IEnumerator IncreaseScores(string matchedPlayerId)
    {
        // Increase score for current player
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "Score", Value = SCORE_INCREMENT }
            }
        }, result =>
        {
            Debug.Log("Current player score updated.");
        }, error =>
        {
            Debug.LogError("Failed to update current player score: " + error.GenerateErrorReport());
        });

        yield return new WaitForSeconds(1); // Small delay to ensure the first update completes

        // Increase score for matched player using CloudScript
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
        {
            FunctionName = "increaseScoreForPlayer",
            FunctionParameter = new { playFabId = matchedPlayerId, increment = SCORE_INCREMENT },
            GeneratePlayStreamEvent = true
        }, result =>
        {
            Debug.Log("Matched player score updated.");
        }, error =>
        {
            Debug.LogError("Failed to update matched player score: " + error.GenerateErrorReport());
        });

        yield return new WaitForSeconds(1); // Small delay to ensure the first update completes

        // Increase invitations for current player
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

            currentInvitations += INVITATION_INCREMENT;

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    { "Invitations", currentInvitations.ToString() }
                }
            }, updateResult =>
            {
                Debug.Log("Current player invitations updated.");
                statusText.text = "Scores and invitations updated successfully!";
            }, error =>
            {
                Debug.LogError("Failed to update current player invitations: " + error.GenerateErrorReport());
                statusText.text = "Failed to update current player invitations.";
            });
        }, error =>
        {
            Debug.LogError("Failed to get current player data: " + error.GenerateErrorReport());
            statusText.text = "Failed to get current player data.";
        });
    }
}
