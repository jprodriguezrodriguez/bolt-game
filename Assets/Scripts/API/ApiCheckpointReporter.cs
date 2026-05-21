using UnityEngine;

public class ApiCheckpointReporter : MonoBehaviour
{
    [Header("Catálogo API")]
    public int idTema;
    public int idCheckpoint;

    [Header("Evento")]
    public int puntajeActual = 0;
    public float vidaActual = 0;
    public float energiaActual = 0;

    private bool registrado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (registrado) return;

        if (!other.CompareTag("Player")) return;

        registrado = true;

        if (BoltApiService.Instance == null)
        {
            Debug.LogWarning("BoltApiService no existe en escena.");
            return;
        }

        BoltApiService.Instance.RegistrarCheckpoint(idTema, idCheckpoint);

        BoltApiService.Instance.RegistrarEventoPartida(
            idTema: idTema,
            idCheckpoint: idCheckpoint,
            tipoEvento: "Checkpoint",
            puntajeActual: puntajeActual,
            vidaActual: vidaActual,
            energiaActual: energiaActual
        );

        Debug.Log($"Checkpoint registrado en API: {idCheckpoint}");
    }
}