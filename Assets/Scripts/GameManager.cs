using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static readonly List<string> sceneList = new()
    {
        "Menu",
        "SampleScene"
    };

    public static void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static int difficulty = 2;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
