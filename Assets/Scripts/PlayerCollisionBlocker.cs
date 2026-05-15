using UnityEngine;

public class PlayerCollisionBlocker : MonoBehaviour
{
    [Header("Collision Settings")]
    public string blockingTag = "BlockedWall";

    public bool isBlocked = false;

    private int blockingContacts = 0;

    private bool IsBlockingCollision(Collision collision)
    {
        return collision.gameObject.CompareTag(blockingTag) ||
               collision.transform.root.CompareTag(blockingTag);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsBlockingCollision(collision))
        {
            blockingContacts++;
            isBlocked = true;

            Debug.Log("BOLT chocó con una pared. Contactos: " + blockingContacts);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (IsBlockingCollision(collision))
        {
            isBlocked = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (IsBlockingCollision(collision))
        {
            blockingContacts = Mathf.Max(0, blockingContacts - 1);
            isBlocked = blockingContacts > 0;

            Debug.Log("BOLT dejó una pared. Contactos restantes: " + blockingContacts);
        }
    }
}