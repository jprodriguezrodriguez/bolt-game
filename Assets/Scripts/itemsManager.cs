using UnityEngine;
using TMPro;
using System.Collections;

public class ItemsManager : MonoBehaviour
{
    [Header("Items Counter")]
    public int totalItems = 3;
    private int collectedItems = 0;

    [Header("Educational UI")]
    public GameObject educationalTextContainer;
    public TextMeshProUGUI educationalText;
    public float educationalMessageDuration = 5f;

    [Header("Mission UI")]
    public GameObject missionTextContainer;
    public TextMeshProUGUI missionText;
    public float missionMessageDuration = 5f;
    public string missionCompletedMessage = "<b>MISIÓN ACTUALIZADA</b>\nAhora puedes enfrentar al Titán de Vapor";

    [Header("Counter UI")]
    public TextMeshProUGUI itemsCounterText;

    [Header("Titan Unlock")]
    public GameObject steamTitan;
    public GameObject steamEffect;
    public Animator steamTitanAnimator;
    public string titanAppearTrigger = "Appear";

    private bool titanUnlocked = false;
    private Coroutine educationalCoroutine;
    private Coroutine missionCoroutine;

    void Start()
    {
        UpdateCounterUI();

        if (educationalTextContainer != null)
            educationalTextContainer.SetActive(false);

        if (missionTextContainer != null)
            missionTextContainer.SetActive(false);

        if (steamTitan != null)
            steamTitan.SetActive(false);

        if (steamEffect != null)
            steamEffect.SetActive(false);
    }

    public void AddItem(string itemTitle, string itemEducationalText)
    {
        collectedItems++;
        Debug.Log("Ítem recolectado. Total: " + collectedItems + " / " + totalItems);

        bool completedMission = collectedItems >= totalItems;

        if (educationalCoroutine != null)
        {
            StopCoroutine(educationalCoroutine);
        }

        educationalCoroutine = StartCoroutine(
            ShowEducationalMessageCoroutine(itemTitle, itemEducationalText, completedMission)
        );

        UpdateCounterUI();
    }

    private void UpdateCounterUI()
    {
        if (itemsCounterText == null) return;

        if (!titanUnlocked)
            itemsCounterText.text = "Pistas: " + collectedItems + " / " + totalItems;
        else
            itemsCounterText.text = "Derrota al Titán de Vapor";
    }

    private IEnumerator ShowEducationalMessageCoroutine(string title, string message, bool unlockAfter)
    {
        if (educationalTextContainer != null)
            educationalTextContainer.SetActive(true);

        if (educationalText != null)
            educationalText.text = "<b>" + title + "</b>\n" + message;

        yield return new WaitForSeconds(educationalMessageDuration);

        if (educationalTextContainer != null)
            educationalTextContainer.SetActive(false);

        if (unlockAfter && !titanUnlocked)
        {
            UnlockSteamTitan();
        }
    }

    private void UnlockSteamTitan()
    {
        titanUnlocked = true;

        Debug.Log("Ahora puedes enfrentar al Titán de Vapor");

        if (steamTitan != null)
            steamTitan.SetActive(true);

        if (steamEffect != null)
            steamEffect.SetActive(true);

        if (steamTitanAnimator != null)
            steamTitanAnimator.SetTrigger(titanAppearTrigger);

        ShowMissionMessage(missionCompletedMessage);
        UpdateCounterUI();
    }

    private void ShowMissionMessage(string message)
    {
        if (missionTextContainer == null || missionText == null)
        {
            Debug.LogWarning("No se asignó la UI del mensaje de misión en ItemsManager.");
            return;
        }

        if (missionCoroutine != null)
            StopCoroutine(missionCoroutine);

        missionCoroutine = StartCoroutine(ShowMissionMessageCoroutine(message));
    }

    private IEnumerator ShowMissionMessageCoroutine(string message)
    {
        missionTextContainer.SetActive(true);
        missionText.text = message;

        yield return new WaitForSeconds(missionMessageDuration);

        missionTextContainer.SetActive(false);
    }

    public int GetCollectedItems()
    {
        return collectedItems;
    }
}