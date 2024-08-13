using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;

public class ChangeCustomId : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button changeIdButton;
    public GameObject warningObject; // アクティブ・非アクティブを切り替えるオブジェクト
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

        // 警告オブジェクトを非アクティブに設定
        if (warningObject != null)
        {
            warningObject.SetActive(false);
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

        // テキストの長さが5文字以上か確認
        if (inputText.Length < 5)
        {
            // 警告オブジェクトをアクティブに設定
            if (warningObject != null)
            {
                StartCoroutine(ShowWarning());
            }
            return;
        }

        // PlayFabでCustomIdをリンク
        var linkCustomIdRequest = new LinkCustomIDRequest
        {
            CustomId = inputText,
            ForceLink = true
        };
        PlayFabClientAPI.LinkCustomID(linkCustomIdRequest, OnCustomIdLinkSuccess, OnCustomIdLinkFailure);

        // 入力フィールドのテキストをPlayerPrefsに保存
        PlayerPrefs.SetString("InputFieldText", inputText);

        // カスタムIDをPlayerPrefsに保存して、PlayFabManagerが次回のログイン時に使用するようにする
        PlayerPrefs.SetString("CUSTOM_ID_SAVE_KEY_GAME2", inputText);

        // スコア増加が既に行われたかどうかをチェック
        if (!PlayerPrefs.HasKey("ScoreIncreased"))
        {
            // スコアを1000増加
            IncreaseScore(1000);

            // スコア増加が行われたことをPlayerPrefsに記録
            PlayerPrefs.SetInt("ScoreIncreased", 1);
        }

        // ボタンを非アクティブにする
        changeIdButton.gameObject.SetActive(false);
    }

    IEnumerator ShowWarning()
    {
        // 警告オブジェクトをアクティブにする
        warningObject.SetActive(true);
        // 3秒待つ
        yield return new WaitForSeconds(3);
        // 警告オブジェクトを再び非アクティブにする
        warningObject.SetActive(false);
    }

    void IncreaseScore(int amount)
    {
        // スコア増加の処理を行う
        ScoreManager.Instance.AddScore(amount);
        Debug.Log($"Score increased by {amount}");
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
