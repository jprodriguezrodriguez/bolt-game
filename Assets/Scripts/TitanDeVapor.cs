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
        Debug.Log("Salud del Titán: " + saludActual + "/" + saludMaxima);

        // Cambiar de fase según salud
        float porcentajeSalud = (saludActual / saludMaxima) * 100f;

        if (porcentajeSalud <= fase3Umbral && faseActual < 3)
        {
            faseActual = 3;
            Debug.Log("Modo furia");
            if (ia != null)
            {
                ia.velocidad = 5f;
                ia.tiempoEntreAtaques = 1f;
            }
        }
        else if (porcentajeSalud <= fase2Umbral && faseActual < 2)
        {
            faseActual = 2;
            Debug.Log("Ataques más rápidos");
            if (ia != null)
            {
                ia.tiempoEntreAtaques = 1.5f;
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

        // Animación de muerte
        if (animator != null)
        {
            if (HasParameter("Morir"))
                animator.SetTrigger("Morir");
            else if (HasParameter("Die"))
                animator.SetTrigger("Die");

            // Detener animaciones de movimiento
            if (HasParameter("isWalking"))
                animator.SetBool("isWalking", false);
            if (HasParameter("isRunning"))
                animator.SetBool("isRunning", false);
        }

        // Desactivar IA
        if (ia != null)
            ia.enabled = false;


        // Destruir después de la animación
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
}