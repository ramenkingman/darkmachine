using UnityEngine;
using UnityEngine.UI;

public class OpenTab1OnClick : MonoBehaviour
{
    public Button diamondButton; // ダイヤボタン
    public GameObject tab1; // Tab1のUIオブジェクト

    void Start()
    {
        if (diamondButton == null || tab1 == null)
        {
            Debug.LogError("Missing component references. Please assign all required components in the Inspector.");
            return;
        }

        // Tab1を初期状態で非表示にする
        tab1.SetActive(false);

        // ボタンのクリックイベントにリスナーを追加
        diamondButton.onClick.AddListener(OpenTab1);
    }

    void OpenTab1()
    {
        // Tab1を表示する
        tab1.SetActive(true);
    }
}