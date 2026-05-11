using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LightningEffect : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    [Header("Lightning Settings")]
    public int segments = 10;
    public float randomness = 0.3f;
    public float refreshRate = 0.05f;

    private LineRenderer lineRenderer;
    private float timer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments;
    }

    void Update()
    {
        if (startPoint == null || endPoint == null) return;

        timer += Time.deltaTime;

        if (timer >= refreshRate)
        {
            timer = 0f;
            GenerateLightning();
        }
    }

    void GenerateLightning()
    {
        Vector3 start = startPoint.position;
        Vector3 end = endPoint.position;

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / (segments - 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            if (i != 0 && i != segments - 1)
            {
                point += new Vector3(
                    Random.Range(-randomness, randomness),
                    Random.Range(-randomness, randomness),
                    Random.Range(-randomness, randomness)
                );
            }

            lineRenderer.SetPosition(i, point);
        }
    }
}