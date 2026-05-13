using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class ControlCinematica : MonoBehaviour
{
    public PlayableDirector director;
    public CinemachineVirtualCamera camCinematica;
    public CinemachineVirtualCamera camJugador;
    public MonoBehaviour scriptMovimiento;

    void Start()
    {
        if (scriptMovimiento != null)
            scriptMovimiento.enabled = false;

        camCinematica.Priority = 20;
        camJugador.Priority = 10;

        director.stopped += OnCinematicaTerminada;
        director.Play();
    }

    void OnCinematicaTerminada(PlayableDirector pd)
    {
        Debug.Log("✅ Cinemática terminada - volviendo al jugador");

        camCinematica.Priority = 0;
        camJugador.Priority = 20;

        if (scriptMovimiento != null)
            scriptMovimiento.enabled = true;

        director.stopped -= OnCinematicaTerminada;
    }
}