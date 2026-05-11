using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class BreakableBox : MonoBehaviour
{
    [Header("References")]
    public GameObject boxObject;

    [Header("Interaction")]
    public string playerTag = "Player";
    public Key interactionKey = Key.E;

    [Header("UI")]
    public GameObject interactionTextContainer;
    public TextMeshProUGUI interactionText;
    public string interactionMessage = "Presiona E para romper la caja";

    [Header("Effects")]
    public GameObject destroyEffect;

    [Header("UserGuide")]
    public GameObject userGuideParticles;
    public GameObject finalGuidePoint;

    private bool playerIsNear = false;
    private bool isDestroyed = false;

    void Start()
    {
        HideInteractionText();
    }

    void Update()
    {
        if (playerIsNear && !isDestroyed)
        {
            if (Keyboard.current[interactionKey].wasPressedThisFrame)
            {
                DestroyBox();
            }
        }
    }

    private void DestroyBox()
    {
        isDestroyed = true;

        HideInteractionText();

        if (destroyEffect != null && boxObject != null)
        {
            Instantiate(destroyEffect, boxObject.transform.position, Quaternion.identity);
        }

        if (boxObject != null)
        {
            Debug.Log(boxObject.name + " destruida.");
            Destroy(boxObject);
        }
        else
        {
            Debug.Log(gameObject.name + " destruida.");
            Destroy(gameObject);
        }

        if (userGuideParticles != null)
        {
            Destroy(userGuideParticles);
        }

        if (finalGuidePoint != null)
        {
            Destroy(finalGuidePoint);
        }

        Debug.Log("Caja destruida.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isDestroyed)
        {
            playerIsNear = true;
            ShowInteractionText();

            Debug.Log("Jugador cerca de la caja.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerIsNear = false;
            HideInteractionText();
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