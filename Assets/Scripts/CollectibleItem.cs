using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("References")]
    public ItemsManager itemsManager;

    [Header("Item Information")]
    public string itemTitle = "ÍTEM";

    [TextArea(3, 6)]
    public string educationalText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

    [Header("Interaction")]
    public string playerTag = "Player";

    [Header("Visual")]
    public GameObject visualObject;

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
            itemsManager.AddItem(itemTitle, educationalText);
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

        Debug.Log("Ítem recolectado: " + gameObject.name);
    }
}