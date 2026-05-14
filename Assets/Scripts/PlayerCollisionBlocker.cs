using UnityEngine;

public class PlayerCollisionBlocker : MonoBehaviour
{
    [Header("Collision Settings")]
    public string blockingTag = "Wall";

    public bool isBlocked = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(blockingTag))
        {
            isBlocked = true;
            Debug.Log("BOLT chocó con una pared. Movimiento bloqueado.");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(blockingTag))
        {
            isBlocked = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(blockingTag))
        {
            isBlocked = false;
            Debug.Log("BOLT dejó de tocar la pared. Movimiento desbloqueado.");
        }
    }
}