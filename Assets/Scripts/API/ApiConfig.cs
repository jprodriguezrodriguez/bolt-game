using UnityEngine;

public class ApiConfig : MonoBehaviour
{
    public static ApiConfig Instance { get; private set; }

    [Header("API Configuration")]
    [SerializeField] private string baseUrl = "https://localhost:7264/api";

    public string BaseUrl => baseUrl;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}