using System.Collections;
using TMPro;
using UnityEngine;

public class DigitalDrone : MonoBehaviour
{
    public enum TipoAtaque
    {
        RayoLuz,
        EsferaPoder
    }

    [Header("Tipo de ataque")]
    public TipoAtaque tipoAtaque = TipoAtaque.RayoLuz;

    [Header("Referencias")]
    public string playerTag = "Player";
    public Transform attackOrigin;
    public LineRenderer lightBeam;
    public GameObject chargeEffect;
    public GameObject impactEffect;
    public GameObject powerSpherePrefab;

    [Header("Rayo de luz")]
    public Transform leftAttackOrigin;
    public Transform rightAttackOrigin;

    public LineRenderer leftLightBeam;
    public LineRenderer rightLightBeam;

    [Header("Movimiento")]
    public float followRange = 14f;
    public float attackRange = 8f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float hoverAmplitude = 0.15f;
    public float hoverFrequency = 2f;

    [Header("Ataque")]
    public float chargeTime = 0.8f;
    public float attackCooldown = 2f;
    public int normalDamage = 10;
    public int coveredDamage = 4;
    public float beamDuration = 0.25f;

    [Header("UI")]
    public GameObject warningTextContainer;
    public TextMeshProUGUI warningText;
    public string warningMessage = "¡Ataque digital!";

    [Header("Target")]
    public Transform targetPoint;

    private Transform jugador;
    private BoltStats boltStats;
    private PlayerDefense playerDefense;

    private bool canAttack = true;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            jugador = player.transform;
            boltStats = player.GetComponent<BoltStats>();
            playerDefense = player.GetComponent<PlayerDefense>();

            if (playerDefense == null)
                playerDefense = player.GetComponentInChildren<PlayerDefense>();
        }

        if (leftLightBeam != null)
        {
            leftLightBeam.enabled = false;
            leftLightBeam.positionCount = 2;
            leftLightBeam.useWorldSpace = true;
        }

        if (rightLightBeam != null)
        {
            rightLightBeam.enabled = false;
            rightLightBeam.positionCount = 2;
            rightLightBeam.useWorldSpace = true;
        }

        if (chargeEffect != null)
            chargeEffect.SetActive(false);

        if (warningTextContainer != null)
            warningTextContainer.SetActive(false);
    }

    private void Update()
    {
        if (jugador == null)
            return;

        HoverEffect();

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= followRange)
        {
            FollowPlayer();
        }

        if (distancia <= attackRange && canAttack)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private void HoverEffect()
    {
        Vector3 pos = transform.position;
        pos.y = initialPosition.y + Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        transform.position = new Vector3(transform.position.x, pos.y, transform.position.z);
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = new Vector3(jugador.position.x, transform.position.y, jugador.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        Vector3 direction = jugador.position - transform.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;

        ShowWarning();

        if (chargeEffect != null)
            chargeEffect.SetActive(true);

        yield return new WaitForSeconds(chargeTime);

        if (chargeEffect != null)
            chargeEffect.SetActive(false);

        if (tipoAtaque == TipoAtaque.RayoLuz)
        {
            yield return StartCoroutine(FireLightBeam());
        }
        else if (tipoAtaque == TipoAtaque.EsferaPoder)
        {
            LaunchPowerSphere();
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private IEnumerator FireLightBeam()
    {
        if (jugador == null)
            yield break;

        Vector3 targetPosition = targetPoint != null
            ? targetPoint.position
            : jugador.position + Vector3.up * 1f;

        Vector3 leftStart = leftAttackOrigin != null
            ? leftAttackOrigin.position
            : transform.position;

        Vector3 rightStart = rightAttackOrigin != null
            ? rightAttackOrigin.position
            : transform.position;

        if (leftLightBeam != null)
        {
            leftLightBeam.useWorldSpace = true;
            leftLightBeam.enabled = true;
            leftLightBeam.positionCount = 2;
            leftLightBeam.SetPosition(0, leftStart);
            leftLightBeam.SetPosition(1, targetPosition);
        }

        if (rightLightBeam != null)
        {
            rightLightBeam.useWorldSpace = true;
            rightLightBeam.enabled = true;
            rightLightBeam.positionCount = 2;
            rightLightBeam.SetPosition(0, rightStart);
            rightLightBeam.SetPosition(1, targetPosition);
        }

        // El daño se aplica SOLO una vez
        ApplyDamage();

        if (impactEffect != null)
        {
            GameObject impact = Instantiate(impactEffect, targetPosition, Quaternion.identity);
            Destroy(impact, 1.5f);
        }

        yield return new WaitForSeconds(beamDuration);

        if (leftLightBeam != null)
            leftLightBeam.enabled = false;

        if (rightLightBeam != null)
            rightLightBeam.enabled = false;
    }

    private void LaunchPowerSphere()
    {
        if (powerSpherePrefab == null || attackOrigin == null || jugador == null)
        {
            Debug.LogWarning("No se puede lanzar esfera: falta prefab, attackOrigin o jugador.");
            return;
        }

        GameObject sphere = Instantiate(powerSpherePrefab, attackOrigin.position, Quaternion.identity);

        DigitalPowerSphere projectile = sphere.GetComponent<DigitalPowerSphere>();

        if (projectile != null)
        {
            Transform sphereTarget = targetPoint != null ? targetPoint : jugador;

            projectile.Initialize(sphereTarget, normalDamage, coveredDamage);
            Debug.Log("Dron lanzó esfera de poder.");
        }
        else
        {
            Debug.LogWarning("El prefab no tiene DigitalPowerSphere.");
        }
    }

    private void ApplyDamage()
    {
        bool damageApplied = false;

        if (playerDefense != null)
        {
            playerDefense.ApplyTitanDamage(normalDamage, coveredDamage, transform.position);
            damageApplied = true;
        }
        else if (boltStats != null)
        {
            boltStats.TakeDamage(normalDamage);
            damageApplied = true;
        }

        if (damageApplied)
        {
            if (DamageFlashUI.Instance != null)
            {
                DamageFlashUI.Instance.ShowDamageFlash();
                Debug.Log("DamageFlash ejecutado desde DigitalDrone.");
            }
            else
            {
                Debug.LogWarning("No existe DamageFlashUI.Instance en la escena.");
            }
        }
    }

    private void ShowWarning()
    {
        if (warningTextContainer != null)
            warningTextContainer.SetActive(true);

        if (warningText != null)
            warningText.text = warningMessage;

        StartCoroutine(HideWarningRoutine());
    }

    private IEnumerator HideWarningRoutine()
    {
        yield return new WaitForSeconds(1f);

        if (warningTextContainer != null)
            warningTextContainer.SetActive(false);
    }
}