using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("=== PANELes DEL MENÚ ===")]
    public GameObject mainMenuPanel;
    public GameObject playMenuPanel;
    public GameObject optionsPanel;

    [Header("=== NOMBRE DE LA ESCENA DEL JUEGO ===")]
    public string Nivel1 = "Nivel 1";   // ← Cambia esto por el nombre EXACTO de tu escena

    // ================== BOTONES ==================
    public void PlayButton()
    {
        mainMenuPanel.SetActive(false);
        playMenuPanel.SetActive(true);     // Muestra el menú de Nueva Partida
    }

    public void NewGameButton()            // ← NUEVO MÉTODO
    {
        SceneManager.LoadScene(Nivel1);
    }

    public void OptionsButton()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void BackToMain()
    {
        playMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ExitButton()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}