using UnityEngine;
using TMPro;
using System.Collections;

public class ElectricDrone : MonoBehaviour
{
    [Header("Detection")]
    public string playerTag = "Player";
    public float attackRange = 8f;

    [Header("Attack")]
    public float chargeTime = 1.2f;
    public float attackCooldown = 2f;
    public int normalDamage = 2;
    public int coveredDamage = 1;

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

    private Transform player;
    private bool isAttacking = false;
    private bool canAttack = true;

    void Start()
    {
        if (lightningLine != null)
        {
            lightningLine.enabled = false;
            lightningLine.positionCount = segments;
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

        if (player == null || isAttacking || !canAttack) return;

        float distance = Vector3.Distance(transform.position, player.position);

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

        Vector3 start = attackOrigin != null ? attackOrigin.position : transform.position;
        Vector3 end = player.position + Vector3.up * 1f;

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
        Collider[] hits = Physics.OverlapSphere(hitPoint, 1f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(playerTag))
            {
                PlayerDefense defense = hit.GetComponent<PlayerDefense>();

                if (defense == null)
                    defense = hit.GetComponentInParent<PlayerDefense>();

                if (defense != null)
                {
                    defense.ApplyExplosionDamage(normalDamage, coveredDamage, transform.position);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}