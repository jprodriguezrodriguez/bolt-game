using UnityEngine;

public class ClearPlayerPrefsTest : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("PlayerPrefs limpiado para prueba.");
    }
}