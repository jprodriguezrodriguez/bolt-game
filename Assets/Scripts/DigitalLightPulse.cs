using UnityEngine;

public class DigitalLightPulse : MonoBehaviour
{
    [Header("Light Reference")]
    public Light targetLight;

    [Header("Pulse Settings")]
    public float minIntensity = 1.2f;
    public float maxIntensity = 3f;
    public float pulseSpeed = 2f;

    [Header("Range Settings")]
    public bool pulseRange = true;
    public float minRange = 2.5f;
    public float maxRange = 4f;

    private void Awake()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();
    }

    private void Update()
    {
        if (targetLight == null)
            return;

        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

        targetLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, pulse);

        if (pulseRange)
            targetLight.range = Mathf.Lerp(minRange, maxRange, pulse);
    }
}