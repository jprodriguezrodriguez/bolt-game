using UnityEngine;

public class LeverVisual : MonoBehaviour
{
    [SerializeField] private Vector3 rotationOffset = new Vector3(0f, 0f, -35f);
    [SerializeField] private float rotationSpeed = 4f;

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private bool activated = false;

    void Start()
    {
        initialRotation = transform.localRotation;
        targetRotation = initialRotation * Quaternion.Euler(rotationOffset);
    }

    void Update()
    {
        if (!activated) return;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void ActivateLever()
    {
        activated = true;
    }
}