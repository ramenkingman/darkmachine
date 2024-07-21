using UnityEngine;

public class SwipeController : MonoBehaviour
{
    public float swipeThreshold = 50f; // スワイプのしきい値（ピクセル単位）
    public float swipeTimeThreshold = 0.5f; // スワイプのしきい値（秒単位）

    private Vector2 startTouchPosition;
    private float startTime;
    private bool isSwiping = false;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // タッチ開始位置と時間を記録
                    startTouchPosition = touch.position;
                    startTime = Time.time;
                    isSwiping = true;
                    break;

                case TouchPhase.Moved:
                    // スワイプを検出する
                    if (isSwiping)
                    {
                        Vector2 endTouchPosition = touch.position;
                        float distance = Vector2.Distance(startTouchPosition, endTouchPosition);
                        float duration = Time.time - startTime;

                        if (distance > swipeThreshold && duration < swipeTimeThreshold)
                        {
                            Vector2 direction = endTouchPosition - startTouchPosition;
                            HandleSwipe(direction);
                            isSwiping = false; // スワイプが検出されたらフラグをリセット
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    // タップ操作を検出
                    if (isSwiping)
                    {
                        Vector2 endTouchPosition = touch.position;
                        float distance = Vector2.Distance(startTouchPosition, endTouchPosition);
                        float duration = Time.time - startTime;

                        if (distance <= swipeThreshold && duration < swipeTimeThreshold)
                        {
                            HandleTap(touch.position);
                        }

                        isSwiping = false;
                    }
                    break;
            }
        }
    }

    private void HandleSwipe(Vector2 direction)
    {
        // スワイプの方向に基づいた処理を実装
        Debug.Log("Swipe detected: " + direction);
    }

    private void HandleTap(Vector2 position)
    {
        // タップ時の処理を実装
        Debug.Log("Tap detected at position: " + position);
    }
}
