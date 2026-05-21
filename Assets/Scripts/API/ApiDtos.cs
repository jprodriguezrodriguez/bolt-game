using System;

[Serializable]
public class UsuarioFormDto
{
    public string aliasUsuario;
    public string codigoSesion;
}

[Serializable]
public class UsuarioResponseDto
{
    public int idUsuario;
    public string aliasUsuario;
    public string codigoSesion;
    public string fechaCreacion;
    public string fechaActualizacion;
}

[Serializable]
public class UsuarioLogroFormDto
{
    public int idUsuario;
    public int idLogro;
    public int cantidad = 1;
}

[Serializable]
public class UsuarioLogroResponseDto
{
    public int idUsuarioLogro;
    public int idUsuario;
    public string aliasUsuario;
    public int idLogro;
    public string nombreLogro;
    public string nombreTipoLogro;
    public string nombreTema;
    public int cantidad;
    public string fechaObtencion;
}

[Serializable]
public class UsuarioCheckpointFormDto
{
    public int idUsuario;
    public int idCheckpoint;
    public int vecesActivado = 1;
}

[Serializable]
public class UsuarioCheckpointResponseDto
{
    public int idUsuarioCheckpoint;
    public int idUsuario;
    public string aliasUsuario;
    public int idCheckpoint;
    public string nombreCheckpoint;
    public string nombreTema;
    public int vecesActivado;
    public string fechaActivacion;
}

[Serializable]
public class RegistroPartidaFormDto
{
    public int idUsuario;
    public int idTema;
    public int? idCheckpoint;
    public string tipoEvento;
    public int puntajeActual;
    public float vidaActual;
    public float energiaActual;
}

[Serializable]
public class RegistroPartidaResponseDto
{
    public int idRegistroPartida;
    public int idUsuario;
    public string aliasUsuario;
    public int idTema;
    public string nombreTema;
    public int? idCheckpoint;
    public string nombreCheckpoint;
    public string tipoEvento;
    public int puntajeActual;
    public float vidaActual;
    public float energiaActual;
    public string fechaEvento;
}