using UnityEngine;
using UnityEngine.InputSystem;

public class LeverInteraction : MonoBehaviour
{
    [SerializeField] private DoorMechanism doorMechanism;
    [SerializeField] private LeverVisual leverVisual;

    private bool playerInRange = false;
    private bool alreadyUsed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Jugador dentro del rango de la palanca");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Jugador salió del rango de la palanca");
        }
    }

    void Update()
    {
        if (!playerInRange || alreadyUsed) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            alreadyUsed = true;

            if (doorMechanism != null)
                doorMechanism.ActivateDoors();

            if (leverVisual != null)
                leverVisual.ActivateLever();

            Debug.Log("Palanca activada");
        }
    }
}