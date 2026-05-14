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
    public Image educationalPanelImage;
    public TextMeshProUGUI educationalCountdownText;
    public float educationalMessageDuration = 10f;
    public bool pauseGameWhileEducationalPanelIsOpen = true;

    [Header("UI a ocultar durante el panel educativo")]
    public GameObject[] objectsToHideWhileEducationalPanel;

    [Header("Mission UI")]
    public GameObject missionTextContainer;
    public TextMeshProUGUI missionText;
    public float missionMessageDuration = 12f;
    public string missionCompletedMessage = "<b>MISIÓN ACTUALIZADA</b>\nAhora puedes enfrentar al Titán de Vapor";

    [Header("Initial Mission")]
    public GameObject initialMissionTextContainer;
    public TextMeshProUGUI initialMissionText;
    public bool showInitialMissionOnStart = true;
    public float initialMissionDuration = 4f;
    public string initialMissionMessage = "<b>OBJETIVO</b>\nBusca las 3 pistas para avanzar";

    [Header("Counter UI")]
    public TextMeshProUGUI itemsCounterText;
    public string collectMissionText = "Recolecta las pistas";
    public string defeatTitanText = "Derrota al Titán de Vapor";

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

    [Header("Titan Guide Images")]
    public Image titanGuideHintImage;
    public Sprite titanParticlesGuideSprite;

    public Image hintBackgroundImage;
    public Sprite titanHintBackgroundSprite;

    [Header("Titan Guide Message")]
    public string titanGuideMessage = "Sigue las partículas para encontrar al Titán";

    [Header("Titan Guide Object")]
    public GameObject guideToTitan;

    [Header("Retry Settings")]
    public string retryTitanUnlockedKey = "RetryWithTitanUnlocked";

    private Coroutine combatHintCoroutine;
    private Coroutine educationalCoroutine;
    private Coroutine missionCoroutine;

    private void Start()
    {
        Debug.Log("ItemsManager inició.");

        if (educationalTextContainer != null)
            educationalTextContainer.SetActive(false);

        if (missionTextContainer != null)
            missionTextContainer.SetActive(false);

        if (initialMissionTextContainer != null)
            initialMissionTextContainer.SetActive(false);

        if (missionText != null)
            missionText.gameObject.SetActive(true);

        if (steamTitan != null)
            steamTitan.SetActive(false);

        if (steamEffect != null)
            steamEffect.SetActive(false);

        if (guideToTitan != null)
            guideToTitan.SetActive(false);

        RestoreTitanRetryState();

        UpdateCounterUI();

        if (showInitialMissionOnStart && !titanUnlocked)
        {
            ShowMissionMessage(initialMissionMessage, initialMissionDuration);
        }
    }

    public void AddItem(string itemTitle, string itemEducationalText, Sprite educationalPanelSprite = null)
    {
        if (titanUnlocked)
            return;

        collectedItems++;
        Debug.Log("Ítem recolectado. Total: " + collectedItems + " / " + totalItems);

        bool completedMission = collectedItems >= totalItems;

        if (educationalCoroutine != null)
            StopCoroutine(educationalCoroutine);

        if (completedMission)
        {
            educationalCoroutine = StartCoroutine(
                ShowFinalEducationalThenMissionCoroutine(itemTitle, itemEducationalText, educationalPanelSprite)
            );
        }
        else
        {
            ShowEducationalMessage(itemTitle, itemEducationalText, educationalPanelSprite);
        }

        UpdateCounterUI();
    }

    private void ShowEducationalMessage(string title, string message, Sprite panelSprite = null)
    {
        if (educationalTextContainer == null)
        {
            Debug.LogWarning("No se asignó la UI del mensaje educativo.");
            return;
        }

        if (educationalCoroutine != null)
            StopCoroutine(educationalCoroutine);

        educationalCoroutine = StartCoroutine(
            ShowEducationalMessageCoroutine(title, message, panelSprite)
        );
    }

    private IEnumerator ShowEducationalMessageCoroutine(string title, string message, Sprite panelSprite = null)
    {
        OpenEducationalPanel(title, message, panelSprite);

        float remainingTime = educationalMessageDuration;

        while (remainingTime > 0f)
        {
            UpdateEducationalCountdown(remainingTime);

            remainingTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        CloseEducationalPanel();
    }

    private IEnumerator ShowFinalEducationalThenMissionCoroutine(string title, string message, Sprite panelSprite = null)
    {
        OpenEducationalPanel(title, message, panelSprite);

        float remainingTime = educationalMessageDuration;

        while (remainingTime > 0f)
        {
            UpdateEducationalCountdown(remainingTime);

            remainingTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        CloseEducationalPanel();

        UnlockSteamTitan();
    }

    private void OpenEducationalPanel(string title, string message, Sprite panelSprite = null)
    {
        if (educationalTextContainer == null)
            return;

        educationalTextContainer.SetActive(true);

        // Esto manda el panel al frente del Canvas.
        educationalTextContainer.transform.SetAsLastSibling();

        SetEducationalHiddenObjects(false);

        if (educationalPanelImage != null && panelSprite != null)
        {
            educationalPanelImage.sprite = panelSprite;
            educationalPanelImage.preserveAspect = true;
            educationalPanelImage.color = Color.white;
            educationalPanelImage.enabled = true;
        }

        if (educationalText != null)
        {
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(message))
            {
                educationalText.gameObject.SetActive(false);
            }
            else
            {
                educationalText.gameObject.SetActive(true);
                educationalText.text = "<b>" + title + "</b>\n" + message;
            }
        }

        if (educationalCountdownText != null)
            educationalCountdownText.gameObject.SetActive(true);

        PauseGameForEducationalPanel();
    }

    private void CloseEducationalPanel()
    {
        if (educationalCountdownText != null)
            educationalCountdownText.gameObject.SetActive(false);

        if (educationalTextContainer != null)
            educationalTextContainer.SetActive(false);

        SetEducationalHiddenObjects(true);

        ResumeGameAfterEducationalPanel();
    }

    private void UpdateEducationalCountdown(float remainingTime)
    {
        if (educationalCountdownText != null)
        {
            educationalCountdownText.text = "Cierra en " + Mathf.CeilToInt(remainingTime) + "s";
        }
    }

    private void SetEducationalHiddenObjects(bool visible)
    {
        if (objectsToHideWhileEducationalPanel == null)
            return;

        foreach (GameObject obj in objectsToHideWhileEducationalPanel)
        {
            if (obj != null)
                obj.SetActive(visible);
        }
    }

    private void UpdateCounterUI()
    {
        if (itemsCounterText == null)
            return;

        if (!titanUnlocked)
        {
            itemsCounterText.text = collectMissionText + "\n" + collectedItems + " / " + totalItems;
        }
        else
        {
            itemsCounterText.text = defeatTitanText;
        }
    }

    private void UnlockSteamTitan()
    {
        titanUnlocked = true;

        Debug.Log("Ahora puedes enfrentar al Titán.");

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
        if (initialMissionTextContainer == null || initialMissionText == null)
        {
            Debug.LogWarning("No se asignó la UI del mensaje de misión inicial.");
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

        initialMissionText.gameObject.SetActive(true);
        initialMissionText.text = message;

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

        if (hintBackgroundImage != null && titanHintBackgroundSprite != null)
            hintBackgroundImage.sprite = titanHintBackgroundSprite;

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
            UpdateTitanGuideHint();

            Debug.Log("Reintento contra el Titán restaurado correctamente.");
        }
    }

    private void PauseGameForEducationalPanel()
    {
        if (!pauseGameWhileEducationalPanelIsOpen)
            return;

        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ResumeGameAfterEducationalPanel()
    {
        if (!pauseGameWhileEducationalPanelIsOpen)
            return;

        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}