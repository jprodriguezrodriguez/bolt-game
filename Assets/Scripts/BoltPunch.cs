using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoltPunch : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Hitbox")]
    public BoltPunchHitbox punchHitbox;

    [Header("Configuración")]
    public bool canAttack = true;
    public float attackDuration = 0.8f;
    public float hitboxStartTime = 0.25f;
    public float hitboxActiveTime = 0.25f;

    [Header("Audio")]
    public AudioSource audioSource; // El componente que reproduce el sonido
    public AudioClip punchSound;    // El archivo de sonido del puño

    private bool isAttacking = false;
    private bool inputReady = false;

    private void Start()
    {
        if (punchHitbox != null)
            punchHitbox.DesactivarDaño();

        StartCoroutine(EnableInputAfterShortDelay());
    }

    private void OnDisable()
    {
        if (punchHitbox != null)
            punchHitbox.DesactivarDaño();

        isAttacking = false;
    }

    private void Update()
    {
        if (!inputReady)
            return;

        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame && canAttack && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        if (animator != null)
            animator.SetTrigger("punch");

        // Sonido de puño
        if (audioSource != null && punchSound != null)
        {
            audioSource.PlayOneShot(punchSound);
        }

        yield return new WaitForSeconds(hitboxStartTime);

        if (punchHitbox != null)
            punchHitbox.ActivarDaño();

        yield return new WaitForSeconds(hitboxActiveTime);

        if (punchHitbox != null)
            punchHitbox.DesactivarDaño();

        float remainingTime = attackDuration - hitboxStartTime - hitboxActiveTime;

        if (remainingTime > 0f)
            yield return new WaitForSeconds(remainingTime);

        isAttacking = false;
    }

    private IEnumerator EnableInputAfterShortDelay()
    {
        yield return new WaitForSeconds(0.3f);
        inputReady = true;
    }
}