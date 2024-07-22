using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class ChipImageManager : MonoBehaviour
{
    public Image chipImage;
    public TextMeshProUGUI rankText;
    private long currentScore;

    private string[] chipImagePaths = {
        "Chips/GunrunnerChip",
        "Chips/RoguelancerChip",
        "Chips/ExoraiderChip"
    };

    private string[] rankNames = {
        "Gunrunner",
        "Roguelancer",
        "Exoraider"
    };

    private Vector3 originalScale;
    public float animationDuration = 0.1f;
    public float scaleFactor = 1.2f;

    private Button button;

    void Start()
    {
        originalScale = chipImage.rectTransform.localScale;
        UpdateChipImage();

        button = gameObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnChipClick);
        }
        else
        {
            Debug.LogError("Button component not found on the object.");
        }
    }

    void Update()
    {
        long newScore = ScoreManager.Instance.Score;
        if (newScore != currentScore)
        {
            currentScore = newScore;
            UpdateChipImage();
        }
    }

    void UpdateChipImage()
    {
        string chipImagePath = GetChipImagePath(currentScore);
        Sprite newChipSprite = Resources.Load<Sprite>(chipImagePath);
        string rankName = GetRankName(currentScore);

        if (newChipSprite != null)
        {
            chipImage.sprite = newChipSprite;
        }
        else
        {
            Debug.LogError("Failed to load chip image at path: " + chipImagePath);
        }

        if (rankText != null)
        {
            rankText.text = rankName;
        }
        else
        {
            Debug.LogError("RankTextオブジェクトが設定されていません。インスペクタで割り当ててください。");
        }
    }

    string GetChipImagePath(long score)
    {
        if (score < 5000) return chipImagePaths[0];
        if (score < 10000) return chipImagePaths[1];
        return chipImagePaths[2];
    }

    string GetRankName(long score)
    {
        if (score < 5000) return rankNames[0];
        if (score < 10000) return rankNames[1];
        return rankNames[2];
    }

    private void OnChipClick()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateChip());
        TapController.Instance?.OnPointerDown(null); // スコア加算を呼び出す
    }

    private IEnumerator AnimateChip()
    {
        yield return StartCoroutine(ScaleTo(originalScale * scaleFactor, animationDuration));
        yield return StartCoroutine(ScaleTo(originalScale, animationDuration));
    }

    private IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        Vector3 startScale = chipImage.rectTransform.localScale;
        float time = 0f;

        while (time < duration)
        {
            chipImage.rectTransform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        chipImage.rectTransform.localScale = targetScale;
    }
}
