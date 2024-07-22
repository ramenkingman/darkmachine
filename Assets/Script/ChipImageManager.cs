using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class ChipImageManager : MonoBehaviour
{
    public Image chipImage; // チップ画像を表示するUIのImageコンポーネント
    public TextMeshProUGUI rankText; // ランク名を表示するTextMeshProUGUI
    private long currentScore; // intからlongに変更

    // 新しいチップ画像のパス
    private string[] chipImagePaths = {
        "Chips/GunrunnerChip",   // 0-5000
        "Chips/RoguelancerChip", // 5000-10K
        "Chips/ExoraiderChip"    // 10K以上
    };

    // 新しいランク名
    private string[] rankNames = {
        "Gunrunner",   // 0-5000
        "Roguelancer", // 5000-10K
        "Exoraider"    // 10K以上
    };

    void Start()
    {
        UpdateChipImage();
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
}