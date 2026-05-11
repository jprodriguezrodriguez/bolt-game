using UnityEngine;

public class Medkit : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BoltStats playerStats = other.GetComponent<BoltStats>();

            if (playerStats != null)
            {
                playerStats.HealFull();
                Debug.Log("Medkit recogido. Vida actual: " + playerStats.currentHealth + "/" + playerStats.maxHealth);
                Destroy(gameObject);

                if (PickupMessageUI.Instance != null)
                {
                    PickupMessageUI.Instance.ShowMessage("BOLT ha recuperado toda su vida");
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
