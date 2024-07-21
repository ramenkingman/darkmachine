using System.Collections;
using UnityEngine;

public class DeactivateOnXFollow : MonoBehaviour
{
    void Start()
    {
        // PlayFabManagerがデータをロードするまで待機するコルーチンを開始
        StartCoroutine(WaitForDataLoad());
    }

    private IEnumerator WaitForDataLoad()
    {
        // データがロードされるのを待機
        while (!PlayFabManager.Instance.IsDataLoaded())
        {
            yield return null;
        }

        // xFollowの値をチェック
        CheckXFollowValue();
    }

    private void CheckXFollowValue()
    {
        int xFollow = PlayFabManager.Instance.XFollowToSave;

        // xFollowの値が101以上であればオブジェクトを非アクティブにする
        if (xFollow >= 101)
        {
            gameObject.SetActive(false);
        }
    }
}
