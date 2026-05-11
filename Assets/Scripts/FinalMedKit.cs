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

                if (PickupMessageUI.Instance != null)
                {
                    PickupMessageUI.Instance.ShowMessage("BOLT ha recuperado toda su vida y energía");
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
}