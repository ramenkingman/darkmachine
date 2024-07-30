using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class ButtonHandler2 : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public GameObject warningObject; // 特定のオブジェクトをインスペクターで指定

    private bool hasPressedButton1 = false;
    private bool hasIncreasedScore = false;

    private void Start()
    {
        if (button1 != null)
        {
            button1.onClick.AddListener(OnButton1Clicked);
        }

        if (button2 != null)
        {
            button2.onClick.AddListener(OnButton2Clicked);
        }

        // PlayFabから初期状態を取得
        LoadPlayerData();
    }

    private void OnButton1Clicked()
    {
        hasPressedButton1 = true;
        SaveButton1Pressed();
    }

    private void OnButton2Clicked()
    {
        if (hasPressedButton1)
        {
            if (!hasIncreasedScore)
            {
                var currentScore = ScoreManager.Instance.Score + 10000; // 5000から10000に変更
                ScoreManager.Instance.SetScore(currentScore);
                hasIncreasedScore = true;
                SavePlayerData(currentScore);
                button2.interactable = false; // ボタン2を非アクティブにする
            }
            else
            {
                button2.interactable = false; // 既にスコアが増加されている場合、ボタン2を常に非アクティブにする
            }
        }
        else
        {
            StartCoroutine(ShowWarning());
        }
    }

    private IEnumerator ShowWarning()
    {
        warningObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        warningObject.SetActive(false);
    }

    private void LoadPlayerData()
    {
        var request = new GetUserDataRequest
        {
            Keys = new List<string> { "HasPressedButton1", "HasIncreasedScore" }
        };

        PlayFabClientAPI.GetUserData(request, OnDataReceived, OnError);
    }

    private void OnDataReceived(GetUserDataResult result)
    {
        if (result.Data != null)
        {
            if (result.Data.ContainsKey("HasPressedButton1"))
            {
                hasPressedButton1 = bool.Parse(result.Data["HasPressedButton1"].Value);
            }

            if (result.Data.ContainsKey("HasIncreasedScore"))
            {
                hasIncreasedScore = bool.Parse(result.Data["HasIncreasedScore"].Value);
            }

            // 取得したデータに基づいてボタン2の状態を設定
            button2.interactable = !hasIncreasedScore;
        }
        else
        {
            // データが存在しない場合もボタン2を非アクティブにする
            button2.interactable = true;
        }
    }

    private void SaveButton1Pressed()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "HasPressedButton1", hasPressedButton1.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    private void SavePlayerData(int score)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Score", score.ToString() },
                { "HasIncreasedScore", hasIncreasedScore.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    private void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("プレイヤーデータが正常に保存されました！");
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("プレイヤーデータの保存中にエラーが発生しました: " + error.GenerateErrorReport());
    }
}