using UnityEngine;

public class PuntoDebil : MonoBehaviour
{
    private TitanDeVapor titan;

    void Start()
    {
        titan = GetComponentInParent<TitanDeVapor>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))  // Asume que el golpe del jugador tiene este tag
        {
            float daño = 10f;  // Daño base del golpe básico
            titan.RecibirDaño(daño, true);  // true = es punto débil
        }
    }
}