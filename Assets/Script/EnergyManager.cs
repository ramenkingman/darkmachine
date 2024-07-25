using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance { get; private set; }

    private int currentEnergy = 2000; // 初期エネルギー
    private int maxEnergy = 2000; // 最大エネルギー
    private bool isEnergyRecovered = false; // オフライン回復の制御フラグ

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
        Debug.Log($"Before SetCurrentEnergy: {currentEnergy}");
        currentEnergy = energy;
        TapController.Instance?.UpdateEnergyText(); // nullチェックを追加
        Debug.Log($"After SetCurrentEnergy: {currentEnergy}");
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
    }

    private IEnumerator IncreaseEnergyOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (!isEnergyRecovered && currentEnergy < maxEnergy)
            {
                int energyIncreaseAmount = LevelManager.Instance.ScoreIncreaseAmount;
                currentEnergy += energyIncreaseAmount;
                if (currentEnergy > maxEnergy)
                {
                    currentEnergy = maxEnergy;
                }
                TapController.Instance?.UpdateEnergyText(); // nullチェックを追加
                Debug.Log($"IncreaseEnergyOverTime: Energy increased to {currentEnergy} by {energyIncreaseAmount}");
            }
        }
    }

    public void MarkEnergyRecovered()
    {
        isEnergyRecovered = true;
    }
}
