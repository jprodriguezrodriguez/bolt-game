using UnityEngine;
using System.Collections;

public class TitanDeVapor : MonoBehaviour
{
    [Header("Salud")]
    public float saludMaxima = 100f;
    private float saludActual;
    public bool estaMuerto = false;

    [Header("Fases")]
    public float fase2Umbral = 70f;
    public float fase3Umbral = 30f;
    public int faseActual = 1;

    [Header("Punto Débil")]
    public GameObject puntoDebil;
    public float multiplicadorPuntoDebil = 3.5f;

    [Header("Recompensa al morir")]
    public GameObject PortalSiguienteNivel;

    [Header("Referencias")]
    public Animator animator;
    private TitanIA ia;

    void Start()
    {
        saludActual = saludMaxima;
        ia = GetComponent<TitanIA>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void RecibirDaño(float daño, bool esPuntoDebil)
    {
        if (estaMuerto) return;

        if (esPuntoDebil)
        {
            daño *= multiplicadorPuntoDebil;
            Debug.Log("¡Golpe crítico! Daño: " + daño);
            StartCoroutine(FeedbackPuntoDebil());
        }

        saludActual -= daño;
        saludActual = Mathf.Clamp(saludActual, 0, saludMaxima);

        Debug.Log("Salud del Titán: " + saludActual + "/" + saludMaxima);

        // Cambiar de fase según salud
        float porcentajeSalud = (saludActual / saludMaxima) * 100f;

        if (porcentajeSalud <= fase3Umbral && faseActual < 3)
        {
            faseActual = 3;
            Debug.Log("Modo furia");
            if (ia != null)
            {
                ia.velocidad = 3.5f;
                ia.tiempoEntreAtaques = 1.5f;
            }
        }
        else if (porcentajeSalud <= fase2Umbral && faseActual < 2)
        {
            faseActual = 2;
            Debug.Log("Ataques más rápidos");
            if (ia != null)
            {
                ia.tiempoEntreAtaques = 2f;
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
                Color colorOriginal = renderer.material.color;
                renderer.material.color = Color.red;
                yield return new WaitForSeconds(0.2f);
                renderer.material.color = colorOriginal;
            }
        }
    }

    void Morir()
    {
        estaMuerto = true;
        Debug.Log("TITÁN DE VAPOR DERROTADO");

        // Detener animaciones de movimiento
        if (animator != null)
        {
            if (HasParameter("isWalking"))
                animator.SetBool("isWalking", false);

            if (HasParameter("isRunning"))
                animator.SetBool("isRunning", false);

            if (HasParameter("Speed"))
                animator.SetFloat("Speed", 0f);
        }

        // Desactivar IA
        if (ia != null)
            ia.enabled = false;

        // Activar portal al siguiente nivel
        if (PortalSiguienteNivel != null)
        {
            PortalSiguienteNivel.SetActive(true);
            Debug.Log("Portal al siguiente nivel activado");
        }

        // Mensaje de victoria
        if (PickupMessageUI.Instance != null)
        {
            PickupMessageUI.Instance.ShowMessage("El Titán de Vapor ha sido derrotado");
        }

        // Desaparece el Titán
        Destroy(gameObject, 1.5f);
    }

    // Función para verificar si un parámetro existe en el Animator
    private bool HasParameter(string paramName)
    {
        if (animator == null) return false;
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
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
}