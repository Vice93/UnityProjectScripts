using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {

    public AudioClip menuTheme;
    public AudioClip level1Theme;
    public AudioClip level2Theme;
    public AudioClip creditsTheme;
    private string sceneName;


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;

        if (sceneName == "HorrorGame_Menu")
        {
            AudioManager.instance.PlayMusic(menuTheme, 1);
        }
        else if (sceneName == "HorrorGame")
        {
            AudioManager.instance.PlayMusic(level1Theme, 1);
        }
        else if (sceneName == "HorrorGame_Level2")
        {
            AudioManager.instance.PlayMusic(level2Theme, 1);
        }
        else if (sceneName == "HorrorGame_Credits")
        {
            AudioManager.instance.PlayMusic(creditsTheme, 1);
        }
        
    }
}
