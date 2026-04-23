using UnityEngine;

public class DoorMechanism : MonoBehaviour
{
    [Header("Puertas")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Header("Movimiento")]
    [SerializeField] private float riseHeight = 4f;
    [SerializeField] private float speed = 2f;

    private Vector3 leftTargetPos;
    private Vector3 rightTargetPos;
    private bool activated = false;

    void Start()
    {
        if (leftDoor != null)
            leftTargetPos = leftDoor.position + Vector3.up * riseHeight;

        if (rightDoor != null)
            rightTargetPos = rightDoor.position + Vector3.up * riseHeight;
    }

    void Update()
    {
        if (!activated) return;

        if (leftDoor != null)
        {
            leftDoor.position = Vector3.MoveTowards(
                leftDoor.position,
                leftTargetPos,
                speed * Time.deltaTime
            );
        }

        if (rightDoor != null)
        {
            rightDoor.position = Vector3.MoveTowards(
                rightDoor.position,
                rightTargetPos,
                speed * Time.deltaTime
            );
        }
    }

    public void ActivateDoors()
    {
        activated = true;
    }
}