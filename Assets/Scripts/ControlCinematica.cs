using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using TMPro;

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

    [TextArea(3, 6)]
    public string cinematicIntroMessage = "BOLT: Evolución tecnológica\n\nPrimera era: Vapor\n\nExplora el escenario y descubre los materiales que impulsaron la producción de energía durante la Primera Revolución Industrial.";

    private bool cinematicRunning = false;

    private IEnumerator Start()
    {
        Debug.Log("ControlCinematica inició.");

        if (director == null)
        {
            Debug.LogError("No se asignó PlayableDirector.");
            yield break;
        }

        if (camCinematica == null)
        {
            Debug.LogError("No se asignó CamCinematica.");
            yield break;
        }

        if (camJugador == null)
        {
            Debug.LogError("No se asignó CamJugador.");
            yield break;
        }

        if (director.playableAsset == null)
        {
            Debug.LogError("El PlayableDirector no tiene Timeline asignada.");
            yield break;
        }

        // Espera un frame para que otros Start(), como ItemsManager, terminen primero.
        yield return null;

        PrepareCinematicState();

        director.time = 0;
        director.stopped += OnCinematicaTerminada;

        Debug.Log("Reproduciendo cinemática con: " + camCinematica.name);
        director.Play();
    }

    private void Awake()
    {
        cinematicRunning = true;

        // Ocultar desde el primer momento posible.
        SetObjectsActive(objectsToHideDuringCinematic, false, "Awake ocultando durante cinemática");
        SetObjectsActive(objectsToShowDuringCinematic, true, "Awake mostrando durante cinemática");

        if (cinematicIntroContainer != null)
            cinematicIntroContainer.SetActive(true);

        if (cinematicIntroText != null)
            cinematicIntroText.text = cinematicIntroMessage;
    }

    private void LateUpdate()
    {
        if (!cinematicRunning)
            return;

        // Fuerza el estado durante la cinemática, por si ItemsManager u otro script activa algo.
        SetObjectsActive(objectsToHideDuringCinematic, false, "Forzando ocultar durante cinemática");
        SetObjectsActive(objectsToShowDuringCinematic, true, "Forzando mostrar durante cinemática");

        if (cinematicIntroContainer != null && !cinematicIntroContainer.activeSelf)
            cinematicIntroContainer.SetActive(true);
    }

    private void PrepareCinematicState()
    {
        Debug.Log("Preparando estado de cinemática.");

        if (scriptMovimiento != null)
            scriptMovimiento.enabled = false;

        if (cinematicIntroContainer != null)
            cinematicIntroContainer.SetActive(true);

        if (cinematicIntroText != null)
            cinematicIntroText.text = cinematicIntroMessage;

        camCinematica.Priority = 100;
        camJugador.Priority = 10;

        SetObjectsActive(objectsToHideDuringCinematic, false, "Ocultando durante cinemática");
        SetObjectsActive(objectsToShowDuringCinematic, true, "Mostrando durante cinemática");
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
}