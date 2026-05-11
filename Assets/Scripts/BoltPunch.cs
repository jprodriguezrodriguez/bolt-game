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

    private bool isAttacking = false;

    private void Update()
    {
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

        animator.SetTrigger("punch");

        yield return new WaitForSeconds(hitboxStartTime);

        if (punchHitbox != null)
            punchHitbox.ActivarDaño();

        yield return new WaitForSeconds(hitboxActiveTime);

        if (punchHitbox != null)
            punchHitbox.DesactivarDaño();

        yield return new WaitForSeconds(attackDuration - hitboxStartTime - hitboxActiveTime);

        isAttacking = false;
    }
}