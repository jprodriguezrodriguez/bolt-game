using UnityEngine;

public class LevelApiInitializer : MonoBehaviour
{
    [Header("Configuración del nivel")]
    public int idTema;
    public int? idCheckpointInicial;

    [Header("Evento inicial")]
    public bool registrarInicioNivel = true;

    private void Start()
    {
        if (BoltApiService.Instance == null)
        {
            Debug.LogWarning("BoltApiService no está disponible.");
            return;
        }

        BoltApiService.Instance.ActualizarNivelActual(idTema, idCheckpointInicial);

        Debug.Log($"Nivel API inicializado. Tema: {idTema}, Checkpoint inicial: {idCheckpointInicial}");

        if (registrarInicioNivel)
        {
            BoltApiService.Instance.RegistrarEventoPartida(
                idTema: idTema,
                idCheckpoint: idCheckpointInicial,
                tipoEvento: "InicioNivel",
                puntajeActual: 0,
                vidaActual: 4,
                energiaActual: 4
            );
        }
    }
}