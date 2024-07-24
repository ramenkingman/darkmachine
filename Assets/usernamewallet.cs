using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class ChangeCustomId : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button changeIdButton;
    private string inputText;

    void Start()
    {
        // ボタンにクリックイベントを追加
        changeIdButton.onClick.AddListener(OnChangeIdButtonClicked);

        // 入力フィールドの値をPlayerPrefsからロード
        if (PlayerPrefs.HasKey("InputFieldText"))
        {
            inputField.text = PlayerPrefs.GetString("InputFieldText");
        }
    }

    void OnDestroy()
    {
        // ボタンのクリックイベントを削除
        if (changeIdButton != null)
        {
            changeIdButton.onClick.RemoveListener(OnChangeIdButtonClicked);
        }
    }

    void OnChangeIdButtonClicked()
    {
        // 入力フィールドのテキストを取得
        inputText = inputField.text;

        // PlayFabでCustomIdをリンク
        var linkCustomIdRequest = new LinkCustomIDRequest
        {
            CustomId = inputText,
            ForceLink = true
        };
        PlayFabClientAPI.LinkCustomID(linkCustomIdRequest, OnCustomIdLinkSuccess, OnCustomIdLinkFailure);

        // 入力フィールドのテキストをPlayerPrefsに保存
        PlayerPrefs.SetString("InputFieldText", inputText);

        // ボタンを非アクティブにする
        changeIdButton.gameObject.SetActive(false);
    }

    void OnCustomIdLinkSuccess(LinkCustomIDResult result)
    {
        Debug.Log("CustomId linked successfully!");
    }

    void OnCustomIdLinkFailure(PlayFabError error)
    {
        Debug.LogError("Failed to link CustomId: " + error.GenerateErrorReport());
    }
}
