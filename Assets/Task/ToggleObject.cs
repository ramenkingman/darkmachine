using UnityEngine;

public class ToggleObjects : MonoBehaviour
{
    // インスペクターで指定するオブジェクト1とオブジェクト2
    public GameObject object1;
    public GameObject object2;

    // ボタンが押されたときに呼び出されるメソッド
    public void Toggle()
    {
        // オブジェクト1をアクティブにする
        if (object1 != null)
        {
            object1.SetActive(true);
        }

        // オブジェクト2を非アクティブにする
        if (object2 != null)
        {
            object2.SetActive(false);
        }
    }
}
