using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void ActualizarBarra(float actual, float maximo)
    {
        float porcentaje = maximo > 0 ? actual / maximo : 0f;
        porcentaje = Mathf.Clamp01(porcentaje);

        if (fillImage != null)
        {
            fillImage.fillAmount = porcentaje;
        }
    }
}