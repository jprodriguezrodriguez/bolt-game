using UnityEngine;

public class TitanDigitalIA : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    private Animator animator;
    private TitanDigital titanVida;

    [Header("Rangos")]
    public float rangoDeteccion = 20f;
    public float rangoAtaque = 7f;
    public float rangoPerdida = 25f;

    [Header("Movimiento")]
    public float velocidad = 4f;
    public float velocidadRotacion = 5f;

    [Header("Ataque")]
    public int dañoAtaque = 30;
    public float tiempoEntreAtaques = 1.2f;
    private float tiempoProximoAtaque;

    private BoltStats boltStats;

    void Start()
    {
        GameObject bolt = GameObject.FindGameObjectWithTag("Player");
        if (bolt != null)
        {
            jugador = bolt.transform;
            boltStats = bolt.GetComponent<BoltStats>();
        }
        animator = GetComponent<Animator>();
        titanVida = GetComponent<TitanDigital>();
    }

    void Update()
    {
        if (jugador == null || titanVida.estaMuerto) return;
        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= rangoDeteccion)
        {
            if (distancia <= rangoAtaque) Atacar();
            else Perseguir();
        }
        else Idle();
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
        if (animator != null) animator.SetBool("isWalking", true);
    }

    void Atacar()
    {
        if (Time.time >= tiempoProximoAtaque)
        {
            tiempoProximoAtaque = Time.time + tiempoEntreAtaques;
            if (animator != null)
            {
                animator.SetTrigger("Atacar");
                animator.SetBool("isWalking", false);
            }
            Invoke(nameof(AplicarDaño), 0.3f);
        }
    }

    void AplicarDaño()
    {
        if (boltStats != null && !titanVida.estaMuerto)
            boltStats.TakeDamage(dañoAtaque);
    }

    void Idle()
    {
        if (animator != null) animator.SetBool("isWalking", false);
    }
}