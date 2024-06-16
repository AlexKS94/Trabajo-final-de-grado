using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonSoundManager : MonoBehaviour
{
    public AudioClip buttonSoundClick;      //Elemento de audio
    private AudioSource audioSource;        //Elemento de unity para controlar la reproducción de sonidos en el juego

    //Se compone de 2 funciones start y playEffect
    void Start()
    {
        //añadimos el componente a la escena 
        audioSource = gameObject.AddComponent<AudioSource>();
        //se indica que no debe ejecutarse al inicializarse
        audioSource.playOnAwake = false;
        //se asigna la pista de sonido
        audioSource.clip = buttonSoundClick;

        // Asumimos que el script se añade al botón, obtener el componente Button
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlayEffect);
        }
    }

    //
    void PlayEffect()
    {
        audioSource.PlayOneShot(buttonSoundClick);
    }
}
