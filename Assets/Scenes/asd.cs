using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeControllerNew : MonoBehaviour
{
    public RectTransform startRect; // スワイプ開始位置
    public RectTransform endRect;   // スワイプ終了位置
    public float swipeDuration = 0.5f; // スワイプの持続時間
    public float swipeInterval = 5f;   // スワイプの間隔

    void Start()
    {
        // スワイプのシミュレーションを開始
        StartCoroutine(SwipeRoutine());
    }

    private IEnumerator SwipeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(swipeInterval);

            // スワイプの開始
            SimulateSwipe(startRect.anchoredPosition, endRect.anchoredPosition);
        }
    }

    private void SimulateSwipe(Vector2 start, Vector2 end)
    {
        // タッチの開始
        SimulateTouch(start, TouchPhase.Began);

        // スワイプの途中経過
        float elapsedTime = 0;
        while (elapsedTime < swipeDuration)
        {
            elapsedTime += Time.deltaTime;
            Vector2 currentPosition = Vector2.Lerp(start, end, elapsedTime / swipeDuration);
            SimulateTouch(currentPosition, TouchPhase.Moved);
        }

        // タッチの終了
        SimulateTouch(end, TouchPhase.Ended);
    }

    private void SimulateTouch(Vector2 position, TouchPhase phase)
    {
        // タッチ入力のシミュレーションを行うカスタムイベント
        PointerEventData touchData = new PointerEventData(EventSystem.current)
        {
            position = position
        };

        GameObject target = EventSystem.current.currentSelectedGameObject;

        switch (phase)
        {
            case TouchPhase.Began:
                ExecuteEvents.Execute(target, touchData, ExecuteEvents.pointerDownHandler);
                break;
            case TouchPhase.Moved:
                ExecuteEvents.Execute(target, touchData, ExecuteEvents.dragHandler);
                break;
            case TouchPhase.Ended:
                ExecuteEvents.Execute(target, touchData, ExecuteEvents.pointerUpHandler);
                break;
        }
    }
}
