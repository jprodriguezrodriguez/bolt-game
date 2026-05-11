using UnityEngine;

public class TitanIA : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    private Animator animator;
    private TitanDeVapor titanVida;

    [Header("Rangos")]
    public float rangoDeteccion = 15f;
    public float rangoAtaque = 2.5f;
    public float rangoPerdida = 20f;

    [Header("Movimiento")]
    public float velocidad = 2.5f;
    public float velocidadRotacion = 5f;

    [Header("Ataque")]
    public int dañoAtaque = 10;
    public float tiempoEntreAtaques = 2f;
    private float tiempoProximoAtaque;

    [Header("Precisión del ataque")]
    public float distanciaRealDeGolpe = 3.5f;
    public float anguloDeGolpe = 270f;

    private BoltStats boltStats;
    private PlayerDefense playerDefense;

    void Start()
    {

        // Buscar jugador
        GameObject bolt = GameObject.FindGameObjectWithTag("Player");
        if (bolt != null)
        {
            jugador = bolt.transform;
            boltStats = bolt.GetComponent<BoltStats>();
            playerDefense = bolt.GetComponent<PlayerDefense>();
        }

        animator = GetComponent<Animator>();
        titanVida = GetComponent<TitanDeVapor>();


    }

    void Update()
    {
        if (jugador == null) return;
        if (titanVida != null && titanVida.estaMuerto) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        // Comportamiento
        if (distancia <= rangoDeteccion)
        {
            if (distancia <= rangoAtaque)
            {
                Atacar();
            }
            else
            {
                Perseguir();
            }
        }
        else
        {
            Detenerse();
        }
    }

    void Perseguir()
    {
        // Movimiento hacia el jugador
        Vector3 direccion = (jugador.position - transform.position).normalized;
        direccion.y = 0;
        transform.position += direccion * velocidad * Time.deltaTime;

        // Rotación hacia el jugador
        if (direccion != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, velocidadRotacion * Time.deltaTime);
        }

        // Animación de caminar/correr
        if (animator != null)
        {
            if (HasParameter("isWalking"))
                animator.SetBool("isWalking", true);
            if (HasParameter("isRunning"))
                animator.SetBool("isRunning", velocidad > 3f);
            if (HasParameter("Speed"))
                animator.SetFloat("Speed", 1f);
        }
    }

    void Atacar()
    {
        if (Time.time >= tiempoProximoAtaque)
        {
            tiempoProximoAtaque = Time.time + tiempoEntreAtaques;

            Debug.Log("TITÁN ATACA! Daño: {dañoAtaque}");

            // Animación de ataque
            if (animator != null)
            {
                if (HasParameter("Atacar"))
                    animator.SetTrigger("Atacar");
                else if (HasParameter("Attack"))
                    animator.SetTrigger("Attack");

                // Detener movimiento durante ataque
                if (HasParameter("isWalking"))
                    animator.SetBool("isWalking", false);
                if (HasParameter("isRunning"))
                    animator.SetBool("isRunning", false);
            }

            // Aplicar daño con delay
            Invoke("AplicarDaño", 0.3f);
            Invoke("VolverAIdle", 0.6f);
        }
    }

    void AplicarDaño()
    {
        if (titanVida != null && titanVida.estaMuerto)
            return;

        if (jugador == null)
            return;

        Vector3 direccionAlJugador = jugador.position - transform.position;
        direccionAlJugador.y = 0f;

        float distancia = direccionAlJugador.magnitude;

        if (distancia > distanciaRealDeGolpe)
        {
            Debug.Log("BOLT esquivó el golpe por distancia.");
            return;
        }

        float angulo = Vector3.Angle(transform.forward, direccionAlJugador.normalized);

        if (angulo > anguloDeGolpe)
        {
            Debug.Log("BOLT esquivó el golpe porque no estaba al frente del Titán.");
            return;
        }

        if (playerDefense != null)
        {
            playerDefense.ApplyTitanDamage(
                dañoAtaque,
                Mathf.RoundToInt(dañoAtaque * 0.4f),
                transform.position
            );

            Debug.Log($"Daño del Titán aplicado a BOLT: {dañoAtaque}");
        }
        else if (boltStats != null)
        {
            boltStats.TakeDamage(dañoAtaque);
            Debug.Log($"Daño aplicado a BOLT sin defensa: {dañoAtaque}");
        }
    }

    void VolverAIdle()
    {
        if (animator != null && titanVida != null && !titanVida.estaMuerto)
        {
            if (HasParameter("isWalking"))
                animator.SetBool("isWalking", false);
            if (HasParameter("isRunning"))
                animator.SetBool("isRunning", false);
            if (HasParameter("Speed"))
                animator.SetFloat("Speed", 0f);
        }
    }

    void Detenerse()
    {
        if (animator != null)
        {
            if (HasParameter("isWalking"))
                animator.SetBool("isWalking", false);
            if (HasParameter("isRunning"))
                animator.SetBool("isRunning", false);
            if (HasParameter("Speed"))
                animator.SetFloat("Speed", 0f);
        }
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