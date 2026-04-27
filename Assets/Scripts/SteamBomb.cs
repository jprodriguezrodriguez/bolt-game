using UnityEngine;

public class SteamBombEnemy : MonoBehaviour
{
    [Header("Detection")]
    public string playerTag = "Player";
    public float detectionRadius = 6f;
    public float explosionRadius = 2f;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public bool chasePlayer = true;

    [Header("Explosion")]
    public float explosionDelay = 1.5f;
    public int normalDamage = 2;
    public int coveredDamage = 1;

    [Header("References")]
    public Animator animator;

    [Header("Visual")]
    public GameObject explosionEffect;

    private Transform player;
    private bool isExploding = false;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        if (isExploding) return;

        FindPlayerIfNeeded();

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius && chasePlayer)
        {
            MoveTowardPlayer();
            SetWalkAnimation(true);
        }
        else
        {
            SetWalkAnimation(false);
        }

        if (distanceToPlayer <= explosionRadius)
        {
            StartExplosion();
        }
    }

    private void FindPlayerIfNeeded()
    {
        if (player != null) return;

        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void MoveTowardPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.magnitude <= 0.1f) return;

        transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        transform.forward = direction.normalized;
    }

    private void StartExplosion()
    {
        if (isExploding) return;

        isExploding = true;

        SetWalkAnimation(false);

        if (animator != null)
        {
            animator.SetTrigger("attack01");
        }

        Debug.Log("Bomba de vapor activada. Va a explotar.");

        Invoke(nameof(Explode), explosionDelay);
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(playerTag))
            {
                PlayerDefense defense = hit.GetComponent<PlayerDefense>();

                if (defense == null)
                {
                    defense = hit.GetComponentInParent<PlayerDefense>();
                }

                if (defense != null)
                {
                    defense.ApplyExplosionDamage(normalDamage, coveredDamage, transform.position);
                }
            }
        }

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Debug.Log("Bomba de vapor explotó.");

        Destroy(gameObject);
    }

    private void SetWalkAnimation(bool value)
    {
        if (animator != null)
        {
            animator.SetBool("walk", value);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}