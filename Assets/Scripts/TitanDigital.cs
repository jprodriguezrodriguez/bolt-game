using UnityEngine;
using System.Collections;

public class TitanDigital : MonoBehaviour
{
    [Header("Salud Digital")]
    public float saludMaxima = 150f;
    private float saludActual;
    public bool estaMuerto = false;

    [Header("Fases")]
    public float fase2Umbral = 70f;
    public float fase3Umbral = 30f;
    public int faseActual = 1;

    [Header("Punto Débil - Núcleo Digital")]
    public GameObject puntoDebil;
    public float multiplicadorPuntoDebil = 4f;

    [Header("Ataques Digitales")]
    public float dañoAtaque = 30f;

    [Header("Recompensa al morir")]
    public GameObject barreraSiguienteNivel;
    public GameObject portalSiguienteNivel;

    [Header("Referencias")]
    public Animator animator;
    private TitanDigitalIA ia;

    void Start()
    {
        saludActual = saludMaxima;
        estaMuerto = false;  // ← Asegurar que no está muerto
        ia = GetComponent<TitanDigitalIA>();

        if (animator == null)
            animator = GetComponent<Animator>();

        // Forzar animación Idle al inicio
        if (animator != null)
        {
            animator.Play("Idle");
        }

        Debug.Log($"Titán Digital iniciado con {saludActual} de salud");
    }

    public void RecibirDaño(float daño, bool esPuntoDebil)
    {
        if (estaMuerto)
            return;

        if (esPuntoDebil)
        {
            daño *= multiplicadorPuntoDebil;
            Debug.Log($"💾 ¡GOLPE CRÍTICO AL NÚCLEO DIGITAL! Daño: {daño}");
            StartCoroutine(FeedbackPuntoDebil());
        }

        saludActual -= daño;
        saludActual = Mathf.Clamp(saludActual, 0, saludMaxima);

        Debug.Log($"💥 Salud del Titán Digital: {saludActual}/{saludMaxima}");

        float porcentajeSalud = (saludActual / saludMaxima) * 100f;

        if (porcentajeSalud <= fase3Umbral && faseActual < 3)
        {
            faseActual = 3;
            Debug.Log("⚡ FASE 3: Modo Overclock - Ataques más rápidos");

            if (ia != null)
            {
                ia.tiempoEntreAtaques = 0.8f;
            }
        }
        else if (porcentajeSalud <= fase2Umbral && faseActual < 2)
        {
            faseActual = 2;
            Debug.Log("🔌 FASE 2: Optimización - Ataques más frecuentes");

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

    private IEnumerator FeedbackPuntoDebil()
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

    private void Morir()
    {
        estaMuerto = true;
        Debug.Log("🏆 ¡TITÁN DIGITAL DERROTADO!");

        if (animator != null)
        {
            if (HasParameter("Morir"))
                animator.SetTrigger("Morir");
            else if (HasParameter("Death"))
                animator.SetTrigger("Death");
            else if (HasParameter("Die"))
                animator.SetTrigger("Die");
        }

        if (ia != null)
            ia.enabled = false;

        if (barreraSiguienteNivel != null)
        {
            barreraSiguienteNivel.SetActive(false);
            Debug.Log("🚪 Barrera del siguiente nivel desactivada.");
        }

        if (portalSiguienteNivel != null)
        {
            portalSiguienteNivel.SetActive(true);
            Debug.Log("🌀 Portal final activado.");
        }

        if (PickupMessageUI.Instance != null)
        {
            PickupMessageUI.Instance.ShowMessage("El Titán Digital ha sido derrotado");
        }

        PlayerPrefs.DeleteKey("RetryWithDigitalTitanUnlocked");
        PlayerPrefs.Save();

        Destroy(gameObject, 2f);
    }

    public float ObtenerSaludActual()
    {
        return saludActual;
    }

    public float ObtenerSaludMaxima()
    {
        return saludMaxima;
    }

    public float ObtenerPorcentajeSalud()
    {
        return saludActual / saludMaxima;
    }

    private bool HasParameter(string paramName)
    {
        if (animator == null)
            return false;

        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }

        return false;
    }
}