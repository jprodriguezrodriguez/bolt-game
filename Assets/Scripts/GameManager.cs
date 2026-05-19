using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;   // Para Timeline
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Slots de las cápsulas")]
    public CapsuleSlot[] capsuleSlots;

    [Header("--- OPCIÓN A: Timeline/Cinemática en la misma escena ---")]
    public PlayableDirector cinematicTimeline;   
    public GameObject cinematicCamera;          
    public GameObject playerCamera;             
    public MonoBehaviour playerController;     

    [Header("--- OPCIÓN B: Cargar otra escena ---")]
    public bool useSceneInstead = false;
    public string cinematicSceneName = "Cinematica_Final"; // Nombre exacto de tu escena

    [Header("UI")]
    public GameObject winPanel;

    private bool gameWon = false;

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (cinematicCamera != null) cinematicCamera.SetActive(false);
    }

    public void CheckWinCondition()
    {
        if (gameWon) return;

        foreach (CapsuleSlot slot in capsuleSlots)
            if (!slot.isOccupied) return;

        gameWon = true;
        StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {
        // Pequeña pausa dramática antes de la cinemática
        yield return new WaitForSeconds(0.8f);

        if (useSceneInstead)
        {
            // OPCIÓN B — Cargar escena de cinemática
            SceneManager.LoadScene(cinematicSceneName);
        }
        else
        {
            // OPCIÓN A — Reproducir Timeline en la misma escena

            // 1. Desactivar control del jugador
            if (playerController != null) playerController.enabled = false;

            // 2. Cambiar cámaras
            if (playerCamera != null) playerCamera.SetActive(false);
            if (cinematicCamera != null) cinematicCamera.SetActive(true);

            // 3. Reproducir la cinemática
            if (cinematicTimeline != null)
            {
                cinematicTimeline.Play();

                // Esperar a que termine el Timeline
                yield return new WaitForSeconds((float)cinematicTimeline.duration);
            }

            // 4. Mostrar panel final (opcional, después de la cinemática)
            if (winPanel != null) winPanel.SetActive(true);
        }
    }

    // Botones del WinPanel
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame() => Application.Quit();
}