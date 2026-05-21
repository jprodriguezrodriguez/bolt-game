using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("GameStartManager iniciado.");

        if (BoltApiService.Instance != null)
        {
            Debug.Log("BoltApiService encontrado. Creando usuario aleatorio...");
            BoltApiService.Instance.CrearUsuarioAleatorio();
        }
        else
        {
            Debug.LogError("BoltApiService no está disponible.");
        }
    }
}