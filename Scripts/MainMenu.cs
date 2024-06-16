using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    //Funciones seg�n el boton:
    //Se carga la escena seg�n indice seg�n Build Settings
    //Boton PLAY
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1); 
    }

    //Boton QUIT
    public void QuitGame()
    {
        SceneManager.LoadSceneAsync(3);
    }

    //Boton COLLECTION
    public void Collection()
    {
        SceneManager.LoadSceneAsync(2);
    }

    //Boton SETTINGS
    /* Se comenta ya que se realizar� en futuras mejoras al trabajo final de grado
    public void Settings()
    {
        SceneManager.LoadSceneAsync(4);
    }
    */

}

