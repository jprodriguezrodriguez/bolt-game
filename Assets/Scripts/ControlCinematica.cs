using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ControlCinematica : MonoBehaviour
{
    [Header("Cinemática")]
    public PlayableDirector director;
    public CinemachineVirtualCamera camCinematica;
    public CinemachineVirtualCamera camJugador;
    public MonoBehaviour scriptMovimiento;

    [Header("UI a ocultar durante la cinemática")]
    public GameObject[] objectsToHideDuringCinematic;

    [Header("UI a mostrar al terminar la cinemática")]
    public GameObject[] objectsToShowAfterCinematic;

    [Header("Objetos a mostrar durante la cinemática")]
    public GameObject[] objectsToShowDuringCinematic;

    [Header("Objetos a ocultar al terminar la cinemática")]
    public GameObject[] objectsToHideAfterCinematic;

    [Header("Misión inicial")]
    public ItemsManager itemsManager;
    public bool showInitialMissionAfterCinematic = true;

    [Header("Delay al terminar")]
    public float delayBeforeReturnToPlayer = 1f;
    public float delayBeforeShowingGameplayUI = 2f;

    [Header("Texto introductorio de cinemática")]
    public GameObject cinematicIntroContainer;
    public TextMeshProUGUI cinematicIntroText;

    [Header("Checkpoint")]
    public CheckpointManager checkpointManager;
    public bool skipCinematicIfCheckpointExists = true;

    [TextArea(3, 6)]
    public string cinematicIntroMessage = "Explora el escenario y descubre los materiales que impulsaron la producción de energía durante la Primera Revolución Industrial.";


    private bool cinematicRunning = false;

    private IEnumerator Start()
    {
        Debug.Log("ControlCinematica inició.");

        if (director == null || camCinematica == null || camJugador == null)
        {
            Debug.LogError("Faltan referencias en ControlCinematica.");
            yield break;
        }

        yield return null;

        if (checkpointManager == null)
            checkpointManager = FindFirstObjectByType<CheckpointManager>();

        if (skipCinematicIfCheckpointExists && checkpointManager != null && checkpointManager.HasCheckpoint())
        {
            Debug.Log("Checkpoint detectado. Se omite la cinemática inicial.");

            SkipCinematicAndStartGameplay();
            yield break;
        }

        PrepareCinematicState();

        director.time = 0;
        director.stopped += OnCinematicaTerminada;

        Debug.Log("Reproduciendo cinemática con: " + camCinematica.name);
        director.Play();
    }

    private void Awake()
    {
        bool hasCheckpoint = HasSavedCheckpointInCurrentScene();

        if (hasCheckpoint && skipCinematicIfCheckpointExists)
        {
            cinematicRunning = false;

            if (cinematicIntroContainer != null)
                cinematicIntroContainer.SetActive(false);

            SetObjectsActive(objectsToShowDuringCinematic, false, "Awake ocultando objetos de cinemática por checkpoint");

            return;
        }

        cinematicRunning = true;

        SetObjectsActive(objectsToHideDuringCinematic, false, "Awake ocultando durante cinemática");
        SetObjectsActive(objectsToShowDuringCinematic, true, "Awake mostrando durante cinemática");

        if (cinematicIntroContainer != null)
            cinematicIntroContainer.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!cinematicRunning)
            return;

        SetObjectsActive(objectsToHideDuringCinematic, false, "Forzando ocultar durante cinemática");
        SetObjectsActive(objectsToShowDuringCinematic, true, "Forzando mostrar durante cinemática");

        if (cinematicIntroContainer != null && !cinematicIntroContainer.activeSelf)
            cinematicIntroContainer.SetActive(true);
    }

    private void SkipCinematicAndStartGameplay()
    {
        cinematicRunning = false;

        if (director != null)
            director.Stop();

        if (camCinematica != null)
            camCinematica.Priority = 0;

        if (camJugador != null)
            camJugador.Priority = 20;

        if (scriptMovimiento != null)
            scriptMovimiento.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SetObjectsActive(objectsToHideDuringCinematic, true, "Restaurando objetos ocultos por cinemática");
        SetObjectsActive(objectsToShowAfterCinematic, true, "Mostrando UI de gameplay");

        if (cinematicIntroContainer != null)
            cinematicIntroContainer.SetActive(false);

        if (itemsManager != null && itemsManager.HasCollectedAllItems())
        {
            Debug.Log("Checkpoint con 3 materiales: no se oculta el Titán.");
            itemsManager.RestoreTitanStateFromProgress();
        }
        else
        {
            SetObjectsActive(objectsToShowDuringCinematic, false, "Ocultando objetos de cinemática");
            SetObjectsActive(objectsToHideAfterCinematic, false, "Ocultando objetos post-cinemática");
        }
    }

    private void PrepareCinematicState()
    {
        Debug.Log("Preparando estado de cinemática.");

        cinematicRunning = true;

        if (scriptMovimiento != null)
            scriptMovimiento.enabled = false;

        camCinematica.Priority = 100;
        camJugador.Priority = 10;

        SetObjectsActive(objectsToHideDuringCinematic, false, "Ocultando durante cinemática");
        SetObjectsActive(objectsToShowDuringCinematic, true, "Mostrando durante cinemática");

        if (cinematicIntroContainer != null)
            cinematicIntroContainer.SetActive(true);

        if (cinematicIntroText != null)
            cinematicIntroText.text = cinematicIntroMessage;
    }

    private void OnCinematicaTerminada(PlayableDirector pd)
    {
        Debug.Log("Cinemática terminada. Esperando antes de volver al jugador.");

        StartCoroutine(ReturnToPlayerAfterDelay());

        director.stopped -= OnCinematicaTerminada;
    }

    private IEnumerator ReturnToPlayerAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeReturnToPlayer);

        Debug.Log("Cambiando de cinemática a cámara del jugador.");

        camCinematica.Priority = 0;
        camJugador.Priority = 20;

        yield return new WaitForSeconds(delayBeforeShowingGameplayUI);
        cinematicRunning = false;

        Debug.Log("Mostrando UI de gameplay después del blend.");

        if (scriptMovimiento != null)
            scriptMovimiento.enabled = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        SetObjectsActive(objectsToShowAfterCinematic, true, "Mostrando después de cinemática");
        SetObjectsActive(objectsToHideAfterCinematic, false, "Ocultando después de cinemática");

        if (showInitialMissionAfterCinematic && itemsManager != null)
        {
            itemsManager.ShowInitialMissionNow();
        }

        if (cinematicIntroContainer != null)
            cinematicIntroContainer.SetActive(false);

    }

    private void SetObjectsActive(GameObject[] objects, bool active, string actionName)
    {
        if (objects == null)
            return;

        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(active);
                Debug.Log(actionName + ": " + obj.name + " = " + active);
            }
        }
    }

    private bool HasSavedCheckpointInCurrentScene()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string checkpointKey = sceneName + "_HasCheckpoint";

        return PlayerPrefs.GetInt(checkpointKey, 0) == 1;
    }
}