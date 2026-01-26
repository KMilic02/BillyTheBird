using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject mainLayout;
    public GameObject optionsLayout;

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
    }

    public void setDifficulty(int inc)
    {
        GameManager.difficulty += inc;
        GameManager.difficulty = Mathf.Clamp(GameManager.difficulty, 1, 3);
        
        difficultyText.text = "Difficulty " + GameManager.difficulty.ToString();
    }

    public void exit()
    {
        Application.Quit();
    }
}
