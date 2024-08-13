using UnityEngine;
using TMPro;
using System.Collections;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance { get; private set; }
    
    [SerializeField] private TextMeshProUGUI energyRecoveredText; // Text Mesh Pro UIの参照

    private int currentEnergy = 2000; 
    private int maxEnergy = 2000; 

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
        StartCoroutine(IncreaseEnergyOverTime());
    }

    public int CurrentEnergy => currentEnergy;
    public int MaxEnergy => maxEnergy;

    public void SetCurrentEnergy(int energy)
    {
        currentEnergy = energy;
        TapController.Instance?.UpdateEnergyText();
        Debug.Log($"SetCurrentEnergy: Energy set to {currentEnergy}");
    }

    public void DecreaseEnergy(int amount)
    {
        currentEnergy -= amount;
        if (currentEnergy < 0)
        {
            currentEnergy = 0;
        }

        TapController.Instance?.UpdateEnergyText();
        Debug.Log($"DecreaseEnergy: Energy decreased to {currentEnergy}");
    }

    public void IncreaseEnergy(int amount)
    {
        currentEnergy += amount;
        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }

        TapController.Instance?.UpdateEnergyText();
        Debug.Log($"IncreaseEnergy: Energy increased to {currentEnergy}");
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
                TapController.Instance?.UpdateEnergyText();
                Debug.Log($"IncreaseEnergyOverTime: Energy increased to {currentEnergy} by {energyIncreaseAmount}");
            }
        }
    }
    
    public void IncreaseEnergyBasedOnTime(int secondsElapsed)
    {
        int energyRecoveryRate = LevelManager.Instance.ScoreIncreaseAmount;
        int energyToAdd = energyRecoveryRate * secondsElapsed;
        DisplayEnergyRecovered(energyToAdd);
        StartCoroutine(DelayedEnergyIncrease(energyToAdd));
    }

    private IEnumerator DelayedEnergyIncrease(int amount)
    {
        yield return new WaitForSeconds(1f); // 1秒待機
        IncreaseEnergy(amount);
    }

    private void DisplayEnergyRecovered(int amount)
    {
        if (energyRecoveredText != null)
        {
            energyRecoveredText.text = $"Energy recovered: {amount}";
        }
    }
}
