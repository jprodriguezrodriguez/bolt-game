using UnityEngine;
using System.Collections;

public class TitanElectrico : MonoBehaviour
{
    [Header("Salud")]
    public float saludMaxima = 120f;  // Un poco más fuerte que el de vapor
    private float saludActual;
    public bool estaMuerto = false;

    [Header("Fases")]
    public float fase2Umbral = 70f;
    public float fase3Umbral = 30f;
    public int faseActual = 1;

    [Header("Punto Débil")]
    public GameObject puntoDebil;  // Núcleo eléctrico en el pecho
    public float multiplicadorPuntoDebil = 4f;  // Más débil al punto crítico

    [Header("Ataques Eléctricos")]
    public float dañoAtaque = 25f;
    public float dañoDescarga = 15f;
    public float dañoRayo = 30f;

    [Header("Recompensa al morir")]
    public GameObject barreraSiguienteNivel;

    [Header("Referencias")]
    public Animator animator;
    private TitanElectricoIA ia;

    void Start()
    {
        saludActual = saludMaxima;
        ia = GetComponent<TitanElectricoIA>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void RecibirDaño(float daño, bool esPuntoDebil)
    {
        if (estaMuerto) return;

        if (esPuntoDebil)
        {
            daño *= multiplicadorPuntoDebil;
            Debug.Log($"⚡ ¡Golpe crítico al núcleo eléctrico! Daño: {daño}");
            StartCoroutine(FeedbackPuntoDebil());
        }

        saludActual -= daño;
        Debug.Log($"💥 Salud del Titán Eléctrico: {saludActual}/{saludMaxima}");

        float porcentajeSalud = (saludActual / saludMaxima) * 100f;

        if (porcentajeSalud <= fase3Umbral && faseActual < 3)
        {
            faseActual = 3;
            Debug.Log("⚡ FASE 3: Modo tormenta - Rayos y descargas");
            if (ia != null)
            {
                ia.velocidad = 6f;
                ia.tiempoEntreAtaques = 0.8f;
            }
        }
        else if (porcentajeSalud <= fase2Umbral && faseActual < 2)
        {
            faseActual = 2;
            Debug.Log("🔌 FASE 2: Ataques eléctricos más frecuentes");
            if (ia != null)
            {
                ia.tiempoEntreAtaques = 1.2f;
            }
        }

        if (saludActual <= 0)
        {
            Morir();
        }
    }

    IEnumerator FeedbackPuntoDebil()
    {
        if (puntoDebil != null)
        {
            Renderer renderer = puntoDebil.GetComponent<Renderer>();
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
        Debug.Log("🏆 ¡TITÁN ELÉCTRICO DERROTADO!");

        if (animator != null)
        {
            animator.SetTrigger("Death1");
        }

        if (ia != null)
            ia.enabled = false;

        if (barreraSiguienteNivel != null)
        {
            barreraSiguienteNivel.SetActive(false);
            Debug.Log("🚪 Puerta a Zona 3 desbloqueada");
        }

        Destroy(gameObject, 2f);
    }
}