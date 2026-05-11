using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class ControlCinematica : MonoBehaviour
{
    [Header("Playable Director")]
    public PlayableDirector director;

    [Header("Camaras Cinemachine")]
    public CinemachineVirtualCamera camaraCinematica;
    public CinemachineVirtualCamera camaraJugador;

    [Header("Control del jugador")]
    public MonoBehaviour scriptMovimientoJugador;

    void Start()
    {
        if (scriptMovimientoJugador != null)
            scriptMovimientoJugador.enabled = false;

        camaraCinematica.Priority = 10;
        camaraJugador.Priority = 0;

        director.stopped += OnCinematicaTerminada;
        director.Play();
    }

    void OnCinematicaTerminada(PlayableDirector pd)
    {
        camaraCinematica.Priority = 0;
        camaraJugador.Priority = 10;

        if (scriptMovimientoJugador != null)
            scriptMovimientoJugador.enabled = true;

        director.stopped -= OnCinematicaTerminada;
    }

    void OnDestroy()
    {
        director.stopped -= OnCinematicaTerminada;
    }
}