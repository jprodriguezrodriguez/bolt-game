using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Movement")]
    public Vector3 openOffset = new Vector3(0f, 3f, 0f);
    public float openSpeed = 2f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool shouldOpen = false;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + openOffset;
    }

    void Update()
    {
        if (shouldOpen)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                openPosition,
                Time.deltaTime * openSpeed
            );
        }
    }

    public void OpenDoor()
    {
        shouldOpen = true;
    }
}