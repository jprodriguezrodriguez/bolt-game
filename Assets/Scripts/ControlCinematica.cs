using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

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

    private void PrepareCinematicState()
    {
        Debug.Log("Preparando estado de cinemática.");

        if (scriptMovimiento != null)
            scriptMovimiento.enabled = false;

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
        // 1. Se queda un momento mirando al Titán.
        yield return new WaitForSeconds(delayBeforeReturnToPlayer);

        Debug.Log("Cambiando de cinemática a cámara del jugador.");

        // 2. Cambia la cámara, pero todavía NO muestra el HUD.
        camCinematica.Priority = 0;
        camJugador.Priority = 20;

        // 3. Espera a que termine el blend de Cinemachine.
        yield return new WaitForSeconds(delayBeforeShowingGameplayUI);

        Debug.Log("Mostrando UI de gameplay después del blend.");

        // 4. Ahora sí vuelve el control del jugador.
        if (scriptMovimiento != null)
            scriptMovimiento.enabled = true;

        // 5. Ahora sí muestra UI.
        SetObjectsActive(objectsToShowAfterCinematic, true, "Mostrando después de cinemática");

        // 6. Ahora sí oculta el Titán temporal.
        SetObjectsActive(objectsToHideAfterCinematic, false, "Ocultando después de cinemática");

        // 7. Muestra mensaje inicial.
        if (showInitialMissionAfterCinematic && itemsManager != null)
        {
            itemsManager.ShowInitialMissionNow();
        }
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