using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Slots de las cápsulas")]
    public CapsuleSlot[] capsuleSlots; 

    [Header("UI Victoria")]
    public GameObject winPanel;        
    public Text winText;               

    private bool gameWon = false;

    void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
    }

    public void CheckWinCondition()
    {
        if (gameWon) return;

        foreach (CapsuleSlot slot in capsuleSlots)
        {
            if (!slot.isOccupied)
                return; 
        }

        
        WinGame();
    }

    private void WinGame()
    {
        gameWon = true;
        Debug.Log("¡GANASTE! Todas las cápsulas están en su lugar.");

        if (winPanel != null)
            winPanel.SetActive(true);

        if (winText != null)
            winText.text = "¡Experimento completado!";


    }


    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}