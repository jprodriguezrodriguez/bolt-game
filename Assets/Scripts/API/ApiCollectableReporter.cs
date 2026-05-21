using UnityEngine;

public class ApiCollectableReporter : MonoBehaviour
{
    [Header("Catálogo API")]
    public int idTema;
    public int idLogro;

    [Header("Checkpoint asociado")]
    public bool usarCheckpoint = true;
    public int idCheckpointAsociado;

    [Header("Puntaje")]
    public int puntajeAlRecolectar = 100;

    private bool registrado = false;

    public void RegistrarRecoleccion()
    {
        if (registrado) return;
        registrado = true;

        if (BoltApiService.Instance == null)
        {
            Debug.LogWarning("BoltApiService no existe en escena.");
            return;
        }

        int? checkpointParaEnviar = usarCheckpoint ? idCheckpointAsociado : null;

        BoltApiService.Instance.RegistrarLogro(idLogro, 1);

        BoltApiService.Instance.RegistrarEventoPartida(
            idTema: idTema,
            idCheckpoint: checkpointParaEnviar,
            tipoEvento: idTema == 4 ? "EvaluacionMaterial" : "RecolectoMaterial",
            puntajeActual: puntajeAlRecolectar,
            vidaActual: 0,
            energiaActual: 0
        );

        Debug.Log($"Material registrado en API. Tema: {idTema}, Logro: {idLogro}, Checkpoint: {checkpointParaEnviar}");
    }
}