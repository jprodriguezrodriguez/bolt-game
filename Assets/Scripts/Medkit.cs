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
            }
        }
    }
}
