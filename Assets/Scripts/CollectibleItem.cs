using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("References")]
    public ItemsManager itemsManager;

    [Header("Item Information")]
    public string itemTitle = "ÍTEM";

    [TextArea(3, 6)]
    public string educationalText = "Texto educativo del ítem.";

    [Header("Educational Panel Image")]
    public Sprite educationalPanelSprite;

    [Header("Interaction")]
    public string playerTag = "Player";

    [Header("Visual")]
    public GameObject visualObject;

    [Header("UserGuide")]
    public GameObject userGuideParticles;
    public GameObject finalGuidePoint;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag(playerTag))
        {
            Collect();
        }
    }

    private void Collect()
    {
        collected = true;

        if (itemsManager != null)
        {
            itemsManager.AddItem(itemTitle, educationalText, educationalPanelSprite);
        }
        else
        {
            Debug.LogWarning("No se asignó ItemsManager en el ítem.");
        }

        if (visualObject != null)
        {
            visualObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }

        if (userGuideParticles != null)
        {
            Destroy(userGuideParticles);
        }

        Debug.Log("Ítem recolectado: " + gameObject.name);
    }
}