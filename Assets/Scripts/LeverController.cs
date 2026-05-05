using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LeverController : MonoBehaviour
{
    [Header("Door Reference")]
    public DoorController doorController;

    [Header("Interaction")]
    public string playerTag = "Player";
    public Key interactionKey = Key.E;

    [Header("UI")]
    public GameObject interactionTextContainer;
    public TextMeshProUGUI interactionText;
    public string interactionMessage = "Presiona E para activar la palanca";

    [Header("Visual Indicator")]
    public GameObject visualIndicator;

    [Header("Lever Animation")]
    public Transform leverShape;
    public Vector3 activatedRotation = new Vector3(45f, 0f, 0f);
    public float rotationSpeed = 5f;

    private bool playerIsNear = false;
    private bool leverActivated = false;

    private Quaternion targetRotation;

    void Start()
    {
        HideInteractionText();

        if (leverShape != null)
        {
            targetRotation = Quaternion.Euler(activatedRotation);
        }
    }

    void Update()
    {
        if (playerIsNear && !leverActivated)
        {
            if (Keyboard.current[interactionKey].wasPressedThisFrame)
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

        HideInteractionText();

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
            ShowInteractionText();

            Debug.Log("Jugador cerca de la palanca. Presiona E.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerIsNear = false;
            HideInteractionText();

            Debug.Log("Jugador se alejó de la palanca.");
        }
    }

    private void ShowInteractionText()
    {
        if (interactionText != null)
        {
            interactionText.text = interactionMessage;
        }

        if (interactionTextContainer != null)
        {
            interactionTextContainer.SetActive(true);
        }
    }

    private void HideInteractionText()
    {
        if (interactionTextContainer != null)
        {
            interactionTextContainer.SetActive(false);
        }
    }
}