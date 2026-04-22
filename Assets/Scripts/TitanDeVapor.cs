using UnityEngine;

public class TitanDeVapor : MonoBehaviour
{
    // === PARÁMETROS QUE TÚ DEFINES ===
    [Header("Salud")]
    public float saludMaxima = 100f;
    private float saludActual;

    [Header("Fases")]
    public float fase2Umbral = 70f;  // Cambia a fase 2 al 70% salud
    public float fase3Umbral = 30f;  // Cambia a fase 3 al 30% salud
    public int faseActual = 1;

    [Header("Ataques")]
    public float dañoGolpe = 20f;
    public float dañoVapor = 15f;
    public float dañoExplosion = 30f;

    [Header("Punto Débil")]
    public GameObject puntoDebil;
    public float multiplicadorPuntoDebil = 3.5f;  // 35 de daño vs 10 normal

    [Header("Referencias")]
    public Animator animator;

    void Start()
    {
        saludActual = saludMaxima;
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

        // Cambiar de fase según salud
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
        Destroy(gameObject);
    }
}