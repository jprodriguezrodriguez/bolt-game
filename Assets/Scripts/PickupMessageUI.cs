using System.Collections;
using TMPro;
using UnityEngine;

public class PickupMessageUI : MonoBehaviour
{
    public static PickupMessageUI Instance;

    [Header("Referencias UI")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    [Header("Configuración")]
    public float displayTime = 2f;

    private Coroutine currentCoroutine;

    private void Awake()
    {
        Instance = this;

        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        if (messagePanel == null || messageText == null)
            return;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ShowMessageCoroutine(message));
    }

    private IEnumerator ShowMessageCoroutine(string message)
    {
        messageText.text = message;
        messagePanel.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        messagePanel.SetActive(false);
    }
}