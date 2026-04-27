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

    [Header("HUD")]
    [SerializeField] private HealthBarUI healthBarUI;
    [SerializeField] private HealthBarUI energyBarUI;

    public bool IsRunning { get; set; }

    void Start()
    {
        ActualizarHUD();
    }

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

        ActualizarHUD();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Vida actual: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("BOLT quedó sin vida.");
        }
    }

    public void UseEnergy(int amount)
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        Debug.Log("Energía actual: " + currentEnergy + "/" + maxEnergy);
    }
    public bool HasEnergy(int amount)
    {
        return currentEnergy >= amount;
    }

    public void RecoverHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        ActualizarHUD();
    }

    private void ActualizarHUD()
    {
        if (healthBarUI != null)
            healthBarUI.ActualizarBarra(currentHealth, maxHealth);

        if (energyBarUI != null)
            energyBarUI.ActualizarBarra(currentEnergy, maxEnergy);
    }

    public int GetEnergyUnits()
    {
        return Mathf.CeilToInt(currentEnergy);
    }
}