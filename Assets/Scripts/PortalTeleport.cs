using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTeleport : MonoBehaviour
{
    [Header("Scene Load")]
    public string playerTag = "Player";
    public string sceneName = "Nivel 2";

    private bool hasLoaded = false;

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Into portal teleport");
        Debug.Log(playerTag);
        Debug.Log(sceneName);

        if (hasLoaded) return;
        if (!other.CompareTag(playerTag)) return;

        hasLoaded = true;

        Time.timeScale = 1f;

        Debug.Log("Cargando escena: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}