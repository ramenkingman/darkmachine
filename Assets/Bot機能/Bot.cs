public class Bot
{
    public string Name { get; private set; }
    private int[] levelCosts;
    private int[] scoresPerHour; // コインをスコアに変更
    public int Level { get; private set; }

    public Bot(string name, int[] levelCosts, int[] scoresPerHour)
    {
        Name = name;
        this.levelCosts = levelCosts;
        this.scoresPerHour = scoresPerHour;
        Level = 0; // 初期レベルを0に設定
    }

    public int GetCurrentCost()
    {
        if (Level < levelCosts.Length)
        {
            return levelCosts[Level];
        }
        return 0;
    }

    public int GetCurrentScorePerHour()
    {
        if (Level == 0)
        {
            return 0; // レベル0の場合は0スコアに設定
        }
        if (Level <= scoresPerHour.Length)
        {
            return scoresPerHour[Level - 1];
        }
        return 0;
    }

    public int GetNextScorePerHour()
    {
        if (Level < scoresPerHour.Length)
        {
            return scoresPerHour[Level];
        }
        return 0;
    }

    public bool CanLevelUp(int currentScore)
    {
        return Level < levelCosts.Length && currentScore >= levelCosts[Level];
    }

    public void LevelUp()
    {
        if (Level < levelCosts.Length)
        {
            Level++;
        }
    }

    public void InitializeLevel()
    {
        Level = 0;
    }

    public int GetMaxLevel()
    {
        return levelCosts.Length;
    }

    public void SetLevel(int level)
    {
        if (level >= 0 && level <= levelCosts.Length)
        {
            Level = level;
        }
    }
}
