using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public BoltStats playerStats;
    public ItemsManager itemsManager;

    [Header("Respawn Settings")]
    public Vector3 defaultRespawnPosition;
    public bool useCurrentPlayerPositionAsDefault = true;

    [Header("Level Start Checkpoint")]
    public Transform levelStartPoint;

    private string sceneName;

    private void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
                playerStats = playerObject.GetComponent<BoltStats>();
            }
        }

        if (itemsManager == null)
            itemsManager = FindFirstObjectByType<ItemsManager>();

        if (useCurrentPlayerPositionAsDefault && player != null)
            defaultRespawnPosition = player.position;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("Progreso limpiado con F12.");
        }
    }


    private IEnumerator Start()
    {
        Debug.Log("CheckpointManager inició.");

        yield return null;
        yield return null;

        RestorePlayerFromCheckpoint();
        RestoreItemsCounter();
    }

    public void SaveCheckpoint(Vector3 checkpointPosition)
    {
        PlayerPrefs.SetInt(GetHasCheckpointKey(), 1);

        PlayerPrefs.SetFloat(GetPositionXKey(), checkpointPosition.x);
        PlayerPrefs.SetFloat(GetPositionYKey(), checkpointPosition.y);
        PlayerPrefs.SetFloat(GetPositionZKey(), checkpointPosition.z);

        PlayerPrefs.Save();

        Debug.Log("Escena al guardar checkpoint: " + sceneName);
        Debug.Log("Checkpoint guardado en: " + checkpointPosition);
    }

    public void SaveCollectedItem(string collectibleId)
    {
        if (string.IsNullOrWhiteSpace(collectibleId))
        {
            Debug.LogWarning("No se puede guardar un material sin collectibleId.");
            return;
        }

        if (IsItemCollected(collectibleId))
        {
            Debug.Log("El material ya estaba guardado: " + collectibleId);
            return;
        }

        PlayerPrefs.SetInt(GetCollectedItemKey(collectibleId), 1);

        int collectedCount = GetCollectedItemsCount();
        PlayerPrefs.SetInt(GetCollectedCountKey(), collectedCount + 1);

        PlayerPrefs.Save();

        Debug.Log("Material guardado como recolectado: " + collectibleId);
    }

    public bool IsItemCollected(string collectibleId)
    {
        if (string.IsNullOrWhiteSpace(collectibleId))
            return false;

        return PlayerPrefs.GetInt(GetCollectedItemKey(collectibleId), 0) == 1;
    }

    public int GetCollectedItemsCount()
    {
        return PlayerPrefs.GetInt(GetCollectedCountKey(), 0);
    }

    private void RestoreItemsCounter()
    {
        if (itemsManager == null)
            return;

        int restoredCount = GetCollectedItemsCount();
        itemsManager.RestoreCollectedItems(restoredCount);

        Debug.Log("Contador de materiales restaurado: " + restoredCount);
    }

    public void RestorePlayerFromCheckpoint()
    {
        Debug.Log("Intentando restaurar checkpoint...");
        Debug.Log("Escena al restaurar checkpoint: " + sceneName);

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
                playerStats = playerObject.GetComponent<BoltStats>();
                Debug.Log("Player encontrado al restaurar checkpoint.");
            }
            else
            {
                Debug.LogWarning("No se encontró Player para restaurar checkpoint.");
                return;
            }
        }

        bool hasCheckpoint = PlayerPrefs.GetInt(GetHasCheckpointKey(), 0) == 1;

        Debug.Log("Key checkpoint: " + GetHasCheckpointKey());
        Debug.Log("¿Tiene checkpoint guardado?: " + hasCheckpoint);

        Vector3 respawnPosition = defaultRespawnPosition;

        if (hasCheckpoint)
        {
            respawnPosition = new Vector3(
                PlayerPrefs.GetFloat(GetPositionXKey()),
                PlayerPrefs.GetFloat(GetPositionYKey()),
                PlayerPrefs.GetFloat(GetPositionZKey())
            );
        }
        else if (levelStartPoint != null)
        {
            respawnPosition = levelStartPoint.position;
        }

        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.position = respawnPosition;
            player.position = respawnPosition;
        }
        else
        {
            player.position = respawnPosition;
        }

        if (playerStats != null)
            playerStats.RechargeAll();

        Debug.Log("BOLT reapareció en checkpoint: " + respawnPosition);
    }

    public void ClearLevelProgress()
    {
        PlayerPrefs.DeleteKey(GetHasCheckpointKey());
        PlayerPrefs.DeleteKey(GetPositionXKey());
        PlayerPrefs.DeleteKey(GetPositionYKey());
        PlayerPrefs.DeleteKey(GetPositionZKey());
        PlayerPrefs.DeleteKey(GetCollectedCountKey());

        PlayerPrefs.Save();

        Debug.Log("Progreso del nivel limpiado.");
    }

    public void SaveCollectibleCheckpoint(string collectibleId, Vector3 checkpointPosition)
    {
        SaveCollectedItem(collectibleId);
        SaveCheckpoint(checkpointPosition);

        Debug.Log("Checkpoint completo guardado para: " + collectibleId + " en " + checkpointPosition);
    }

    public bool HasCheckpoint()
    {
        return PlayerPrefs.GetInt(GetHasCheckpointKey(), 0) == 1;
    }

    private string GetHasCheckpointKey()
    {
        return sceneName + "_HasCheckpoint";
    }

    private string GetPositionXKey()
    {
        return sceneName + "_Checkpoint_X";
    }

    private string GetPositionYKey()
    {
        return sceneName + "_Checkpoint_Y";
    }

    private string GetPositionZKey()
    {
        return sceneName + "_Checkpoint_Z";
    }

    private string GetCollectedItemKey(string collectibleId)
    {
        return sceneName + "_Collected_" + collectibleId;
    }

    private string GetCollectedCountKey()
    {
        return sceneName + "_CollectedCount";
    }
}