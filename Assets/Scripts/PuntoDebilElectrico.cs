using UnityEngine;

public class PuntoDebilElectrico : MonoBehaviour
{
    private TitanElectrico titan;

    void Start()
    {
        titan = GetComponentInParent<TitanElectrico>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            if (titan != null)
            {
                float daño = 15f;
                titan.RecibirDaño(daño, true);
                Debug.Log("⚡ ¡Impacto en el núcleo eléctrico!");
            }
        }
    }
}