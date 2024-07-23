using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class ChangeDisplayName : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button changeNameButton;
    private string inputText;

    void Start()
    {
        // ボタンにクリックイベントを追加
        changeNameButton.onClick.AddListener(OnChangeNameButtonClicked);

        // 入力フィールドの値をPlayerPrefsからロード
        if (PlayerPrefs.HasKey("InputFieldText"))
        {
            inputField.text = PlayerPrefs.GetString("InputFieldText");
        }
    }

    void OnDestroy()
    {
        // ボタンのクリックイベントを削除
        if (changeNameButton != null)
        {
            changeNameButton.onClick.RemoveListener(OnChangeNameButtonClicked);
        }
    }

    void OnChangeNameButtonClicked()
    {
        // 入力フィールドのテキストを取得
        inputText = inputField.text;

        // PlayFabでDisplayNameを更新
        var displayNameRequest = new UpdateUserTitleDisplayNameRequest { DisplayName = inputText };
        PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, OnDisplayNameUpdateSuccess, OnDisplayNameUpdateFailure);

        // Player Data (Title)にWalletAddressを保存
        var playerDataRequest = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "WalletAddress", inputText }
            }
        };
        PlayFabClientAPI.UpdateUserData(playerDataRequest, OnUserDataUpdateSuccess, OnUserDataUpdateFailure);

        // 入力フィールドのテキストをPlayerPrefsに保存
        PlayerPrefs.SetString("InputFieldText", inputText);

        // ボタンを非アクティブにする
        changeNameButton.gameObject.SetActive(false);
    }

    void OnDisplayNameUpdateSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("DisplayName updated successfully!");
    }

    void OnDisplayNameUpdateFailure(PlayFabError error)
    {
        Debug.LogError("Failed to update DisplayName: " + error.GenerateErrorReport());
    }

    void OnUserDataUpdateSuccess(UpdateUserDataResult result)
    {
        Debug.Log("UserData updated successfully!");
    }

    void OnUserDataUpdateFailure(PlayFabError error)
    {
        Debug.LogError("Failed to update UserData: " + error.GenerateErrorReport());
    }
}
