using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitConfirmationMenu : MonoBehaviour
{

    //Boton TIC VERDE
    public void QuitGameConfirmationPositive()
    {
        //Cerramos de la aplicación
        Application.Quit();
    }

    //Boton CRUZ ROJA
    public void QuitGameConfirmationNegative()
    {
        //Cargamos el MainMenu
        SceneManager.LoadSceneAsync(0);
    }
}
