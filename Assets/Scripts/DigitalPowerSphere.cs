using UnityEngine;

public class DigitalPowerSphere : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float lifeTime = 5f;

    [Header("Impacto")]
    public float hitDistance = 0.45f;
    public GameObject impactEffect;

    private Transform target;
    private PlayerDefense playerDefense;
    private BoltStats boltStats;
    private Collider playerCollider;

    private int normalDamage;
    private int coveredDamage;
    private bool hasHit = false;

    public void Initialize(Transform newTarget, int damage, int covered)
    {
        target = newTarget;
        normalDamage = damage;
        coveredDamage = covered;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerDefense = playerObject.GetComponent<PlayerDefense>();

            if (playerDefense == null)
                playerDefense = playerObject.GetComponentInChildren<PlayerDefense>();

            boltStats = playerObject.GetComponent<BoltStats>();

            playerCollider = playerObject.GetComponent<Collider>();

            if (playerCollider == null)
                playerCollider = playerObject.GetComponentInChildren<Collider>();
        }

        Debug.Log("Esfera digital inicializada contra: " + target.name);

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (target == null || hasHit)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        transform.Rotate(Vector3.up, 180f * Time.deltaTime);

        CheckDistanceImpact();
    }

    private void CheckDistanceImpact()
    {
        if (playerCollider == null)
            return;

        Vector3 closestPoint = playerCollider.ClosestPoint(transform.position);
        float distance = Vector3.Distance(transform.position, closestPoint);

        if (distance <= hitDistance)
        {
            HitPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;

        if (other.CompareTag("Player") || other.GetComponentInParent<PlayerDefense>() != null)
        {
            HitPlayer();
        }
    }

    private void HitPlayer()
    {
        if (hasHit)
            return;

        hasHit = true;

        if (playerDefense != null)
        {
            playerDefense.ApplyExplosionDamage(normalDamage, coveredDamage, transform.position);
            Debug.Log("La esfera digital golpeó a BOLT.");
        }
        else if (boltStats != null)
        {
            boltStats.TakeDamage(normalDamage);

            if (DamageFlashUI.Instance != null)
                DamageFlashUI.Instance.ShowDamageFlash();

            Debug.Log("La esfera digital hizo daño directo a BOLT.");
        }
        else
        {
            Debug.LogWarning("La esfera no encontró PlayerDefense ni BoltStats.");
        }

        if (impactEffect != null)
        {
            GameObject impact = Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(impact, 1.5f);
        }

        Destroy(gameObject);
    }
}