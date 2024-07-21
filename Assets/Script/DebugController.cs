using UnityEngine;

public class DebugController : MonoBehaviour
{
    private void Update()
    {
        // デバッグキーを使用してスコアを設定
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                int score = i * 1000;
                ScoreManager.Instance.SetScore(score);
                Debug.Log($"Score set to {score} for debugging.");
            }
        }

        // 0キーで200000に設定
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ScoreManager.Instance.SetScore(200000);
            Debug.Log("Score set to 200000 for debugging.");
        }
    }
}