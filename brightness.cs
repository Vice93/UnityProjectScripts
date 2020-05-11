using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class brightness : MonoBehaviour {

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
            RenderSettings.ambientIntensity = PlayerPrefs.GetFloat("brightness");
        }
        else if (sceneName == "HorrorGame")
        {
            RenderSettings.ambientIntensity = PlayerPrefs.GetFloat("brightness");
        }
        else if (sceneName == "HorrorGame_Level2")
        {
            RenderSettings.ambientIntensity = PlayerPrefs.GetFloat("brightness");
        }
    }
}
