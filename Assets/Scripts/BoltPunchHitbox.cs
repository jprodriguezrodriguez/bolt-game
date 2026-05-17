using UnityEngine;

public class BoltPunchHitbox : MonoBehaviour
{
    [Header("Daño del golpe")]
    public float daño = 15f;

    [Header("Estado del golpe")]
    public bool puedeHacerDaño = false;

    private bool yaGolpeoEnEsteAtaque = false;
    private Collider hitboxCollider;

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider>();
        DesactivarDaño();
    }

    private void OnEnable()
    {
        DesactivarDaño();
    }

    private void OnDisable()
    {
        puedeHacerDaño = false;
        yaGolpeoEnEsteAtaque = false;
    }

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
            DesactivarDaño();

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
            DesactivarDaño();

            Debug.Log("BOLT golpeó al Titán Eléctrico correctamente.");
            return;
        }

        TitanDigital titanDigital = other.GetComponent<TitanDigital>();
        if (titanDigital == null)
            titanDigital = other.GetComponentInParent<TitanDigital>();

        if (titanDigital != null)
        {
            titanDigital.RecibirDaño(daño, false);
            yaGolpeoEnEsteAtaque = true;
            DesactivarDaño();

            Debug.Log("BOLT golpeó al Titán Digital correctamente.");
            return;
        }
    }

    public void ActivarDaño()
    {
        puedeHacerDaño = true;
        yaGolpeoEnEsteAtaque = false;

        if (hitboxCollider != null)
            hitboxCollider.enabled = true;

        Debug.Log("Hitbox de BOLT ACTIVADO");
    }

    public void DesactivarDaño()
    {
        puedeHacerDaño = false;
        yaGolpeoEnEsteAtaque = false;

        if (hitboxCollider != null)
            hitboxCollider.enabled = false;

        Debug.Log("Hitbox de BOLT DESACTIVADO");
    }
}