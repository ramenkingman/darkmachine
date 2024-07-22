using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChipImageManager : MonoBehaviour
{
    public Image chipImage; // チップ画像を表示するUIのImageコンポーネント
    public TextMeshProUGUI rankText; // ランク名を表示するTextMeshProUGUI
    private long currentScore; // intからlongに変更

    // チップ画像のパス
    private string[] chipImagePaths = {
        "Chips/WhiteChip",   // 0-5000
        "Chips/RedChip",     // 5000-10K
        "Chips/BlueChip",    // 10K-100K
        "Chips/GreenChip",   // 100K-1M
        "Chips/CyanChip",    // 1M-10M
        "Chips/BlackChip",   // 10M-50M
        "Chips/DarkBlueChip",// 50M-100M
        "Chips/YellowChip",  // 100M-1B
        "Chips/SilverChip",  // 1B-100B
        "Chips/GoldChip"     // 100B以上
    };

    // ランク名
    private string[] rankNames = {
        "White rank",   // 0-5000
        "Red rank",     // 5000-10K
        "Blue rank",    // 10K-100K
        "Green rank",   // 100K-1M
        "Cyan rank",    // 1M-10M
        "Black rank",   // 10M-50M
        "DarkBlue rank",// 50M-100M
        "Yellow rank",  // 100M-1B
        "Silver rank",  // 1B-100B
        "Gold rank"     // 100B以上
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
        if (score < 100000) return chipImagePaths[2];
        if (score < 1000000) return chipImagePaths[3];
        if (score < 10000000) return chipImagePaths[4];
        if (score < 50000000) return chipImagePaths[5];
        if (score < 100000000) return chipImagePaths[6];
        if (score < 1000000000) return chipImagePaths[7];
        if (score < 100000000000L) return chipImagePaths[8]; // Lを追加してlong型であることを明示
        return chipImagePaths[9];
    }

    string GetRankName(long score)
    {
        if (score < 5000) return rankNames[0];
        if (score < 10000) return rankNames[1];
        if (score < 100000) return rankNames[2];
        if (score < 1000000) return rankNames[3];
        if (score < 10000000) return rankNames[4];
        if (score < 50000000) return rankNames[5];
        if (score < 100000000) return rankNames[6];
        if (score < 1000000000) return rankNames[7];
        if (score < 100000000000L) return rankNames[8]; // Lを追加してlong型であることを明示
        return rankNames[9];
    }
}
