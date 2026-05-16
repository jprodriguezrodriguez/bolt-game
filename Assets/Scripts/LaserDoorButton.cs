using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LaserDoorButton : MonoBehaviour
{
    [Header("References")]
    public LaserDoorLineRenderer laserDoor;
    public GameObject interactionTextContainer;
    public TextMeshProUGUI interactionText;

    [Header("Interaction")]
    public string playerTag = "Player";
    public Key interactionKey = Key.E;
    public string interactionMessage = "Presiona E para desactivar la puerta láser";

    [Header("Button Visual")]
    public Renderer buttonRenderer;
    public Material activeMaterial;
    public Material disabledMaterial;

    [Header("Button Press Animation")]
    public Transform buttonVisualTransform;
    public Vector3 pressedOffset = new Vector3(0f, -0.08f, 0f);
    public float pressDuration = 0.12f;
    public float returnDuration = 0.15f;
    public bool stayPressed = true;

    private Vector3 initialButtonPosition;

    private bool playerInside = false;
    private bool buttonUsed = false;

    private void Start()
    {
        if (interactionTextContainer != null)
            interactionTextContainer.SetActive(false);

        if (buttonVisualTransform != null)
        {
            initialButtonPosition = buttonVisualTransform.localPosition;
        }

        UpdateButtonVisual();
    }

    private void Update()
    {
        if (!playerInside) return;
        if (buttonUsed) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current[interactionKey].wasPressedThisFrame)
        {
            DisableLaserDoor();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (buttonUsed) return;

        if (other.CompareTag(playerTag))
        {
            playerInside = true;

            if (interactionTextContainer != null)
                interactionTextContainer.SetActive(true);

            if (interactionText != null)
                interactionText.text = interactionMessage;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;

            if (interactionTextContainer != null)
                interactionTextContainer.SetActive(false);
        }
    }

    private void DisableLaserDoor()
    {
        buttonUsed = true;

        if (laserDoor != null)
        {
            laserDoor.SetLaserActive(false);
            Debug.Log("Puerta láser desactivada.");
        }
        else
        {
            Debug.LogWarning("No se asignó LaserDoorLineRenderer en el botón.");
        }

        if (interactionTextContainer != null)
            interactionTextContainer.SetActive(false);

        if (buttonVisualTransform != null)
        {
            StartCoroutine(PressButtonRoutine());
        }

        UpdateButtonVisual();
    }

    private void UpdateButtonVisual()
    {
        if (buttonRenderer == null) return;

        if (buttonUsed && disabledMaterial != null)
        {
            buttonRenderer.material = disabledMaterial;
        }
        else if (!buttonUsed && activeMaterial != null)
        {
            buttonRenderer.material = activeMaterial;
        }
    }

    private IEnumerator PressButtonRoutine()
    {
        Vector3 pressedPosition = initialButtonPosition + pressedOffset;

        float elapsed = 0f;

        while (elapsed < pressDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pressDuration;

            buttonVisualTransform.localPosition = Vector3.Lerp(
                initialButtonPosition,
                pressedPosition,
                t
            );

            yield return null;
        }

        buttonVisualTransform.localPosition = pressedPosition;

        if (stayPressed)
            yield break;

        elapsed = 0f;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;

            buttonVisualTransform.localPosition = Vector3.Lerp(
                pressedPosition,
                initialButtonPosition,
                t
            );

            yield return null;
        }

        buttonVisualTransform.localPosition = initialButtonPosition;
    }
}