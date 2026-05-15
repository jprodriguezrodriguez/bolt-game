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
    private PlayerDefense playerDefense;

    void Start()
    {
        GameObject bolt = GameObject.FindGameObjectWithTag("Player");

        if (bolt != null)
        {
            jugador = bolt.transform;

            boltStats = bolt.GetComponent<BoltStats>();
            playerDefense = bolt.GetComponent<PlayerDefense>();

            if (playerDefense == null)
                playerDefense = bolt.GetComponentInChildren<PlayerDefense>();

            Debug.Log(boltStats != null ? "BoltStats encontrado" : "BoltStats NO encontrado");
            Debug.Log(playerDefense != null ? "PlayerDefense encontrado" : "PlayerDefense NO encontrado");
        }
        animator = GetComponent<Animator>();
        titanVida = GetComponent<TitanDigital>();
    }

    void Update()
    {
        if (jugador == null) return;
        if (titanVida != null && titanVida.estaMuerto) return;
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
        Vector3 direccion = jugador.position - transform.position;
        direccion.y = 0f;
        direccion.Normalize();

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
        if (titanVida != null && titanVida.estaMuerto)
            return;

        if (playerDefense != null)
        {
            playerDefense.ApplyTitanDamage(
                dañoAtaque,
                Mathf.RoundToInt(dañoAtaque * 0.4f),
                transform.position
            );

            Debug.Log($"Daño digital aplicado mediante PlayerDefense: {dañoAtaque}");
        }
        else if (boltStats != null)
        {
            boltStats.TakeDamage(dañoAtaque);

            if (DamageFlashUI.Instance != null)
                DamageFlashUI.Instance.ShowDamageFlash();

            Debug.Log($"Daño digital aplicado directo: {dañoAtaque}");
        }
    }

    void Idle()
    {
        if (animator != null) animator.SetBool("isWalking", false);
    }
}