using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardCollection : MonoBehaviour
{
    public List<Card> playerCards = new List<Card>();

    //Metodo Instance que se utilizará para obtener la colección de cartas
    //Patrón Singleton
    public static PlayerCardCollection Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //Evitamos que la collección de cartas se destruya entre escenas
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
