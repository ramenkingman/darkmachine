using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class AutoClickButtonOnSceneLoad : MonoBehaviour
{
    public Button targetButton; // クリックするボタン

    void Start()
    {
        // シーンがロードされたときのイベントにハンドラを登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // シーンがロードされたときのイベントのハンドラを解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // このスクリプトがアタッチされているオブジェクトのシーンかどうか確認
        if (scene == gameObject.scene)
        {
            // 1秒待ってからボタンをクリックするコルーチンを開始
            StartCoroutine(ClickButtonAfterDelay());
        }
    }

    IEnumerator ClickButtonAfterDelay()
    {
        // 1秒待つ
        yield return new WaitForSeconds(3.0f);

        // ボタンを一度クリックする
        targetButton.onClick.Invoke();
    }
}
