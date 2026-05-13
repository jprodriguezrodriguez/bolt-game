using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalLoadScene : MonoBehaviour
{
    [Header("Scene Load")]
    public string playerTag = "Player";
    public string sceneName = "Nivel3";

    private bool hasLoaded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasLoaded) return;
        if (!other.CompareTag(playerTag)) return;

        hasLoaded = true;

        Time.timeScale = 1f;

        Debug.Log("Cargando escena: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}