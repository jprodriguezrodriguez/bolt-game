using UnityEngine;

public class TestAtaqueTitan : MonoBehaviour
{
    void Update()
    {
        // Presiona la tecla T para probar ataque manual
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("PRUEBA MANUAL: Buscando BoltStats...");

            GameObject bolt = GameObject.FindGameObjectWithTag("Player");
            if (bolt != null)
            {
                BoltStats stats = bolt.GetComponent<BoltStats>();
                if (stats != null)
                {
                    stats.TakeDamage(10);
                    Debug.Log("ATAQUE MANUAL EXITOSO");
                }
                else
                {
                    Debug.LogError("BoltStats no encontrado en BOLT");
                }
            }
        }
    }
}