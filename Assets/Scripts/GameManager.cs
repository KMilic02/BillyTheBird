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
    public static int seeds = 0;
    public static int feathers = 0;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
