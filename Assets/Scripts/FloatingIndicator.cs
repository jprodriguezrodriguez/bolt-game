using UnityEngine;

public class FloatingIndicator : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float floatHeight = 0.15f;
    public float rotationSpeed = 40f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.localPosition = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}