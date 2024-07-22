using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScaleObjectOnButtonPress : MonoBehaviour
{
    public Button scaleButton; // スケールを変更するボタン
    public GameObject targetObject; // サイズを変更する対象オブジェクト
    public float scaleMultiplier = 1.2f; // 倍率
    public float scaleDuration = 1f; // 元に戻るまでの時間（秒）

    private Vector3 originalScale; // 元のスケール

    void Start()
    {
        if (scaleButton != null && targetObject != null)
        {
            scaleButton.onClick.AddListener(OnScaleButtonClicked);
            originalScale = targetObject.transform.localScale;
        }
        else
        {
            Debug.LogError("Button or Target Object is not assigned.");
        }
    }

    void OnScaleButtonClicked()
    {
        // 既存のコルーチンを停止
        StopAllCoroutines();

        // スケールを変更
        Vector3 newScale = originalScale * scaleMultiplier;
        targetObject.transform.localScale = newScale;

        // 元に戻すコルーチンを開始
        StartCoroutine(ResetScaleAfterDelay());
    }

    IEnumerator ResetScaleAfterDelay()
    {
        yield return new WaitForSeconds(scaleDuration);

        // スケールを元に戻す
        targetObject.transform.localScale = originalScale;
    }
}
