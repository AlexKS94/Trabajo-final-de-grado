using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatSceneManager : MonoBehaviour
{
    // M�todo para cargar la escena del men� principal
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(3);
    }
}
