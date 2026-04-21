using UnityEngine;
using TMPro;

public class BoltHUD : MonoBehaviour
{
    public BoltStats stats;

    public TMP_Text healthText;
    public TMP_Text energyText;

    void Update()
    {
        if (stats == null) return;

        if (healthText != null)
        {
            healthText.text = "Vida: " + stats.currentHealth + "/" + stats.maxHealth;
        }

        if (energyText != null)
        {
            energyText.text = "Energía: " + stats.GetEnergyUnits() + "/" + (int)stats.maxEnergy;
        }
    }
}