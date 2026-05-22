using UnityEngine;

public class CapsuleSlot : MonoBehaviour
{
    [Header("Configuración")]
    public string requiredTag;
    public Transform snapPoint;

    [Header("Estado")]
    public bool isOccupied = false;

    [Header("API - Intentos Nivel 4")]
    [SerializeField] private bool registrarIntentosApi = true;
    [SerializeField] private int idTemaNivel4 = 4;

    [Header("Puntaje por intentos")]
    [SerializeField] private int puntajeInicial = 100;
    [SerializeField] private int penalizacionPorIntento = 20;
    [SerializeField] private int puntajeMinimo = 0;

    private GameObject placedObject = null;
    private GameManager gameManager;
    private BoltStats boltStats;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        boltStats = FindFirstObjectByType<BoltStats>();

        if (snapPoint == null)
            snapPoint = this.transform;
    }

    public bool TryPlaceObject(GameObject obj)
    {
        if (obj == null) return false;

        bool espacioDisponible = !isOccupied;
        bool materialCorrecto = obj.CompareTag(requiredTag);

        if (!espacioDisponible)
            return false;

        ApiCollectableReporter apiReporter = obj.GetComponent<ApiCollectableReporter>();

        if (apiReporter != null && apiReporter.puntajeAlRecolectar <= 0)
        {
            apiReporter.puntajeAlRecolectar = puntajeInicial;
        }

        if (!materialCorrecto)
        {
            PenalizarIntentoIncorrecto(obj, apiReporter);
            RegistrarIntentoNivel4(obj, false, apiReporter);
            return false;
        }

        RegistrarIntentoNivel4(obj, true, apiReporter);
        PlaceObject(obj);
        return true;
    }

    private void PenalizarIntentoIncorrecto(GameObject obj, ApiCollectableReporter apiReporter)
    {
        if (apiReporter == null)
        {
            Debug.LogWarning($"El objeto {obj.name} no tiene ApiCollectableReporter para aplicar penalización.");
            return;
        }

        int puntajeAnterior = apiReporter.puntajeAlRecolectar;

        apiReporter.puntajeAlRecolectar = Mathf.Max(
            apiReporter.puntajeAlRecolectar - penalizacionPorIntento,
            puntajeMinimo
        );

        Debug.Log($"Intento incorrecto con {obj.name}. Puntaje bajó de {puntajeAnterior} a {apiReporter.puntajeAlRecolectar}");
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

    private void RegistrarIntentoNivel4(GameObject obj, bool fueCorrecto, ApiCollectableReporter apiReporter)
    {
        if (!registrarIntentosApi)
            return;

        if (BoltApiService.Instance == null)
        {
            Debug.LogWarning("BoltApiService no está disponible para registrar intento.");
            return;
        }

        float vidaActual = boltStats != null ? boltStats.currentHealth : 0;
        float energiaActual = boltStats != null ? boltStats.currentEnergy : 0;

        int puntajeActual = apiReporter != null ? apiReporter.puntajeAlRecolectar : 0;

        string tipoEvento = fueCorrecto
            ? "IntentoNivel4Correcto"
            : "IntentoNivel4Incorrecto";

        BoltApiService.Instance.RegistrarEventoPartida(
            idTema: idTemaNivel4,
            idCheckpoint: null,
            tipoEvento: tipoEvento,
            puntajeActual: puntajeActual,
            vidaActual: vidaActual,
            energiaActual: energiaActual
        );

        Debug.Log($"Intento nivel 4 registrado. Objeto: {obj.name}, Correcto: {fueCorrecto}, Puntaje actual: {puntajeActual}");
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