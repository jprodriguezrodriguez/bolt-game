using UnityEngine;

public class TitanDeVapor : MonoBehaviour
{
    [Header("Salud")]
    public float saludMaxima = 100f;
    private float saludActual;

    [Header("Fases")]
    public float fase2Umbral = 70f;
    public float fase3Umbral = 30f;
    public int faseActual = 1;

    [Header("Ataques")]
    public float dañoGolpe = 20f;
    public float dañoVapor = 15f;
    public float dañoExplosion = 30f;

    [Header("Punto Débil")]
    public GameObject puntoDebil;
    public float multiplicadorPuntoDebil = 3.5f;

    [Header("Referencias")]
    public Animator animator;

    [Header("Portal")]
    public GameObject portalNivel2;

    void Start()
    {
        saludActual = saludMaxima;

        if (portalNivel2 != null)
        {
            portalNivel2.SetActive(false);
        }
    }

    public void RecibirDaño(float daño, bool esPuntoDebil)
    {
        if (esPuntoDebil)
        {
            daño *= multiplicadorPuntoDebil;
            Debug.Log("¡Golpe crítico! Daño: " + daño);
        }

        saludActual -= daño;
        Debug.Log("Salud del Titán: " + saludActual);

        if (saludActual <= saludMaxima * 0.3f && faseActual < 3)
        {
            faseActual = 3;
            Debug.Log("FASE 3: Modo furia");
        }
        else if (saludActual <= saludMaxima * 0.7f && faseActual < 2)
        {
            faseActual = 2;
            Debug.Log("FASE 2: Ataques más rápidos");
        }

        if (saludActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        Debug.Log("Titán de Vapor derrotado");

        if (portalNivel2 != null)
        {
            portalNivel2.SetActive(true);
        }

        Destroy(gameObject);
    }
}