using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public Scrollbar loadingScrollbar;  // Scrollbarを使用
    public TextMeshProUGUI loadingText;  // TextMeshProUGUIを使用
    public string floorSceneName = "Floor"; // Floorシーンの名前

    void Start()
    {
        StartCoroutine(LoadFloorScene());
    }

    private IEnumerator LoadFloorScene()
    {
        yield return new WaitForSeconds(1f); // 少し待ってからロード開始

        // 前のシーン情報を設定
        SceneTransitionManager.PreviousScene = "Loading";
        Debug.Log("Setting Previous Scene to Loading");

        // Floorシーンの読み込みを開始
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(floorSceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            if (loadingScrollbar != null)
            {
                loadingScrollbar.size = progress; // Scrollbarのsizeを更新
            }
            if (loadingText != null)
            {
                loadingText.text = $"Loading... {progress * 100:0}%";
            }

            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
