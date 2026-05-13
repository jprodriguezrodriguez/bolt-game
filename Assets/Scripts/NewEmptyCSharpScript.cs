using UnityEngine;
using UnityEngine.Playables;

public class PlayCinematica : MonoBehaviour
{
    public PlayableDirector director; // Arrastra tu objeto Cinematica aquí

    void Start()
    {
        director.Play(); // O llámalo cuando quieras
    }
}