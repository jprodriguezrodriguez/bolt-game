using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject gameOverPanel;

    [Header("Referencias")]
    public ItemsManager itemsManager;

    [Header("Retry Settings")]
    public string retryTitanUnlockedKey = "RetryWithTitanUnlocked";

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

        bool titanUnlocked = itemsManager != null && itemsManager.IsTitanUnlocked();

        PlayerPrefs.SetInt(retryTitanUnlockedKey, titanUnlocked ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("GameOver guardó " + retryTitanUnlockedKey + " = " + PlayerPrefs.GetInt(retryTitanUnlockedKey));

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Debug.Log("BOTÓN REINTENTAR PRESIONADO");

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("PLAY HUD");
    }
}