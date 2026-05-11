using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageFlashUI : MonoBehaviour
{
    public static DamageFlashUI Instance;

    [Header("Referencias")]
    public Image flashImage;

    [Header("Colores")]
    public Color damageColor = new Color(1f, 0f, 0f, 0.35f);
    public Color blockedColor = new Color(1f, 0.85f, 0f, 0.30f);

    [Header("Configuración")]
    public float flashDuration = 0.25f;

    private Coroutine flashCoroutine;

    private void Awake()
    {
        Instance = this;

        if (flashImage != null)
        {
            Color color = flashImage.color;
            color.a = 0f;
            flashImage.color = color;
        }
    }

    public void ShowDamageFlash()
    {
        ShowFlash(damageColor);
    }

    public void ShowBlockedFlash()
    {
        ShowFlash(blockedColor);
    }

    private void ShowFlash(Color color)
    {
        if (flashImage == null)
            return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashCoroutine(color));
    }

    private IEnumerator FlashCoroutine(Color color)
    {
        flashImage.color = color;

        float timer = 0f;

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(color.a, 0f, timer / flashDuration);

            Color newColor = color;
            newColor.a = alpha;
            flashImage.color = newColor;

            yield return null;
        }

        Color finalColor = flashImage.color;
        finalColor.a = 0f;
        flashImage.color = finalColor;
    }
}