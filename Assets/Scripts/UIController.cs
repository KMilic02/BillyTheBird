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
            GameManager.loadScene(GameManager.sceneList[1]);
        }
    }

    public void options()
    {
        mainLayout.SetActive(!mainLayout.activeSelf);
        optionsLayout.SetActive(!optionsLayout.activeSelf);
        difficultyText.text = "Difficulty " + GameManager.difficulty.ToString();
        AudioManager.Instance.PlaySFX(buttonClick, 0.6f);
    }

    public void setDifficulty(int inc)
    {
        GameManager.difficulty += inc;
        GameManager.difficulty = Mathf.Clamp(GameManager.difficulty, 1, 3);
        
        difficultyText.text = "Difficulty " + GameManager.difficulty.ToString();
        AudioManager.Instance.PlaySFX(buttonClick, 0.6f);
    }

    public void exit()
    {
        AudioManager.Instance.PlaySFX(buttonClick, 0.6f);
        Application.Quit();
    }
}
