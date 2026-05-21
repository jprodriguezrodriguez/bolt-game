using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    public static ApiClient Instance { get; private set; }

    private string BaseUrl => ApiConfig.Instance.BaseUrl;

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

    public IEnumerator Post<TRequest, TResponse>(
        string endpoint,
        TRequest data,
        Action<TResponse> onSuccess,
        Action<string> onError = null
    )
    {
        string url = $"{BaseUrl}/{endpoint}";
        string json = JsonUtility.ToJson(data);

        using UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseJson = request.downloadHandler.text;

            if (!string.IsNullOrWhiteSpace(responseJson))
            {
                TResponse response = JsonUtility.FromJson<TResponse>(responseJson);
                onSuccess?.Invoke(response);
            }
            else
            {
                onSuccess?.Invoke(default);
            }
        }
        else
        {
            string error = $"Error API: {request.responseCode} - {request.error}\n{request.downloadHandler.text}";
            Debug.LogError(error);
            onError?.Invoke(error);
        }
    }
}