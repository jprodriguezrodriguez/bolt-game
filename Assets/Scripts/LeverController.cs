using UnityEngine;
using UnityEngine.InputSystem;

public class LeverController : MonoBehaviour
{
    [Header("Door Reference")]
    public DoorController doorController;

    [Header("Interaction")]
    public string playerTag = "Player";

    [Header("UI")]
    public GameObject interactionText;

    [Header("Visual Indicator")]
    public GameObject visualIndicator;

    [Header("Lever Animation")]
    public Transform leverShape;
    public Vector3 activatedRotation = new Vector3(45f, 0f, 0f);
    public float rotationSpeed = 5f;

    private bool playerIsNear = false;
    private bool leverActivated = false;

    private Quaternion initialRotation;
    private Quaternion targetRotation;

    void Start()
    {
        if (interactionText != null)
        {
            interactionText.SetActive(false);
        }

        if (leverShape != null)
        {
            initialRotation = leverShape.localRotation;
            targetRotation = Quaternion.Euler(activatedRotation);
        }
    }

    void Update()
    {
        if (playerIsNear && !leverActivated)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                ActivateLever();
            }
        }

        if (leverActivated && leverShape != null)
        {
            leverShape.localRotation = Quaternion.Lerp(
                leverShape.localRotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }

    private void ActivateLever()
    {
        leverActivated = true;

        if (interactionText != null)
        {
            interactionText.SetActive(false);
        }

        if (doorController != null)
        {
            doorController.OpenDoor();
            Debug.Log("Palanca activada. Puerta abierta.");
        }
        else
        {
            Debug.LogWarning("No se asignó DoorController en la palanca.");
        }

        if (visualIndicator != null)
        {
            visualIndicator.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !leverActivated)
        {
            playerIsNear = true;

            if (interactionText != null)
            {
                interactionText.SetActive(true);
            }

            Debug.Log("Jugador cerca de la palanca. Presiona E.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerIsNear = false;

            if (interactionText != null)
            {
                interactionText.SetActive(false);
            }

            Debug.Log("Jugador se alejó de la palanca.");
        }
    }
}