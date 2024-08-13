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
    public Button resetButton; // リセットボタンをインスペクターで指定
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

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetPlayerData);
        }

        // PlayFabから初期状態を取得
        LoadPlayerData();
    }

    private void OnButton1Clicked()
    {
        hasPressedButton1 = true;
        SaveButton1Pressed();
        Debug.Log("Button1 pressed. State saved: hasPressedButton1 = " + hasPressedButton1);
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
                button2.gameObject.SetActive(false); // ボタン2を完全に非アクティブにする
                Debug.Log("Button2 pressed. Score increased by 10000. Current score: " + currentScore);
            }
            else
            {
                button2.gameObject.SetActive(false); // 既にスコアが増加されている場合、ボタン2を完全に非アクティブにする
                Debug.Log("Button2 pressed but score already increased.");
            }
        }
        else
        {
            StartCoroutine(ShowWarning());
            Debug.Log("Button2 pressed without pressing Button1. Warning shown.");
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
            Keys = new List<string> { "ButtonHandler2_HasPressedButton1", "ButtonHandler2_HasIncreasedScore" }
        };

        PlayFabClientAPI.GetUserData(request, OnDataReceived, OnError);
    }

    private void OnDataReceived(GetUserDataResult result)
    {
        if (result.Data != null)
        {
            if (result.Data.ContainsKey("ButtonHandler2_HasPressedButton1"))
            {
                hasPressedButton1 = bool.Parse(result.Data["ButtonHandler2_HasPressedButton1"].Value);
                Debug.Log("Loaded hasPressedButton1: " + hasPressedButton1);
            }

            if (result.Data.ContainsKey("ButtonHandler2_HasIncreasedScore"))
            {
                hasIncreasedScore = bool.Parse(result.Data["ButtonHandler2_HasIncreasedScore"].Value);
                Debug.Log("Loaded hasIncreasedScore: " + hasIncreasedScore);
            }

            // 取得したデータに基づいてボタン2の状態を設定
            button2.gameObject.SetActive(!hasIncreasedScore);
        }
        else
        {
            // データが存在しない場合もボタン2をアクティブにする
            button2.gameObject.SetActive(true);
        }
    }

    private void SaveButton1Pressed()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "ButtonHandler2_HasPressedButton1", hasPressedButton1.ToString() }
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
                { "ButtonHandler2_HasIncreasedScore", hasIncreasedScore.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    public void ResetPlayerData() // public に変更
    {
        hasPressedButton1 = false;
        hasIncreasedScore = false;
        var initialScore = 0; // 初期スコアを適切に設定

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "ButtonHandler2_HasPressedButton1", hasPressedButton1.ToString() },
                { "ButtonHandler2_HasIncreasedScore", hasIncreasedScore.ToString() },
                { "Score", initialScore.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnResetDataSend, OnError);

        // スコアをリセット
        ScoreManager.Instance.SetScore(initialScore);
        // ボタン2を再びアクティブにする
        button2.gameObject.SetActive(true);
        Debug.Log("Player data reset. Initial score: " + initialScore);
    }

    private void OnResetDataSend(UpdateUserDataResult result)
    {
        Debug.Log("プレイヤーデータがリセットされました！");
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
