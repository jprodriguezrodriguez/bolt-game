using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("References")]
    public ItemsManager itemsManager;

    [Header("Educational Panel Image")]
    public Sprite educationalPanelSprite;

    [Header("Interaction")]
    public string playerTag = "Player";

    [Header("Visual")]
    public GameObject visualObject;

    [Header("UserGuide")]
    public GameObject userGuideParticles;
    public GameObject finalGuidePoint;

    [Header("Checkpoint")]
    public CheckpointManager checkpointManager;
    public Transform checkpointRespawnPoint;
    public bool saveCheckpointOnCollect = true;
    public string collectibleId = "Material_01";

    [Header("Objects to hide when collected")]
    public GameObject[] objectsToHideWhenCollected;

    private bool collected = false;

    private void Start()
    {
        if (checkpointManager == null)
            checkpointManager = FindFirstObjectByType<CheckpointManager>();

        if (checkpointManager != null && checkpointManager.IsItemCollected(collectibleId))
        {
            collected = true;

            if (visualObject != null)
                visualObject.SetActive(false);
            else
                gameObject.SetActive(false);

            if (userGuideParticles != null)
                Destroy(userGuideParticles);

            HideCollectedItemObjects();

            Debug.Log("Material ya estaba recolectado: " + collectibleId);
        }
    }

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

        if (saveCheckpointOnCollect && checkpointManager != null)
        {
            Vector3 checkpointPosition = checkpointRespawnPoint != null
                ? checkpointRespawnPoint.position
                : transform.position;

            checkpointManager.SaveCollectibleCheckpoint(collectibleId, checkpointPosition);
        }

        if (itemsManager != null)
        {
            itemsManager.AddItem(educationalPanelSprite);
        }
        else
        {
            Debug.LogWarning("No se asignó ItemsManager en el ítem.");
        }

        HideCollectedItemObjects();

        Debug.Log("Ítem recolectado: " + gameObject.name);
    }

    private void HideCollectedItemObjects()
    {
        if (visualObject != null)
        {
            visualObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }

        if (userGuideParticles != null)
            userGuideParticles.SetActive(false);

        if (finalGuidePoint != null)
            finalGuidePoint.SetActive(false);

        if (objectsToHideWhenCollected != null)
        {
            foreach (GameObject obj in objectsToHideWhenCollected)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }
}