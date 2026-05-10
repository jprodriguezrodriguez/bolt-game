using UnityEngine;

public class PortalTeleport : MonoBehaviour
{
    [Header("Teleport")]
    public string playerTag = "Player";
    public Transform destinationPoint;

    private bool hasTeleported = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTeleported) return;
        if (!other.CompareTag(playerTag) || destinationPoint == null) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb == null)
            rb = other.GetComponentInChildren<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        other.transform.position = destinationPoint.position;
        other.transform.rotation = destinationPoint.rotation;

        hasTeleported = true;

        Debug.Log("BOLT fue teletransportado a: " + destinationPoint.position);
    }
}