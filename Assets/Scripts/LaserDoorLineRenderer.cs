using UnityEngine;

public class LaserDoorLineRenderer : MonoBehaviour
{
    [Header("Laser Points")]
    public Transform[] startPoints;
    public Transform[] endPoints;

    [Header("Laser Visual")]
    public Material laserMaterial;
    public float laserWidth = 0.08f;
    public Color laserColor = Color.cyan;

    [Header("Laser State")]
    public bool isActive = true;

    [Header("Laser Colliders")]
    public GameObject laserBlocker;
    public GameObject laserDamageTrigger;
    public GameObject laserParticles;

    [Header("Laser Flicker")]
    public bool enableFlicker = true;
    public float flickerSpeed = 12f;
    public float minWidth = 0.04f;
    public float maxWidth = 0.09f;
    public float minAlpha = 0.45f;
    public float maxAlpha = 1f;

    private LineRenderer[] laserLines;

    private void Start()
    {
        CreateLasers();
        UpdateLaserState();
    }

    private void Update()
    {
        if (!isActive) return;

        UpdateLaserPositions();

        if (enableFlicker)
        {
            UpdateLaserFlicker();
        }
    }

    private void CreateLasers()
    {
        int laserCount = Mathf.Min(startPoints.Length, endPoints.Length);
        laserLines = new LineRenderer[laserCount];

        for (int i = 0; i < laserCount; i++)
        {
            GameObject laserObject = new GameObject("LaserLine_" + i);
            laserObject.transform.SetParent(transform);

            LineRenderer line = laserObject.AddComponent<LineRenderer>();

            line.positionCount = 2;
            line.useWorldSpace = true;
            line.startWidth = laserWidth;
            line.endWidth = laserWidth;
            line.startColor = laserColor;
            line.endColor = laserColor;

            if (laserMaterial != null)
                line.material = laserMaterial;

            laserLines[i] = line;
        }

        UpdateLaserPositions();
    }

    private void UpdateLaserPositions()
    {
        if (laserLines == null) return;

        for (int i = 0; i < laserLines.Length; i++)
        {
            if (startPoints[i] == null || endPoints[i] == null)
                continue;

            laserLines[i].SetPosition(0, startPoints[i].position);
            laserLines[i].SetPosition(1, endPoints[i].position);
        }
    }

    public void SetLaserActive(bool active)
    {
        isActive = active;
        UpdateLaserState();
    }

    private void UpdateLaserState()
    {
        if (laserLines != null)
        {
            foreach (LineRenderer line in laserLines)
            {
                if (line != null)
                    line.enabled = isActive;
            }
        }

        if (laserBlocker != null)
            laserBlocker.SetActive(isActive);

        if (laserDamageTrigger != null)
            laserDamageTrigger.SetActive(isActive);

        if (laserParticles != null)
            laserParticles.SetActive(isActive);
    }

    private void UpdateLaserFlicker()
    {
        if (laserLines == null) return;

        float flicker = Mathf.PingPong(Time.time * flickerSpeed, 1f);

        float currentWidth = Mathf.Lerp(minWidth, maxWidth, flicker);
        float currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, flicker);

        Color currentColor = laserColor;
        currentColor.a = currentAlpha;

        foreach (LineRenderer line in laserLines)
        {
            if (line == null) continue;

            line.startWidth = currentWidth;
            line.endWidth = currentWidth;

            line.startColor = currentColor;
            line.endColor = currentColor;
        }
    }
}