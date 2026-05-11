using UnityEngine;

public class BoltPunchHitbox : MonoBehaviour
{
    [Header("Daño del golpe")]
    public float daño = 15f;

    [Header("Estado del golpe")]
    public bool puedeHacerDaño = false;

    private bool yaGolpeoEnEsteAtaque = false;

    private void OnTriggerEnter(Collider other)
    {
        IntentarGolpear(other);
    }

    private void OnTriggerStay(Collider other)
    {
        IntentarGolpear(other);
    }

    private void IntentarGolpear(Collider other)
    {
        if (!puedeHacerDaño || yaGolpeoEnEsteAtaque)
            return;

        TitanDeVapor titan = other.GetComponent<TitanDeVapor>();

        if (titan == null)
            titan = other.GetComponentInParent<TitanDeVapor>();

        if (titan != null)
        {
            titan.RecibirDaño(daño, false);
            yaGolpeoEnEsteAtaque = true;

            Debug.Log("BOLT golpeó al Titán correctamente.");
        }
    }

    public void ActivarDaño()
    {
        puedeHacerDaño = true;
        yaGolpeoEnEsteAtaque = false;
        Debug.Log("Hitbox de BOLT ACTIVADO");
    }

    public void DesactivarDaño()
    {
        puedeHacerDaño = false;
        Debug.Log("Hitbox de BOLT DESACTIVADO");
    }
}