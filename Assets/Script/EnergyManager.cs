using UnityEngine;
using System.Collections;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance { get; private set; }

    private int currentEnergy = 2000; // 初期エネルギー
    private int maxEnergy = 2000; // 最大エネルギー

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("EnergyManager Start: Starting IncreaseEnergyOverTime coroutine");
        StartCoroutine(IncreaseEnergyOverTime()); // コルーチンを開始
    }

    public int CurrentEnergy => currentEnergy;
    public int MaxEnergy => maxEnergy;

    public void SetCurrentEnergy(int energy)
    {
        currentEnergy = energy;
        TapController.Instance?.UpdateEnergyText(); // nullチェックを追加
        Debug.Log($"SetCurrentEnergy: Energy set to {currentEnergy}");

        // スタミナが変更されたことを通知
        PlayFabManager.Instance.SavePlayerData(ScoreManager.Instance.Score, LevelManager.Instance.PlayerLevel, PlayFabManager.Instance.XFollowToSave, PlayFabManager.Instance.InvitationToSave, BotManager.Instance.GetBotLevels(), currentEnergy);
    }

    public void DecreaseEnergy(int amount)
    {
        currentEnergy -= amount;
        if (currentEnergy < 0)
        {
            currentEnergy = 0;
        }

        TapController.Instance?.UpdateEnergyText(); // nullチェックを追加
        Debug.Log($"DecreaseEnergy: Energy decreased to {currentEnergy}");

        // スタミナが変更されたことを通知
        PlayFabManager.Instance.SavePlayerData(ScoreManager.Instance.Score, LevelManager.Instance.PlayerLevel, PlayFabManager.Instance.XFollowToSave, PlayFabManager.Instance.InvitationToSave, BotManager.Instance.GetBotLevels(), currentEnergy);
    }

    public void IncreaseEnergy(int amount)
    {
        currentEnergy += amount;
        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }

        TapController.Instance?.UpdateEnergyText(); // nullチェックを追加
        Debug.Log($"IncreaseEnergy: Energy increased to {currentEnergy}");

        // スタミナが変更されたことを通知
        PlayFabManager.Instance.SavePlayerData(ScoreManager.Instance.Score, LevelManager.Instance.PlayerLevel, PlayFabManager.Instance.XFollowToSave, PlayFabManager.Instance.InvitationToSave, BotManager.Instance.GetBotLevels(), currentEnergy);
    }

    private IEnumerator IncreaseEnergyOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (currentEnergy < maxEnergy)
            {
                int energyIncreaseAmount = LevelManager.Instance.ScoreIncreaseAmount;
                currentEnergy += energyIncreaseAmount;
                if (currentEnergy > maxEnergy)
                {
                    currentEnergy = maxEnergy;
                }
                TapController.Instance?.UpdateEnergyText(); // nullチェックを追加
                Debug.Log($"IncreaseEnergyOverTime: Energy increased to {currentEnergy} by {energyIncreaseAmount}");

                // スタミナが変更されたことを通知
                PlayFabManager.Instance.SavePlayerData(ScoreManager.Instance.Score, LevelManager.Instance.PlayerLevel, PlayFabManager.Instance.XFollowToSave, PlayFabManager.Instance.InvitationToSave, BotManager.Instance.GetBotLevels(), currentEnergy);
            }
        }
    }
}
