using UnityEngine;

public class FinalMedKit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BoltStats playerStats = other.GetComponent<BoltStats>();

            if (playerStats != null)
            {
                Debug.Log("FinalKit recogido.");
                playerStats.RechargeAll();

                Destroy(transform.parent.gameObject);
            }
        }
    }
}