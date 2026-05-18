using UnityEngine;
using TMPro;
using System.Collections;

public class ElectricDrone : MonoBehaviour
{
    [Header("Detection")]
    public string playerTag = "Player";
    public float attackRange = 8f;

    [Header("Movement")]
    public bool followPlayer = true;
    public float followRange = 15f;
    public float stopDistance = 4f;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public Transform detectionCenter;

    [Header("Attack")]
    public float chargeTime = 1.2f;
    public float attackCooldown = 2f;
    public int normalDamage = 8;
    public int coveredDamage = 3;

    [Header("Lightning Visual")]
    public Transform attackOrigin;
    public LineRenderer lightningLine;
    public GameObject chargeEffect;
    public GameObject impactEffect;

    [Header("Lightning Shape")]
    public int segments = 8;
    public float randomness = 0.35f;
    public float lightningDuration = 0.2f;
    public float refreshRate = 0.03f;

    [Header("UI")]
    public GameObject warningTextContainer;
    public TextMeshProUGUI warningText;
    public string warningMessage = "Presiona Q para cubrirte";

    [Header("Audio")]
    public AudioSource audioSource;       // Componente reproductor de audio
    public AudioClip lightningSound;      // Archivo de audio del rayo

    private Transform player;
    private bool isAttacking = false;
    private bool canAttack = true;

    void Start()
    {
        if (lightningLine != null)
        {
            lightningLine.enabled = false;
            lightningLine.positionCount = segments;
            lightningLine.useWorldSpace = true;
        }

        if (warningTextContainer != null)
        {
            warningTextContainer.SetActive(false);
        }

        if (chargeEffect != null)
        {
            chargeEffect.SetActive(false);
        }

    }

    void Update()
    {
        FindPlayerIfNeeded();

        if (player == null)
            return;

        Vector3 centerPosition = detectionCenter != null ? detectionCenter.position : transform.position;
        float distance = Vector3.Distance(centerPosition, player.position);

        if (!isAttacking && canAttack)
        {
            FollowPlayer(distance);
        }

        if (isAttacking || !canAttack)
            return;

        if (distance <= attackRange)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    void FindPlayerIfNeeded()
    {
        if (player != null) return;

        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        canAttack = false;

        if (warningTextContainer != null)
            warningTextContainer.SetActive(true);

        if (warningText != null)
            warningText.text = warningMessage;

        if (chargeEffect != null)
            chargeEffect.SetActive(true);

        yield return new WaitForSeconds(chargeTime);

        yield return StartCoroutine(FireLightningRoutine());

        if (chargeEffect != null)
            chargeEffect.SetActive(false);

        if (warningTextContainer != null)
            warningTextContainer.SetActive(false);

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        canAttack = true;
    }

    IEnumerator FireLightningRoutine()
    {
        if (player == null || lightningLine == null) yield break;

        // --- REPRODUCCIÓN DEL SONIDO DEL RAYO ---
        if (audioSource != null && lightningSound != null)
        {
            audioSource.PlayOneShot(lightningSound);
        }

        Vector3 start = attackOrigin != null ? attackOrigin.position : transform.position;
        Vector3 end = GetPlayerTargetPoint();

        ApplyDamageToPlayer(end);

        if (impactEffect != null)
        {
            GameObject impactInstance = Instantiate(impactEffect, end, Quaternion.identity);
            Destroy(impactInstance, 1.5f);
        }

        

        lightningLine.enabled = true;

        float elapsed = 0f;

        while (elapsed < lightningDuration)
        {
            GenerateLightning(start, end);
            elapsed += refreshRate;
            yield return new WaitForSeconds(refreshRate);
        }

        lightningLine.enabled = false;

        Debug.Log("Enemigo eléctrico lanzó un rayo.");
    }

    void GenerateLightning(Vector3 start, Vector3 end)
    {
        if (lightningLine == null) return;

        lightningLine.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / (segments - 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            if (i != 0 && i != segments - 1)
            {
                point += new Vector3(
                    Random.Range(-randomness, randomness),
                    Random.Range(-randomness, randomness),
                    Random.Range(-randomness, randomness)
                );
            }

            lightningLine.SetPosition(i, point);
        }
    }

    void ApplyDamageToPlayer(Vector3 hitPoint)
    {
        if (player == null)
            return;

        PlayerDefense defense = player.GetComponent<PlayerDefense>();

        if (defense == null)
            defense = player.GetComponentInParent<PlayerDefense>();

        if (defense == null)
            defense = player.GetComponentInChildren<PlayerDefense>();

        if (defense != null)
        {
            defense.ApplyExplosionDamage(normalDamage, coveredDamage, transform.position);
            Debug.Log("El dron eléctrico aplicó daño a BOLT.");
        }
        else
        {
            Debug.LogWarning("No se encontró PlayerDefense en BOLT.");
        }
    }

    private Vector3 GetPlayerTargetPoint()
    {
        Collider playerCollider = player.GetComponent<Collider>();

        if (playerCollider == null)
            playerCollider = player.GetComponentInChildren<Collider>();

        if (playerCollider != null)
            return playerCollider.bounds.center;

        return player.position + Vector3.up * 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 centerPosition = detectionCenter != null ? detectionCenter.position : transform.position;

        Gizmos.DrawWireSphere(centerPosition, attackRange);
    }

    void FollowPlayer(float distance)
    {
        if (!followPlayer || player == null)
            return;

        if (distance > followRange)
            return;

        if (distance <= stopDistance)
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction == Vector3.zero)
            return;

        transform.position += direction.normalized * moveSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}