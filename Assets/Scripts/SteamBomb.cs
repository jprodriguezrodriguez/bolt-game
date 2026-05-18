using UnityEngine;
using TMPro;
using System.Collections;

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

    [Header("Respawn")]
    public float respawnDelay = 5f;

    [Header("References")]
    public Animator animator;

    [Header("Visual")]
    public GameObject visualObject;
    public Collider bombCollider;
    public GameObject explosionEffect;

    [Header("UI")]
    public GameObject warningTextContainer;
    public TextMeshProUGUI warningText;
    public string warningMessage = "Presiona Q para cubrirte";

    [Header("Audio")]
    public AudioSource audioSource;       // El componente que emite el sonido
    public AudioClip explosionSound;      // El archivo de audio de la explosión

    private Transform player;
    private bool isExploding = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (bombCollider == null)
        {
            bombCollider = GetComponent<Collider>();
        }

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        if (warningTextContainer != null)
        {
            warningTextContainer.SetActive(false);
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

        if (warningTextContainer != null)
        {
            warningTextContainer.SetActive(true);
        }

        if (warningText != null)
        {
            warningText.text = warningMessage;
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

        // --- REPRODUCCIÓN DEL SONIDO ---
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        if (warningTextContainer != null)
        {
            warningTextContainer.SetActive(false);
        }

        Debug.Log("Bomba de vapor explotó.");

        StartCoroutine(RespawnBomb());
    }

    private IEnumerator RespawnBomb()
    {
        Debug.Log("Inicio respawn");

        if (visualObject != null)
        {
            visualObject.SetActive(false);
        }

        if (bombCollider != null)
        {
            bombCollider.enabled = false;
        }

        SetWalkAnimation(false);

        yield return new WaitForSeconds(respawnDelay);

        transform.position = initialPosition;
        transform.rotation = initialRotation;

        if (visualObject != null)
        {
            visualObject.SetActive(true);
        }

        if (bombCollider != null)
        {
            bombCollider.enabled = true;
        }

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        isExploding = false;

        Debug.Log("Bomba de vapor reapareció.");
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