using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatSceneManager : MonoBehaviour
{
    // Método para cargar la escena del menú principal
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(3);
    }
}
