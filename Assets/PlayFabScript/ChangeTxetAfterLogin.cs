using System.Collections;
using UnityEngine;
using TMPro; // TextMeshProを使用するための名前空間

public class ChangeTextAfterLogin : MonoBehaviour
{
    public TMP_Text textMeshPro; // TextMeshProオブジェクトへの参照

    void Start()
    {
        StartCoroutine(UpdateTextPeriodically());
    }

    private IEnumerator UpdateTextPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f); // 0.5秒ごとに待機

            if (PlayFabManager.Instance != null && PlayFabManager.Instance.IsDataLoaded())
            {
                // ログインしているユーザーのMaster Player Account IDを取得してTextMeshProのテキストを更新
                string masterPlayerAccountID = PlayFabManager.Instance.GetMasterPlayerAccountID();
                textMeshPro.text = masterPlayerAccountID;
            }
            else
            {
                Debug.LogWarning("PlayFabManager.Instance is null or data is not loaded.");
            }
        }
    }
}
