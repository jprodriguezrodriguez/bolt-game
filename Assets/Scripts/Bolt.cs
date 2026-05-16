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

    private bool isGrounded = true;

    [Header("Collision Blocker")]
    public PlayerCollisionBlocker collisionBlocker;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        stats = GetComponent<BoltStats>();
        rb = GetComponentInChildren<Rigidbody>();
        defense = GetComponent<PlayerDefense>();

        if (anim == null)
            Debug.LogError("No se encontró Animator en el objeto hijo.");

        if (stats == null)
            Debug.LogError("No se encontró BoltStats en el objeto.");

        if (collisionBlocker == null)
            collisionBlocker = GetComponent<PlayerCollisionBlocker>();
    }

    void Update()
    {
        if (anim == null) return;
        if (Keyboard.current == null) return;

        bool isMoving = false;
        bool wantsToRun = Keyboard.current.leftShiftKey.isPressed;
        bool canRun = stats != null && stats.currentEnergy > 0f;
        bool isRunning = wantsToRun && canRun;

        float currentSpeed = isRunning ? runningSpeed : speed;

        if (stats != null)
        {
            stats.IsRunning = false;
        }

        if (Keyboard.current.wKey.isPressed)
        {
            if (collisionBlocker == null || !collisionBlocker.isBlocked)
            {
                transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
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
            transform.Translate(Vector3.back * currentSpeed * Time.deltaTime, Space.Self);

            anim.SetInteger("boltStates", isRunning ? 3 : 1);
            isMoving = true;

            if (stats != null)
            {
                stats.IsRunning = isRunning;
            }
        }

        if (Keyboard.current.aKey.isPressed)
        {
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
        }

        if (Keyboard.current.dKey.isPressed)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}