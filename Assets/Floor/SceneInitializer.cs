using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneInitializer : MonoBehaviour
{
    public Image splashImage;  // 特定のイメージを表示するためのImageコンポーネント

    void Start()
    {
        Debug.Log("Previous Scene: " + SceneTransitionManager.PreviousScene);

        // Loadingシーンから遷移してきた場合はイメージを表示
        if (SceneTransitionManager.PreviousScene == "Loading")
        {
            StartCoroutine(ShowSplashImage());
        }
        else
        {
            splashImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator ShowSplashImage()
    {
        // イメージを表示
        splashImage.gameObject.SetActive(true);

        // 1秒間待機
        yield return new WaitForSeconds(1f);

        // イメージを非表示
        splashImage.gameObject.SetActive(false);

        // スプラッシュイメージを表示後、PreviousSceneをリセット
        SceneTransitionManager.PreviousScene = null;
    }
}
