using UnityEngine;
using UnityEngine.InputSystem;

public class BreakableBox : MonoBehaviour
{
    [Header("References")]
    public GameObject boxObject;

    [Header("Interaction")]
    public string playerTag = "Player";
    public Key interactionKey = Key.E;

    [Header("UI")]
    public GameObject interactionText;

    [Header("Effects")]
    public GameObject destroyEffect;

    private bool playerIsNear = false;
    private bool isDestroyed = false;

    void Start()
    {
        if (interactionText != null)
        {
            interactionText.SetActive(false);
        }
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

        if (interactionText != null)
        {
            interactionText.SetActive(false);
        }

        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, boxObject.transform.position, Quaternion.identity);
        }

        if (boxObject != null)
        {
            Destroy(boxObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log("Caja destruida.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isDestroyed)
        {
            playerIsNear = true;

            if (interactionText != null)
            {
                interactionText.SetActive(true);
            }

            Debug.Log("Jugador cerca de la caja. Presiona E para destruir.");
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
        }
    }
}