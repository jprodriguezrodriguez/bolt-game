using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject gameOverPanel;

    [Header("Referencias")]
    public ItemsManager itemsManager;

    private bool gameOverActive = false;

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        if (gameOverActive)
            return;

        gameOverActive = true;

        if (itemsManager != null && itemsManager.IsTitanUnlocked())
        {
            PlayerPrefs.SetInt("RetryWithTitanUnlocked", 1);
        }
        else
        {
            PlayerPrefs.SetInt("RetryWithTitanUnlocked", 0);
        }

        PlayerPrefs.Save();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("PLAY HUD");
    }
}