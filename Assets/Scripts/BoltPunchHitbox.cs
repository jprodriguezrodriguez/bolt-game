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

        TitanDeVapor titanVapor = other.GetComponent<TitanDeVapor>();

        if (titanVapor == null)
            titanVapor = other.GetComponentInParent<TitanDeVapor>();

        if (titanVapor != null)
        {
            titanVapor.RecibirDaño(daño, false);
            yaGolpeoEnEsteAtaque = true;

            Debug.Log("BOLT golpeó al Titán de Vapor correctamente.");
            return;
        }

        TitanElectrico titanElectrico = other.GetComponent<TitanElectrico>();

        if (titanElectrico == null)
            titanElectrico = other.GetComponentInParent<TitanElectrico>();

        if (titanElectrico != null)
        {
            titanElectrico.RecibirDaño(daño, false);
            yaGolpeoEnEsteAtaque = true;

            Debug.Log("BOLT golpeó al Titán Eléctrico correctamente.");
            return;
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