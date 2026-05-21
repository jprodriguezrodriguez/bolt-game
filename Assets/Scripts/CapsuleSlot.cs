using UnityEngine;

public class CapsuleSlot : MonoBehaviour
{
    [Header("Configuración")]
    public string requiredTag;
    public Transform snapPoint;

    [Header("Estado")]
    public bool isOccupied = false;

    private GameObject placedObject = null;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (snapPoint == null)
            snapPoint = this.transform;
    }

    public bool TryPlaceObject(GameObject obj)
    {
        if (!isOccupied && obj.CompareTag(requiredTag))
        {
            PlaceObject(obj);
            return true;
        }

        return false;
    }

    private void PlaceObject(GameObject obj)
    {
        isOccupied = true;
        placedObject = obj;

        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        obj.transform.SetParent(this.transform);
        obj.transform.position = snapPoint.position;
        obj.transform.rotation = snapPoint.rotation;

        Debug.Log($"Cápsula '{requiredTag}' colocada correctamente en {gameObject.name}");

        // Registrar logro / evento en la API cuando el material fue ubicado correctamente
        ApiCollectableReporter apiReporter = obj.GetComponent<ApiCollectableReporter>();

        if (apiReporter != null)
        {
            apiReporter.RegistrarRecoleccion();
        }
        else
        {
            Debug.LogWarning($"El objeto {obj.name} no tiene ApiCollectableReporter.");
        }

        gameManager?.CheckWinCondition();
    }

    public GameObject RemoveObject()
    {
        if (!isOccupied) return null;

        isOccupied = false;

        GameObject obj = placedObject;
        placedObject = null;

        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        obj.transform.SetParent(null);

        return obj;
    }
}