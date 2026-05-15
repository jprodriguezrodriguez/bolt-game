using UnityEngine;
using UnityEngine.InputSystem;

public class Bolt : MonoBehaviour
{
    public float speed = 1f;
    public float rotationSpeed = 120f;
    public float runningSpeed = 3f;
    public float jumpForce = 5f;

    private Animator anim;
    private BoltStats stats;
    private Rigidbody rb;
    private PlayerDefense defense;
    private CapsuleCollider capsuleCollider;

    [Header("Wall Detection")]
    public float wallCheckDistance = 0.45f;
    public float wallCheckRadius = 0.25f;
    public LayerMask wallLayer;

    [Header("Collision Blocker")]
    public PlayerCollisionBlocker collisionBlocker;


    private bool isGrounded = true;
    private Vector3 moveDirection = Vector3.zero;
    private float currentMoveSpeed = 0f;
    private float rotationInput = 0f;


    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        stats = GetComponent<BoltStats>();
        rb = GetComponent<Rigidbody>();
        defense = GetComponent<PlayerDefense>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        if (anim == null)
            Debug.LogError("No se encontró Animator en el objeto hijo.");

        if (stats == null)
            Debug.LogError("No se encontró BoltStats en el objeto.");

        if (rb == null)
            Debug.LogError("No se encontró Rigidbody en PlayerCapsule.");

        if (capsuleCollider == null)
            Debug.LogError("No se encontró CapsuleCollider en PlayerCapsule.");

        if (collisionBlocker == null)
            collisionBlocker = GetComponent<PlayerCollisionBlocker>();
    }

    void Update()
    {
        if (anim == null) return;
        if (Keyboard.current == null) return;

        moveDirection = Vector3.zero;
        currentMoveSpeed = 0f;
        rotationInput = 0f;

        bool isMoving = false;
        bool wantsToRun = Keyboard.current.leftShiftKey.isPressed;
        bool canRun = stats != null && stats.currentEnergy > 0f;
        bool isRunning = wantsToRun && canRun;

        float selectedSpeed = isRunning ? runningSpeed : speed;

        if (stats != null)
        {
            stats.IsRunning = false;
        }

        if (Keyboard.current.wKey.isPressed)
        {
            if (!IsWallInFront())
            {
                moveDirection = transform.forward;
                currentMoveSpeed = selectedSpeed;
            }
            else
            {
                moveDirection = Vector3.zero;
                currentMoveSpeed = 0f;
                Debug.Log("Movimiento hacia adelante bloqueado por pared.");
            }

            anim.SetInteger("boltStates", isRunning ? 3 : 1);
            isMoving = true;

            if (stats != null)
            {
                stats.IsRunning = isRunning;
            }
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            moveDirection = -transform.forward;
            currentMoveSpeed = selectedSpeed;

            anim.SetInteger("boltStates", isRunning ? 3 : 1);
            isMoving = true;

            if (stats != null)
            {
                stats.IsRunning = isRunning;
            }
        }

        if (Keyboard.current.aKey.isPressed)
        {
            rotationInput = -1f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            rotationInput = 1f;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            anim.SetInteger("boltStates", 2);
        }
        else if (!isMoving && isGrounded)
        {
            anim.SetInteger("boltStates", 0);
        }

        if (Keyboard.current.kKey.wasPressedThisFrame && stats != null)
        {
            stats.TakeDamage(1);
        }

        if (Keyboard.current.lKey.wasPressedThisFrame && stats != null)
        {
            stats.RecoverHealth(1);
        }
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        if (moveDirection != Vector3.zero && currentMoveSpeed > 0f)
        {
            Vector3 newPosition = rb.position + moveDirection.normalized * currentMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }

        if (rotationInput != 0f)
        {
            Quaternion deltaRotation = Quaternion.Euler(
                0f,
                rotationInput * rotationSpeed * Time.fixedDeltaTime,
                0f
            );

            rb.MoveRotation(rb.rotation * deltaRotation);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private bool IsWallInFront()
    {
        if (capsuleCollider == null)
            return false;

        Vector3 origin = transform.TransformPoint(capsuleCollider.center) + Vector3.up * 0.2f;
        Vector3 direction = transform.forward;

        bool wallDetected = Physics.SphereCast(
            origin,
            wallCheckRadius,
            direction,
            out RaycastHit hit,
            wallCheckDistance,
            wallLayer,
            QueryTriggerInteraction.Ignore
        );

        if (wallDetected)
        {
            Debug.Log("Pared detectada al frente: " + hit.collider.name);
        }

        return wallDetected;
    }
}