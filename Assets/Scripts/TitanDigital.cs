using UnityEngine;
using System.Collections;

public class TitanDigital : MonoBehaviour
{
    [Header("Salud Digital")]
    public float saludMaxima = 150f; // Un poco más resistente
    private float saludActual;
    public bool estaMuerto = false;

    [Header("Fases")]
    public float fase2Umbral = 70f;
    public float fase3Umbral = 30f;
    public int faseActual = 1;

    [Header("Punto Débil (Núcleo Digital)")]
    public GameObject puntoDebil; // Crea un punto débil, por ejemplo, en su espalda o pecho
    public float multiplicadorPuntoDebil = 4f;

    [Header("Ataques Digitales")]
    public float dañoAtaque = 30f; // Daño alto

    [Header("Recompensa al morir")]
    public GameObject barreraSiguienteNivel; // La puerta hacia el final del juego o el cráter

    [Header("Referencias")]
    public Animator animator;
    private TitanDigitalIA ia;

    void Start()
    {
        saludActual = saludMaxima;
        ia = GetComponent<TitanDigitalIA>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void RecibirDaño(float daño, bool esPuntoDebil)
    {
        if (estaMuerto) return;

        if (esPuntoDebil)
        {
            daño *= multiplicadorPuntoDebil;
            Debug.Log($"💾 ¡GOLPE CRÍTICO AL NÚCLEO DIGITAL! Daño: {daño}");
            StartCoroutine(FeedbackPuntoDebil());
        }

        saludActual -= daño;
        Debug.Log($"💥 Salud del Titán Digital: {saludActual}/{saludMaxima}");

        float porcentajeSalud = (saludActual / saludMaxima) * 100f;

        if (porcentajeSalud <= fase3Umbral && faseActual < 3)
        {
            faseActual = 3;
            Debug.Log("⚡ FASE 3: Modo Overclock - Ataques más rápidos");
            if (ia != null) ia.tiempoEntreAtaques = 0.8f;
        }
        else if (porcentajeSalud <= fase2Umbral && faseActual < 2)
        {
            faseActual = 2;
            Debug.Log("🔌 FASE 2: Optimización - Ataques más frecuentes");
            if (ia != null) ia.tiempoEntreAtaques = 1.2f;
        }

        if (saludActual <= 0) Morir();
    }

    IEnumerator FeedbackPuntoDebil()
    {
        // Cambia el color o emite partículas en el punto débil
        if (puntoDebil != null)
        {
            var renderer = puntoDebil.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color original = renderer.material.color;
                renderer.material.color = Color.cyan;
                yield return new WaitForSeconds(0.2f);
                renderer.material.color = original;
            }
        }
    }

    void Morir()
    {
        estaMuerto = true;
        Debug.Log("🏆 ¡TITÁN DIGITAL DERROTADO!");

        if (animator != null) animator.SetTrigger("Morir");
        if (ia != null) ia.enabled = false;
        if (barreraSiguienteNivel != null) barreraSiguienteNivel.SetActive(false);

        Destroy(gameObject, 2f);
    }
}