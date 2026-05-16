using UnityEngine;

public class LaserDoorDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int healthDamage = 10;
    public float energyDamage = 1f;
    public float damageCooldown = 1f;

    [Header("Player")]
    public string playerTag = "Player";

    private float lastDamageTime = -999f;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (Time.time < lastDamageTime + damageCooldown)
            return;

        BoltStats stats = other.GetComponent<BoltStats>();

        if (stats == null)
            stats = other.GetComponentInParent<BoltStats>();

        if (stats != null)
        {
            // Daño a la vida
            stats.TakeDamage(healthDamage);

            // Daño a la energía
            stats.currentEnergy = Mathf.Max(0f, stats.currentEnergy - energyDamage);

            lastDamageTime = Time.time;

            if (DamageFlashUI.Instance != null)
                DamageFlashUI.Instance.ShowDamageFlash();

            Debug.Log("BOLT recibió daño por láser. Vida -" + healthDamage + " | Energía -" + energyDamage);
        }
    }
}