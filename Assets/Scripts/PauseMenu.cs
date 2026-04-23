using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;     // ← Importante

public class PauseMenu : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject pauseMenuUI;     // Arrastra aquí el PANEL del menú de pausa

    private bool isPaused = false;
    private PlayerInput playerInput;   // Para detectar ESC correctamente

    void Awake()
    {
        // Busca el PlayerInput (normalmente está en el jugador o en la cámara)
        playerInput = FindObjectOfType<PlayerInput>();
    }

    void Update()
    {
        // Detecta ESC con el nuevo Input System
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("ESC presionado");

            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("PLAY HUD");   // ← Cambia por el nombre exacto de tu escena del menú
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}