using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("=== PANELes DEL MENÚ ===")]
    public GameObject mainMenuPanel;      // Panel actual con PLAY, OPTIONS, EXIT
    public GameObject playMenuPanel;      // El nuevo menú que quieres mostrar al pulsar PLAY
    public GameObject optionsPanel;       // Panel de opciones (puedes crearlo después)

    // ================== BOTONES ==================
    public void PlayButton()
    {
        mainMenuPanel.SetActive(false);
        playMenuPanel.SetActive(true);
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