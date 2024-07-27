using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class LeaderboardCheck : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button checkButton;
    public GameObject resultObject;
    public GameObject errorObject;
    public Button disableButton;

    private string playerDisplayName;

    void Start()
    {
        checkButton.onClick.AddListener(OnCheckButtonClick);
        playerDisplayName = PlayFabManager.Instance.GetMasterPlayerAccountID();
        if (PlayerPrefs.GetInt("DisableButton", 0) == 1)
        {
            disableButton.gameObject.SetActive(false);
        }
    }

    private void OnCheckButtonClick()
    {
        string inputName = inputField.text;

        if (string.IsNullOrEmpty(inputName))
        {
            ShowErrorObject();
            return;
        }

        GetLeaderboard(inputName);
    }

    private void GetLeaderboard(string inputName)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "LeaderboardName",
            MaxResultsCount = 100
        };

        PlayFabClientAPI.GetLeaderboard(request, result =>
        {
            bool nameFound = false;

            foreach (var item in result.Leaderboard)
            {
                if (item.DisplayName == inputName)
                {
                    nameFound = true;
                    if (item.PlayFabId != playerDisplayName)
                    {
                        IncreaseScore(item.PlayFabId);
                    }
                    else
                    {
                        ShowErrorObject();
                        return;
                    }
                }
            }

            if (nameFound)
            {
                IncreaseScore(playerDisplayName);
                ShowResultObject();
            }
            else
            {
                ShowErrorObject();
            }
        }, error =>
        {
            Debug.LogError("Error retrieving leaderboard: " + error.GenerateErrorReport());
            ShowErrorObject();
        });
    }

    private void IncreaseScore(string playFabId)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "Score", Value = 10000 }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, result =>
        {
            Debug.Log("Score increased successfully.");
        }, error =>
        {
            Debug.LogError("Error updating score: " + error.GenerateErrorReport());
        });
    }

    private void ShowResultObject()
    {
        resultObject.SetActive(true);
        StartCoroutine(HideObjectAfterDelay(resultObject, 3f));
        disableButton.gameObject.SetActive(false);
        PlayerPrefs.SetInt("DisableButton", 1);
        PlayerPrefs.Save();
    }

    private void ShowErrorObject()
    {
        errorObject.SetActive(true);
        StartCoroutine(HideObjectAfterDelay(errorObject, 3f));
    }

    private IEnumerator HideObjectAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}
