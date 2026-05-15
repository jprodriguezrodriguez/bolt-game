using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitanDigitalHealthBar : MonoBehaviour
{
    [Header("Referencia al Titán Digital")]
    public TitanDigital titan;

    [Header("Referencia al progreso")]
    public ItemsManager itemsManager;

    [Header("Elementos UI")]
    public GameObject healthBarContainer;
    public Image healthBarFill;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI titanNameText;

    [Header("Configuración")]
    public string titanName = "TITÁN DIGITAL";

    private void Start()
    {
        if (healthBarContainer != null)
            healthBarContainer.SetActive(false);

        if (titanNameText != null)
            titanNameText.text = titanName;
    }

    private void Update()
    {
        if (healthBarContainer == null)
            return;

        if (itemsManager == null || !itemsManager.IsTitanUnlocked())
        {
            healthBarContainer.SetActive(false);
            return;
        }

        if (titan == null)
        {
            healthBarContainer.SetActive(false);
            return;
        }

        if (titan.estaMuerto)
        {
            healthBarContainer.SetActive(false);
            return;
        }

        if (!healthBarContainer.activeSelf)
            healthBarContainer.SetActive(true);

        float porcentaje = titan.ObtenerPorcentajeSalud();

        if (healthBarFill != null)
            healthBarFill.fillAmount = porcentaje;

        if (healthText != null)
        {
            float saludActual = titan.ObtenerSaludActual();
            float saludMaxima = titan.ObtenerSaludMaxima();

            healthText.text = saludActual.ToString("0") + " / " + saludMaxima.ToString("0");
        }
    }
}