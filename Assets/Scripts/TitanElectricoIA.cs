using UnityEngine;

public class TitanElectricoIA : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    private Animator animator;
    private TitanElectrico titanVida;

    [Header("Rangos")]
    public float rangoDeteccion = 18f;
    public float rangoAtaque = 6f;
    public float rangoPerdida = 25f;

    [Header("Movimiento")]
    public float velocidad = 3.5f;
    public float velocidadRotacion = 5f;

    [Header("Ataque")]
    public int dañoAtaque = 25;
    public float tiempoEntreAtaques = 1.5f;
    private float tiempoProximoAtaque;

    private BoltStats boltStats;

    void Start()
    {
        GameObject bolt = GameObject.FindGameObjectWithTag("Player");
        if (bolt != null)
        {
            jugador = bolt.transform;
            boltStats = bolt.GetComponent<BoltStats>();
            Debug.Log(boltStats != null ? "✅ BoltStats encontrado" : "❌ BoltStats NO encontrado");
        }

        animator = GetComponent<Animator>();
        titanVida = GetComponent<TitanElectrico>();
    }

    void Update()
    {
        if (jugador == null) return;
        if (titanVida != null && titanVida.estaMuerto) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

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
            Idle();
        }
    }

    void Perseguir()
    {
        Vector3 direccion = (jugador.position - transform.position).normalized;
        direccion.y = 0;
        transform.position += direccion * velocidad * Time.deltaTime;

        if (direccion != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, velocidadRotacion * Time.deltaTime);
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }
    }

    void Atacar()
    {
        if (Time.time >= tiempoProximoAtaque)
        {
            tiempoProximoAtaque = Time.time + tiempoEntreAtaques;

            Debug.Log($"⚡ TITÁN ELÉCTRICO ATACA! Daño: {dañoAtaque}");

            if (animator != null)
            {
                // Usa una animación de ataque disponible
                animator.SetTrigger("Attack2");
                animator.SetBool("isWalking", false);
            }

            Invoke("AplicarDaño", 0.3f);
            Invoke("VolverAIdle", 0.6f);
        }
    }

    void AplicarDaño()
    {
        if (boltStats != null && titanVida != null && !titanVida.estaMuerto)
        {
            boltStats.TakeDamage(dañoAtaque);
            Debug.Log($"💥 Daño eléctrico aplicado: {dañoAtaque}");
        }
    }

    void VolverAIdle()
    {
        if (animator != null && titanVida != null && !titanVida.estaMuerto)
        {
            animator.SetBool("isWalking", false);
        }
    }

    void Idle()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}