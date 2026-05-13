using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

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

    [Header("Initial Mission")]
    public GameObject initialMissionTextContainer;
    public TextMeshProUGUI initialMissionText;
    public bool showInitialMissionOnStart = true;
    public float initialMissionDuration = 4f;
    public string initialMissionMessage = "<b>OBJETIVO</b>\nBusca las 3 pistas para avanzar";

    [Header("Counter UI")]
    public TextMeshProUGUI itemsCounterText;

    [Header("Titan Unlock")]
    public GameObject steamTitan;
    public GameObject steamEffect;
    public Animator steamTitanAnimator;
    public string titanAppearTrigger = "Appear";
    private bool titanUnlocked = false;

    [Header("Combat Hint UI")]
    public GameObject combatHintContainer;
    public TextMeshProUGUI combatHintText;
    public float combatHintDuration = 6f;
    public string combatHintMessage = "Clic izquierdo: atacar al Titán\nQ: cubrirte de sus ataques";

    [Header("Titan Guide Hint UI")]
    public GameObject titanGuideHintContainer;
    public TextMeshProUGUI titanGuideHintText;
    public Image titanGuideHintImage;
    public Sprite titanParticlesGuideSprite;
    public string titanGuideMessage = "Sigue las partículas para encontrar al Titán";
    public GameObject guideToTitan;

    [Header("Retry Settings")]
    public string retryTitanUnlockedKey = "RetryWithTitanUnlocked";

    private Coroutine combatHintCoroutine;

    private Coroutine educationalCoroutine;
    private Coroutine missionCoroutine;

    void Start()
    {
        Debug.Log("ItemsManager inició.");

        UpdateCounterUI();

        if (educationalTextContainer != null)
            educationalTextContainer.SetActive(false);

        if (missionTextContainer != null)
            missionTextContainer.SetActive(false);

        if (missionText != null)
            missionText.gameObject.SetActive(true);

        if (steamTitan != null)
            steamTitan.SetActive(false);

        if (steamEffect != null)
            steamEffect.SetActive(false);

        if (showInitialMissionOnStart && !titanUnlocked)
        {
            ShowMissionMessage(initialMissionMessage, initialMissionDuration);
        }
        if (guideToTitan != null)
            guideToTitan.SetActive(false);
    }

    public void AddItem(string itemTitle, string itemEducationalText)
    {
        collectedItems++;
        Debug.Log("Ítem recolectado. Total: " + collectedItems + " / " + totalItems);

        bool completedMission = collectedItems >= totalItems;

        if (!titanUnlocked && completedMission)
        {
            if (educationalCoroutine != null)
                StopCoroutine(educationalCoroutine);

            educationalCoroutine = StartCoroutine(ShowFinalEducationalThenMissionCoroutine(itemTitle, itemEducationalText));
        }
        else if (!titanUnlocked)
        {
            ShowEducationalMessage(itemTitle, itemEducationalText);
        }

        UpdateCounterUI();
    }

    private IEnumerator ShowFinalEducationalThenMissionCoroutine(string title, string message)
    {
        if (educationalTextContainer != null && educationalText != null)
        {
            educationalTextContainer.SetActive(true);
            educationalText.text = "<b>" + title + "</b>\n" + message;

            yield return new WaitForSeconds(educationalMessageDuration);

            educationalTextContainer.SetActive(false);
        }

        UnlockSteamTitan();
    }

    private void UpdateCounterUI()
    {
        if (itemsCounterText == null) return;

        if (!titanUnlocked)
            itemsCounterText.text = "Pistas: " + collectedItems + " / " + totalItems;
        else
            itemsCounterText.text = "Derrota al Titán de Vapor";
    }

    private void ShowEducationalMessage(string title, string message)
    {
        if (educationalTextContainer == null || educationalText == null)
        {
            Debug.LogWarning("No se asignó la UI del mensaje educativo.");
            return;
        }

        if (educationalCoroutine != null)
            StopCoroutine(educationalCoroutine);

        educationalCoroutine = StartCoroutine(ShowEducationalMessageCoroutine(title, message));
    }

    private IEnumerator ShowEducationalMessageCoroutine(string title, string message)
    {
        educationalTextContainer.SetActive(true);
        educationalText.text = "<b>" + title + "</b>\n" + message;

        yield return new WaitForSeconds(educationalMessageDuration);

        educationalTextContainer.SetActive(false);
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

        UpdateCounterUI();

        ShowMissionMessage(missionCompletedMessage, missionMessageDuration);
        ShowCombatHint();
        UpdateTitanGuideHint();

    }

    private void ShowMissionMessage(string message, float duration)
    {
        if (missionTextContainer == null || missionText == null)
        {
            Debug.LogWarning("No se asignó la UI del mensaje de misión.");
            return;
        }

        if (missionCoroutine != null)
            StopCoroutine(missionCoroutine);

        missionCoroutine = StartCoroutine(ShowMissionMessageCoroutine(message, duration));
    }

    private IEnumerator ShowMissionMessageCoroutine(string message, float duration)
    {
        Debug.Log("Mostrando mensaje de misión: " + message);

        initialMissionTextContainer.SetActive(true);

        if (initialMissionText != null)
        {
            initialMissionText.gameObject.SetActive(true);
            initialMissionText.text = message;
        }

        yield return null;
        yield return new WaitForSeconds(duration);

        initialMissionTextContainer.SetActive(false);
    }

    public int GetCollectedItems()
    {
        return collectedItems;
    }

    public bool IsTitanUnlocked()
    {
        return titanUnlocked;
    }

    private void ShowCombatHint()
    {
        if (combatHintContainer == null || combatHintText == null)
        {
            Debug.LogWarning("No se asignó la UI del mensaje de combate.");
            return;
        }

        if (combatHintCoroutine != null)
            StopCoroutine(combatHintCoroutine);

        combatHintCoroutine = StartCoroutine(ShowCombatHintCoroutine());
    }

    private IEnumerator ShowCombatHintCoroutine()
    {
        combatHintContainer.SetActive(true);
        combatHintText.text = combatHintMessage;

        yield return new WaitForSeconds(combatHintDuration);

        combatHintContainer.SetActive(false);
    }

    private void UpdateTitanGuideHint()
    {
        if (titanGuideHintContainer != null)
            titanGuideHintContainer.SetActive(true);

        if (titanGuideHintText != null)
            titanGuideHintText.text = titanGuideMessage;

        if (titanGuideHintImage != null && titanParticlesGuideSprite != null)
            titanGuideHintImage.sprite = titanParticlesGuideSprite;

        if (guideToTitan != null)
            guideToTitan.SetActive(true);


        Debug.Log("Guía actualizada: sigue las partículas para encontrar al Titán.");
    }

    private void RestoreTitanRetryState()
    {
        int retryState = PlayerPrefs.GetInt(retryTitanUnlockedKey, 0);

        Debug.Log("ItemsManager leyó " + retryTitanUnlockedKey + " = " + retryState);

        if (retryState == 1)
        {
            collectedItems = totalItems;
            titanUnlocked = true;

            if (steamTitan != null)
                steamTitan.SetActive(true);

            if (steamEffect != null)
                steamEffect.SetActive(true);

            if (guideToTitan != null)
                guideToTitan.SetActive(true);

            UpdateCounterUI();

            // Solo si ya agregaste este método para cambiar texto/imagen guía
            UpdateTitanGuideHint();

            Debug.Log("Reintento contra el Titán restaurado correctamente.");
        }
    }
}