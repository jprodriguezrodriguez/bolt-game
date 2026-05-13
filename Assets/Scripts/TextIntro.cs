using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using System.Collections;

public class ControlIntroCinematica : MonoBehaviour
{
    [Header("Cinemachine")]
    public PlayableDirector director;
    public CinemachineVirtualCamera camCinematica;
    public CinemachineVirtualCamera camJugador;
    public MonoBehaviour scriptMovimiento;

    [Header("Panel Intro")]
    public GameObject panelIntro;       // El PanelIntro completo
    public Image imagenIntro;           // La imagen
    public TextMeshProUGUI textoIntro;  // El texto debajo

    [Header("Configuración")]
    public float tiempoVisible = 4f;    // Segundos que se ve la intro

    void Start()
    {
        if (scriptMovimiento != null)
            scriptMovimiento.enabled = false;

        camCinematica.Priority = 20;
        camJugador.Priority = 10;

        director.stopped += OnCinematicaTerminada;

        StartCoroutine(SecuenciaIntro());
    }

    IEnumerator SecuenciaIntro()
    {
        // Mostrar panel
        panelIntro.SetActive(true);

        // Fade IN
        yield return StartCoroutine(FadePanel(0f, 1f, 1.5f));

        // Esperar mientras el jugador lee
        yield return new WaitForSeconds(tiempoVisible);

        // Fade OUT
        yield return StartCoroutine(FadePanel(1f, 0f, 1.5f));

        // Ocultar panel e iniciar cinemática
        panelIntro.SetActive(false);
        director.Play();
        director.stopped += OnCinematicaTerminada;
    }

    IEnumerator FadePanel(float desde, float hasta, float duracion)
    {
        float tiempo = 0f;

        Color colorImagen = imagenIntro.color;
        Color colorTexto = textoIntro.color;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;

            colorImagen.a = Mathf.Lerp(desde, hasta, t);
            colorTexto.a = Mathf.Lerp(desde, hasta, t);
            imagenIntro.color = colorImagen;
            textoIntro.color = colorTexto;

            yield return null;
        }
    }

    void OnCinematicaTerminada(PlayableDirector pd)
    {
        camCinematica.Priority = 0;
        camJugador.Priority = 20;

        if (scriptMovimiento != null)
            scriptMovimiento.enabled = true;

        director.stopped -= OnCinematicaTerminada;
    }
}