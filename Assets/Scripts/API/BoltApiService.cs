using System;
using System.Collections;
using UnityEngine;

public class BoltApiService : MonoBehaviour
{
    public static BoltApiService Instance { get; private set; }

    private const string PlayerPrefIdUsuario = "BOLT_API_ID_USUARIO";
    private const string PlayerPrefAliasUsuario = "BOLT_API_ALIAS_USUARIO";
    private const string PlayerPrefCodigoSesion = "BOLT_API_CODIGO_SESION";

    public int IdUsuarioActual { get; private set; }
    public string AliasUsuarioActual { get; private set; } = string.Empty;
    public string CodigoSesionActual { get; private set; } = string.Empty;

    public int TemaActualId { get; private set; }
    public int? UltimoCheckpointId { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CargarUsuarioGuardado();
    }

    private void CargarUsuarioGuardado()
    {
        if (PlayerPrefs.HasKey(PlayerPrefIdUsuario))
        {
            IdUsuarioActual = PlayerPrefs.GetInt(PlayerPrefIdUsuario);
            AliasUsuarioActual = PlayerPrefs.GetString(PlayerPrefAliasUsuario, string.Empty);
            CodigoSesionActual = PlayerPrefs.GetString(PlayerPrefCodigoSesion, string.Empty);

            Debug.Log($"Usuario API cargado desde PlayerPrefs. ID: {IdUsuarioActual}, Alias: {AliasUsuarioActual}");
        }
    }

    public void CrearUsuarioAleatorio()
    {
        if (IdUsuarioActual > 0)
        {
            Debug.Log($"Ya existe usuario API activo. ID: {IdUsuarioActual}, Alias: {AliasUsuarioActual}");
            return;
        }

        Debug.Log("No hay usuario API activo. Creando usuario aleatorio...");
        StartCoroutine(CrearUsuarioCoroutine());
    }

    private IEnumerator CrearUsuarioCoroutine()
    {
        string randomCode = Guid.NewGuid().ToString();

        UsuarioFormDto usuario = new UsuarioFormDto
        {
            aliasUsuario = $"BOLT_{UnityEngine.Random.Range(1000, 9999)}",
            codigoSesion = randomCode
        };

        yield return ApiClient.Instance.Post<UsuarioFormDto, UsuarioResponseDto>(
            "Usuarios",
            usuario,
            response =>
            {
                if (response == null)
                {
                    Debug.LogError("La API no devolvió información del usuario creado.");
                    return;
                }

                IdUsuarioActual = response.idUsuario;
                AliasUsuarioActual = response.aliasUsuario;
                CodigoSesionActual = response.codigoSesion;

                PlayerPrefs.SetInt(PlayerPrefIdUsuario, IdUsuarioActual);
                PlayerPrefs.SetString(PlayerPrefAliasUsuario, AliasUsuarioActual);
                PlayerPrefs.SetString(PlayerPrefCodigoSesion, CodigoSesionActual);
                PlayerPrefs.Save();

                Debug.Log($"Usuario creado y guardado. ID: {IdUsuarioActual}, Alias: {AliasUsuarioActual}");
            },
            error =>
            {
                Debug.LogError("No se pudo crear el usuario en la API: " + error);
            }
        );
    }

    public void CrearNuevaSesion()
    {
        PlayerPrefs.DeleteKey(PlayerPrefIdUsuario);
        PlayerPrefs.DeleteKey(PlayerPrefAliasUsuario);
        PlayerPrefs.DeleteKey(PlayerPrefCodigoSesion);
        PlayerPrefs.Save();

        IdUsuarioActual = 0;
        AliasUsuarioActual = string.Empty;
        CodigoSesionActual = string.Empty;

        Debug.Log("Sesión anterior eliminada. Creando nueva sesión...");
        CrearUsuarioAleatorio();
    }

    public void ActualizarNivelActual(int idTema, int? idCheckpointInicial = null)
    {
        TemaActualId = idTema;
        UltimoCheckpointId = idCheckpointInicial;

        Debug.Log($"Tema actual actualizado: {TemaActualId}, checkpoint actual: {UltimoCheckpointId}");
    }

    public void ActualizarCheckpointActual(int idTema, int idCheckpoint)
    {
        TemaActualId = idTema;
        UltimoCheckpointId = idCheckpoint;

        Debug.Log($"Checkpoint actual actualizado. Tema: {TemaActualId}, Checkpoint: {UltimoCheckpointId}");
    }

    public void RegistrarLogro(int idLogro, int cantidad = 1)
    {
        if (IdUsuarioActual <= 0)
        {
            Debug.LogWarning("No hay usuario actual para registrar logro.");
            return;
        }

        UsuarioLogroFormDto dto = new UsuarioLogroFormDto
        {
            idUsuario = IdUsuarioActual,
            idLogro = idLogro,
            cantidad = cantidad
        };

        StartCoroutine(ApiClient.Instance.Post<UsuarioLogroFormDto, UsuarioResponseDto>(
            "UsuariosLogros",
            dto,
            response =>
            {
                Debug.Log($"Logro registrado en API. Usuario: {IdUsuarioActual}, Logro: {idLogro}");
            },
            error =>
            {
                Debug.LogError("No se pudo registrar el logro: " + error);
            }
        ));
    }

    public void RegistrarCheckpoint(int idTema, int idCheckpoint)
    {
        if (IdUsuarioActual <= 0)
        {
            Debug.LogWarning("No hay usuario actual para registrar checkpoint.");
            return;
        }

        ActualizarCheckpointActual(idTema, idCheckpoint);

        UsuarioCheckpointFormDto dto = new UsuarioCheckpointFormDto
        {
            idUsuario = IdUsuarioActual,
            idCheckpoint = idCheckpoint,
            vecesActivado = 1
        };

        StartCoroutine(ApiClient.Instance.Post<UsuarioCheckpointFormDto, UsuarioCheckpointResponseDto>(
            "UsuariosCheckpoints",
            dto,
            response =>
            {
                Debug.Log($"Checkpoint registrado en API. Usuario: {IdUsuarioActual}, Checkpoint: {idCheckpoint}");
            },
            error =>
            {
                Debug.LogError("No se pudo registrar el checkpoint: " + error);
            }
        ));
    }

    public void RegistrarEventoPartida(
        int idTema,
        int? idCheckpoint,
        string tipoEvento,
        int puntajeActual,
        float vidaActual,
        float energiaActual
    )
    {
        if (IdUsuarioActual <= 0)
        {
            Debug.LogWarning("No hay usuario actual para registrar evento de partida.");
            return;
        }

        if (idTema <= 0)
        {
            Debug.LogWarning("No hay tema actual válido para registrar evento de partida.");
            return;
        }

        RegistroPartidaFormDto dto = new RegistroPartidaFormDto
        {
            idUsuario = IdUsuarioActual,
            idTema = idTema,
            idCheckpoint = idCheckpoint,
            tipoEvento = tipoEvento,
            puntajeActual = puntajeActual,
            vidaActual = vidaActual,
            energiaActual = energiaActual
        };

        StartCoroutine(ApiClient.Instance.Post<RegistroPartidaFormDto, RegistroPartidaResponseDto>(
            "RegistrosPartida",
            dto,
            response =>
            {
                Debug.Log($"Evento registrado en API: {tipoEvento}. Usuario: {IdUsuarioActual}, Tema: {idTema}, Checkpoint: {idCheckpoint}");
            },
            error =>
            {
                Debug.LogError("No se pudo registrar el evento de partida: " + error);
            }
        ));
    }

    public void RegistrarMuerte(int puntajeActual, float vidaActual, float energiaActual)
    {
        if (TemaActualId <= 0)
        {
            Debug.LogWarning("No se puede registrar muerte porque TemaActualId no está configurado.");
            return;
        }

        RegistrarEventoPartida(
            idTema: TemaActualId,
            idCheckpoint: UltimoCheckpointId,
            tipoEvento: "Muerte",
            puntajeActual: puntajeActual,
            vidaActual: vidaActual,
            energiaActual: energiaActual
        );
    }
}