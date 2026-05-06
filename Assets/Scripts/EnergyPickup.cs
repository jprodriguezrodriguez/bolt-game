using UnityEngine;

public class EnergyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        BoltStats playerStats = other.GetComponent<BoltStats>();

        if (playerStats == null)
            playerStats = other.GetComponentInParent<BoltStats>();

        if (playerStats != null)
        {
            playerStats.RechargeFullEnergy();
            Destroy(gameObject);
        }
    }
}