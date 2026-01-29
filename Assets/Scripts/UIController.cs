using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject mainLayout;
    public GameObject optionsLayout;
    public AudioClip buttonClick;

    public Text difficultyText;
    
    public void start()
    {
        if (SceneManager.GetActiveScene().name == GameManager.sceneList[0])
        {
            AudioManager.Instance.PlaySFX(buttonClick, 0.6f);
            GameManager.Instance.loadScene(GameManager.sceneList[1]);
        }
    }

    public void options()
    {
        mainLayout.SetActive(!mainLayout.activeSelf);
        optionsLayout.SetActive(!optionsLayout.activeSelf);
        String difficulty=GameManager.difficulty.ToString();
        switch (difficulty) {
            case "1":
                difficultyText.text = "Easy";
                break;
            case "2":
                difficultyText.text = "Medium";
                break;
            case "3":
                difficultyText.text = "Hard";
                break;
            default:
                difficultyText.text = "Difficulty " + difficulty;
                break;
        }
        AudioManager.Instance.PlaySFX(buttonClick, 0.6f);
    }

    public void setDifficulty(int inc)
    {
        GameManager.difficulty += inc;
        GameManager.difficulty = Mathf.Clamp(GameManager.difficulty, 1, 3);
        String difficulty=GameManager.difficulty.ToString();
        switch (difficulty) {
            case "1":
                difficultyText.text = "Easy";
                break;
            case "2":
                difficultyText.text = "Medium";
                break;
            case "3":
                difficultyText.text = "Hard";
                break;
            default:
                difficultyText.text = "Difficulty " + difficulty;
                break;
        }
        AudioManager.Instance.PlaySFX(buttonClick, 0.6f);
    }

    public void exit()
    {
        AudioManager.Instance.PlaySFX(buttonClick, 0.6f);
        Application.Quit();
    }
}
