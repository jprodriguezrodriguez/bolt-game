using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDefense : MonoBehaviour
{
    [Header("References")]
    public BoltStats stats;
    public Animator animator;

    [Header("Defense Settings")]
    public int energyCostOnHit = 1;
    public float energyDrainInterval = 1f;
    public int energyDrainAmount = 1;

    [Header("Knockback")]
    public Rigidbody playerRigidbody;
    public float knockbackForce = 6f;
    public float upwardForce = 1.5f;

    private bool isCovering = false;
    private float energyDrainTimer = 0f;

    public bool IsCovering
    {
        get { return isCovering; }
    }

    void Start()
    {
        if (stats == null)
        {
            stats = GetComponent<BoltStats>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody>();

            if (playerRigidbody == null)
            {
                playerRigidbody = GetComponentInChildren<Rigidbody>();
            }
        }
    }

    void Update()
    {
        bool wantsToCover = Keyboard.current.qKey.isPressed;

        if (wantsToCover && stats != null && stats.HasEnergy(1))
        {
            isCovering = true;
            DrainEnergyWhileCovering();
        }
        else
        {
            isCovering = false;
            energyDrainTimer = 0f;
        }

        UpdateAnimation();
    }

    private void DrainEnergyWhileCovering()
    {
        energyDrainTimer += Time.deltaTime;

        if (energyDrainTimer >= energyDrainInterval)
        {
            stats.UseEnergy(energyDrainAmount);
            energyDrainTimer = 0f;
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        animator.SetBool("isCovering", isCovering);
    }

    public void ApplyExplosionDamage(int normalDamage, int coveredDamage, Vector3 explosionPosition)
    {
        if (stats == null) return;

        if (isCovering && stats.HasEnergy(energyCostOnHit))
        {
            stats.TakeDamage(coveredDamage);
            stats.UseEnergy(energyCostOnHit);

            ApplyKnockback(explosionPosition, 0.4f);

            Debug.Log("BOLT se cubrió. Daño reducido y empuje menor.");
        }
        else
        {
            stats.TakeDamage(normalDamage);

            ApplyKnockback(explosionPosition, 1f);

            Debug.Log("BOLT no se cubrió. Daño completo y empuje fuerte.");
        }
    }

    public void ApplyKnockback(Vector3 explosionPosition, float forceMultiplier = 1f)
    {
        if (playerRigidbody == null) return;

        Vector3 direction = transform.position - explosionPosition;
        direction.y = 0f;
        direction.Normalize();

        Vector3 finalForce = direction * knockbackForce * forceMultiplier;
        finalForce.y = upwardForce;

        playerRigidbody.AddForce(finalForce, ForceMode.Impulse);

        Debug.Log("BOLT fue empujado por la explosión.");
    }
}