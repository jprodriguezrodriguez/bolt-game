using UnityEngine;

public class PuntoDebil : MonoBehaviour
{
    private TitanDeVapor titan;

    private void Start()
    {
        titan = GetComponentInParent<TitanDeVapor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        IntentarRecibirGolpe(other);
    }

    private void OnTriggerStay(Collider other)
    {
        IntentarRecibirGolpe(other);
    }

    private void IntentarRecibirGolpe(Collider other)
    {
        BoltPunchHitbox punchHitbox = other.GetComponent<BoltPunchHitbox>();

        if (punchHitbox == null)
            punchHitbox = other.GetComponentInParent<BoltPunchHitbox>();

        if (punchHitbox == null)
            return;

        if (!punchHitbox.puedeHacerDaño)
            return;

        if (titan != null)
        {
            titan.RecibirDaño(punchHitbox.daño, true);
            punchHitbox.DesactivarDaño();

            Debug.Log("BOLT golpeó el punto débil del Titán de Vapor.");
        }
    }
}