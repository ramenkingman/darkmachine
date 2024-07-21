using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    [System.Serializable]
    public class ButtonPopupPair
    {
        public Button button;
        public GameObject popup;
        public bool autoClose; // 自動で閉じるかどうかを判別するフラグ
        public string popupIdentifier; // ポップアップの識別子を追加
    }

    public ButtonPopupPair[] buttonPopupPairs;
    public float slideInDuration = 0.5f; // スライドインの持続時間

    void Start()
    {
        foreach (var pair in buttonPopupPairs)
        {
            if (pair.button == null || pair.popup == null)
            {
                Debug.LogError("必要なコンポーネントの参照が不足しています。インスペクタで全てのコンポーネントを割り当ててください。");
                return;
            }

            // ポップアップを初期状態で非表示にする
            pair.popup.SetActive(false);

            // ボタンのクリックイベントにリスナーを追加
            pair.button.onClick.AddListener(() => OpenPopup(pair));

            // 自動で閉じない設定の場合、ポップアップ内の閉じるボタンを見つけてリスナーを追加
            if (!pair.autoClose)
            {
                Button closeButton = pair.popup.GetComponentInChildren<Button>();
                if (closeButton != null)
                {
                    closeButton.onClick.AddListener(() => ClosePopup(pair.popup));
                }
            }
        }
    }

    void OpenPopup(ButtonPopupPair pair)
    {
        // 全てのポップアップを非表示にする
        foreach (var p in buttonPopupPairs)
        {
            p.popup.SetActive(false);
        }

        // 対応するポップアップを表示し、スライドインアニメーションを開始
        pair.popup.SetActive(true);
        StartCoroutine(SlideInPopup(pair));

        // 自動で閉じる設定の場合、2秒後に非表示にするコルーチンを開始
        if (pair.autoClose)
        {
            StartCoroutine(HidePopupAfterDelay(pair.popup, 2f));
        }
    }

    IEnumerator SlideInPopup(ButtonPopupPair pair)
    {
        RectTransform rectTransform = pair.popup.GetComponent<RectTransform>();
        Vector2 originalPosition = rectTransform.anchoredPosition;
        Vector2 startPosition;

        // ポップアップの識別子に基づいてスライドインの開始位置を決定
        if (pair.popupIdentifier == "LevelupPopUp1" || pair.popupIdentifier == "LevelupPopUp2")
        {
            startPosition = new Vector2(originalPosition.x, Screen.height);
        }
        else
        {
            startPosition = new Vector2(originalPosition.x, -Screen.height);
        }

        rectTransform.anchoredPosition = startPosition;

        float elapsedTime = 0f;

        while (elapsedTime < slideInDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(
                startPosition,
                originalPosition,
                elapsedTime / slideInDuration
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;
    }

    void ClosePopup(GameObject popup)
    {
        // 指定されたポップアップを非表示にする
        popup.SetActive(false);
    }

    IEnumerator HidePopupAfterDelay(GameObject popup, float delay)
    {
        yield return new WaitForSeconds(delay);
        ClosePopup(popup);
    }
}