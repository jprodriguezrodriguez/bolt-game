using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PortalTeleport : MonoBehaviour
{
    [Header("Scene Load")]
    public string playerTag = "Player";
    public string sceneName = "Nivel 2";
    private bool hasLoaded = false;

    [Header("Pantalla de Carga")]
    public GameObject panelCarga;
    public Image barraRelleno;
    public TextMeshProUGUI textoCargando;

    void Start()
    {
        if (panelCarga != null)
            panelCarga.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Into portal teleport");
        if (hasLoaded) return;
        if (!other.CompareTag(playerTag)) return;

        hasLoaded = true;
        Time.timeScale = 1f;
        Debug.Log("Cargando escena: " + sceneName);

        // Si no hay panel configurado, carga directo como antes
        if (panelCarga == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        panelCarga.SetActive(true);
        StartCoroutine(CargarConPantalla());
    }

    IEnumerator CargarConPantalla()
    {
        yield return new WaitForSeconds(0.5f);

        AsyncOperation operacion = SceneManager.LoadSceneAsync(sceneName);
        operacion.allowSceneActivation = false;

        float tiempoMinimo = 3f;
        float tiempoTranscurrido = 0f;

        while (!operacion.isDone)
        {
            tiempoTranscurrido += Time.deltaTime;

            float progresoCarga = Mathf.Clamp01(operacion.progress / 0.9f);
            float progresoFinal = Mathf.Min(progresoCarga, tiempoTranscurrido / tiempoMinimo);

            if (barraRelleno != null)
                barraRelleno.fillAmount = progresoFinal;

            if (textoCargando != null)
                textoCargando.text = "Cargando... " + Mathf.RoundToInt(progresoFinal * 100) + "%";

            if (progresoFinal >= 1f)
            {
                if (textoCargando != null)
                    textoCargando.text = "¡Listo!";
                yield return new WaitForSeconds(0.5f);
                operacion.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}