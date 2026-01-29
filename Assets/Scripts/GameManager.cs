using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject pauseCanvas;
    bool paused;

    public static readonly List<string> sceneList = new()
    {
        "Menu",
        "Level1",
        "Level2",
        "CreditScene"
    };

    public void loadScene(string sceneName)
    {
        StartCoroutine(Instance.FadeOut(() => SceneManager.LoadSceneAsync(sceneName)));
    }

    public static int difficulty = 2;
    public static int seeds = 0;
    public static int feathers = 0;

    public Image overlayImage;

    void Awake()
    {
        SceneManager.sceneLoaded += onSceneLoaded;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Level1" && SceneManager.GetActiveScene().name != "Level2")
            return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    void onSceneLoaded(Scene  scene, LoadSceneMode mode)
    {
        if (this == null)
            return;
        
        AudioManager.Instance.PlayMusic(AudioManager.Instance.startMusic);
        StartCoroutine(FadeIn(() => { }));
    }

    float t;
    
    public IEnumerator FadeOut(Action postFadeAction)
    {
        while (t < 1.0f)
        {
            t += Time.deltaTime / 2.0f;
            var color = overlayImage.color;
            color.a = Mathf.Clamp01(t);
            overlayImage.color = color;
            yield return null;
        } 
        
        postFadeAction();
    }

    public IEnumerator FadeIn(Action postFadeAction)
    {
        while (t > 0.0f)
        {
            t -= Time.deltaTime / 2.0f;
            var color = overlayImage.color;
            color.a = Mathf.Clamp01(t);
            overlayImage.color = color;
            yield return null;
        }
        
        postFadeAction();
    }

    public void Pause()
    {
        pauseCanvas.SetActive(!pauseCanvas.activeSelf);
        paused = !paused;
        
        if (paused)
        {
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
