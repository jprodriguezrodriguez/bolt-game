using UnityEngine;

public class PuntoDebilDigital : MonoBehaviour
{
    private TitanDigital titan;

    void Start()
    {
        // Busca el script TitanDigital en el padre o en el objeto principal
        titan = GetComponentInParent<TitanDigital>();

        if (titan == null)
        {
            Debug.LogWarning("⚠️ PuntoDebilDigital: No se encontró el script TitanDigital en el padre");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Detecta el golpe de BOLT (debe tener tag "PlayerAttack")
        if (other.CompareTag("PlayerAttack"))
        {
            if (titan != null)
            {
                float daño = 15f; // Daño base del golpe básico

                // Verificar si es ataque energético (opcional)
                // Puedes detectar por el nombre del objeto o por un componente
                if (other.name.Contains("Energetico") || Input.GetMouseButton(1))
                {
                    daño = 30f;
                }

                titan.RecibirDaño(daño, true);
                Debug.Log("💾 ¡Impacto en el NÚCLEO DIGITAL! Daño crítico: " + daño);
            }
        }
    }
}