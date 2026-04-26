using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        transform.forward = mainCamera.transform.forward;
    }
}