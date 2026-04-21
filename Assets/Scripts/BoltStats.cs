using UnityEngine;

public class BoltStats : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 4;
    public int currentHealth = 4;

    [Header("Energía")]
    public float maxEnergy = 4f;
    public float currentEnergy = 4f;

    public float energyDrainPerSecond = 1f;
    public float energyRecoveryPerSecond = 0.5f;

    public bool IsRunning { get; set; }

    void Update()
    {
        if (IsRunning && currentEnergy > 0)
        {
            currentEnergy -= energyDrainPerSecond * Time.deltaTime;
        }
        else
        {
            currentEnergy += energyRecoveryPerSecond * Time.deltaTime;
        }

        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void RecoverHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public int GetEnergyUnits()
    {
        return Mathf.CeilToInt(currentEnergy);
    }
}