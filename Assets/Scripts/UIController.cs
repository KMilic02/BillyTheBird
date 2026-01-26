using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public void start()
    {
        if (SceneManager.GetActiveScene().name == GameManager.sceneList[0])
        {
            GameManager.loadScene(GameManager.sceneList[1]);
        }
    }

    public void options()
    {
        
    }

    public void exit()
    {
        Application.Quit();
    }
}
