using UnityEngine;
using TMPro;
using System.Collections;

public class ItemsManager : MonoBehaviour
{
    [Header("Items Counter")]
    public int totalItems = 3;
    private int collectedItems = 0;

    [Header("UI")]
    public TextMeshProUGUI itemsCounterText;
    public GameObject educationalTextContainer;
    public TextMeshProUGUI educationalText;

    [Header("Educational Message")]
    public float messageDuration = 5f;

    private Coroutine messageCoroutine;

    void Start()
    {
        UpdateCounterUI();

        if (educationalTextContainer != null)
        {
            educationalTextContainer.SetActive(false);
            Debug.Log(itemsCounterText.text);
        }
    }

    public void AddItem(string itemTitle, string itemEducationalText)
    {
        collectedItems++;
        UpdateCounterUI();

        ShowEducationalMessage(itemTitle, itemEducationalText);

        Debug.Log("Ítem recolectado. Total: " + collectedItems + "-" + totalItems);
    }

    private void UpdateCounterUI()
    {
        if (itemsCounterText != null)
        {
            Debug.Log("Pruebaa");
            itemsCounterText.text = "Pistas: " + collectedItems + " - " + totalItems;
        }
    }

    private void ShowEducationalMessage(string title, string message)
    {
        if (educationalTextContainer == null || educationalText == null)
        {
            Debug.LogWarning("No se asignó el texto educativo en ItemsManager.");
            return;
        }

        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }

        messageCoroutine = StartCoroutine(ShowMessageCoroutine(title, message));
    }

    private IEnumerator ShowMessageCoroutine(string title, string message)
    {
        educationalTextContainer.SetActive(true);

        educationalText.text = "<b>" + title + "</b>\n" + message;

        yield return new WaitForSeconds(messageDuration);

        educationalTextContainer.SetActive(false);
    }

    public int GetCollectedItems()
    {
        return collectedItems;
    }
}