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

            if (PickupMessageUI.Instance != null)
            {
                PickupMessageUI.Instance.ShowMessage("BOLT ha recargado toda su energía");
            }

            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}