using UnityEngine;

public class LaserDoor : MonoBehaviour
{
    [Header("Laser Settings")]
    public bool isActive = true;
    public int damage = 10;
    public float damageCooldown = 1f;

    [Header("References")]
    public GameObject laserVisual;
    public GameObject laserBlocker;

    [Header("Player")]
    public string playerTag = "Player";

    private float lastDamageTime = -999f;

    private void Start()
    {
        UpdateLaserState();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isActive) return;

        if (other.CompareTag(playerTag))
        {
            if (Time.time >= lastDamageTime + damageCooldown)
            {
                BoltStats stats = other.GetComponent<BoltStats>();

                if (stats == null)
                    stats = other.GetComponentInParent<BoltStats>();

                if (stats != null)
                {
                    stats.TakeDamage(damage);
                    lastDamageTime = Time.time;

                    if (DamageFlashUI.Instance != null)
                        DamageFlashUI.Instance.ShowDamageFlash();

                    Debug.Log("BOLT recibió daño por puerta láser.");
                }
            }
        }
    }

    public void SetLaserActive(bool active)
    {
        isActive = active;
        UpdateLaserState();
    }

    private void UpdateLaserState()
    {
        if (laserVisual != null)
            laserVisual.SetActive(isActive);

        if (laserBlocker != null)
            laserBlocker.SetActive(isActive);
    }
}