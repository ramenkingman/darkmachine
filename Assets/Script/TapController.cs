using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class TapController : MonoBehaviour
{
    public static TapController Instance { get; private set; } // インスタンスを追加
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreIncreasePrefab; // スコア増加量表示用のプレハブ
    public TextMeshProUGUI energyText; // エネルギーテキストへの参照
    public Canvas canvas; // 追加: キャンバスの参照
    public AudioClip tapSound; // タップ音のクリップ

    private int scoreToAdd = 0; // バッチ更新用のスコア変数

    private List<AudioSource> audioSources; // オーディオソースのプール

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetScoreText(scoreText);
        }

        StartCoroutine(UpdateScoreTextBatch()); // スコアテキストをバッチ更新するコルーチンを開始

        // オーディオソースのプールを初期化
        audioSources = new List<AudioSource>();
        for (int i = 0; i < 10; i++)
        {
            AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.playOnAwake = false;
            newAudioSource.spatialBlend = 0f; // 2Dサウンド
            newAudioSource.volume = 1.0f;
            audioSources.Add(newAudioSource);
        }

        // エネルギーテキストの初期化
        UpdateEnergyText();
    }

    public void OnChipTap(BaseEventData eventData)
    {
        if (EnergyManager.Instance.CurrentEnergy <= 0)
        {
            Debug.LogWarning("スタミナが不足しています。");
            return;
        }

        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData == null)
        {
            Debug.LogError("OnChipTap called without PointerEventData");
            return;
        }

        int scoreIncrease = LevelManager.Instance.ScoreIncreaseAmount;
        ScoreManager.Instance.AddScore(scoreIncrease); // スコアを即座に更新
        EnergyManager.Instance.DecreaseEnergy(scoreIncrease); // スタミナの減少

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, pointerEventData.position, canvas.worldCamera, out localPoint);
        ShowScoreIncrease("+" + scoreIncrease.ToString(), localPoint); // スコア増加量の表示
        Debug.Log("Score increased at position: " + localPoint); // デバッグログを追加

        PlayTapSound(); // タップ音を再生
    }

    public void UpdateEnergyText()
    {
        if (energyText != null)
        {
            energyText.text = $"{EnergyManager.Instance.CurrentEnergy}/{EnergyManager.Instance.MaxEnergy}";
            Debug.Log($"UpdateEnergyText: Energy text updated to {EnergyManager.Instance.CurrentEnergy}/{EnergyManager.Instance.MaxEnergy}");
        }
        else
        {
            Debug.LogWarning("UpdateEnergyText: energyText is null");
        }
    }

    private void ShowScoreIncrease(string amount, Vector2 localPosition)
    {
        TextMeshProUGUI scoreIncrease = Instantiate(scoreIncreasePrefab, canvas.transform);
        scoreIncrease.rectTransform.anchoredPosition = localPosition;
        scoreIncrease.text = amount;
        scoreIncrease.fontSize = 96; // フォントサイズを倍に設定
        StartCoroutine(FadeOutAndDestroy(scoreIncrease, 0.5f)); // フェードアウトの持続時間を2秒に設定
    }

    private IEnumerator FadeOutAndDestroy(TextMeshProUGUI scoreIncrease, float duration)
    {
        Color originalColor = scoreIncrease.color;
        Vector2 originalPosition = scoreIncrease.rectTransform.anchoredPosition;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            scoreIncrease.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - normalizedTime);
            scoreIncrease.rectTransform.anchoredPosition = originalPosition + new Vector2(0, normalizedTime * 400); // 上に移動する速さを調整
            yield return null;
        }

        Destroy(scoreIncrease.gameObject);
    }

    private IEnumerator UpdateScoreTextBatch()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f); // スコアをバッチ更新する間隔を短くする
            if (scoreToAdd > 0)
            {
                ScoreManager.Instance.AddScore(scoreToAdd);
                scoreToAdd = 0;
            }
        }
    }

    private void PlayTapSound()
    {
        if (tapSound != null)
        {
            // プールから利用可能なAudioSourceを取得
            AudioSource availableSource = audioSources.Find(source => !source.isPlaying);
            if (availableSource != null)
            {
                availableSource.clip = tapSound;
                availableSource.Play();
            }
        }
    }
}
