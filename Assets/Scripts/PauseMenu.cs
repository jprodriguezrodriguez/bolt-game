using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject pauseMenuUI;

    [Header("Objetos a ocultar durante la pausa")]
    public GameObject[] objectsToHideOnPause;

    [Header("Configuración del cursor")]
    public bool lockCursorDuringGameplay = true;

    private bool isPaused = false;

    private void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        SetObjectsVisibility(true);

        Time.timeScale = 1f;
        isPaused = false;

        if (lockCursorDuringGameplay)
            LockCursor();
        else
            UnlockCursor();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }

        // Seguridad extra: mientras esté pausado, mantener el cursor visible.
        if (isPaused)
            UnlockCursor();
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        SetObjectsVisibility(true);

        Time.timeScale = 1f;
        isPaused = false;

        if (lockCursorDuringGameplay)
            LockCursor();
        else
            UnlockCursor();
    }

    public void Pause()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        SetObjectsVisibility(false);

        Time.timeScale = 0f;
        isPaused = true;

        UnlockCursor();
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        UnlockCursor();
        SceneManager.LoadScene("PLAY HUD");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        UnlockCursor();

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void SetObjectsVisibility(bool visible)
    {
        if (objectsToHideOnPause == null)
            return;

        foreach (GameObject obj in objectsToHideOnPause)
        {
            if (obj != null)
                obj.SetActive(visible);
        }
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}