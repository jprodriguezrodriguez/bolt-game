using UnityEngine;

public class CameraWallCollision : MonoBehaviour
{
    [Header("Referencia del jugador")]
    public Transform target;

    [Header("Configuración de cámara")]
    public float distance = 4f;
    public float height = 1.8f;
    public float sideOffset = 0f;

    [Header("Suavizado")]
    public float positionSmoothTime = 0.08f;
    public float rotationSmoothSpeed = 12f;

    [Header("Colisión")]
    public float cameraRadius = 0.25f;
    public float collisionOffset = 0.2f;
    public LayerMask collisionLayers;

    private Vector3 currentVelocity;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 focusPoint = target.position + Vector3.up * height;

        Vector3 desiredDirection = -target.forward;
        Vector3 desiredPosition = focusPoint + desiredDirection * distance + target.right * sideOffset;

        Vector3 directionToCamera = desiredPosition - focusPoint;
        float desiredDistance = directionToCamera.magnitude;

        Vector3 finalPosition = desiredPosition;

        if (Physics.SphereCast(
            focusPoint,
            cameraRadius,
            directionToCamera.normalized,
            out RaycastHit hit,
            desiredDistance,
            collisionLayers
        ))
        {
            float adjustedDistance = Mathf.Max(hit.distance - collisionOffset, 0.5f);
            finalPosition = focusPoint + directionToCamera.normalized * adjustedDistance;
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            finalPosition,
            ref currentVelocity,
            positionSmoothTime
        );

        Quaternion targetRotation = Quaternion.LookRotation(focusPoint - transform.position);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }
}