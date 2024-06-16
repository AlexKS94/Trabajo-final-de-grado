using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CollectionMenu : MonoBehaviour
{
    public Transform collectionPanel;      // Panel donde se mostrar� la carta
    public GameObject cardButtonPrefab;     // Prefab del bot�n de la carta
    public Button nextCardButton;           // Bot�n para ir a la siguiente carta
    public Button previousCardButton;       // Bot�n para ir a la carta anterior
    public Button backButton;               // Bot�n para volver al men� principal

    public Card[] playerCards;              // Array de cartas del jugador          

    private int currentIndex = 0;           // �ndice actual de la carta que se muestra

    private Image cardImage;                // Referencia a la imagen de la carta en el centro

    void Start()
    {
        //Colecci�n de cartas del jugador
        cardImage = collectionPanel.Find("ImagenCarta").GetComponent<Image>();           // Obtener la referencia a la imagen donde situaremos la carta

        //Poner los eventos a los botones
        nextCardButton.onClick.AddListener(NextCard);
        previousCardButton.onClick.AddListener(PreviousCard);
        backButton.onClick.AddListener(VolverMainMenu);

        // Mostrar la primera carta
        UpdateCardDisplay();
    }

    void UpdateCardDisplay()
    {
       
        // Instanciar la carta actual
        if (playerCards.Length > 0)
        {
            Card currentCard = playerCards[currentIndex];
            cardImage.sprite = currentCard.cardImage;        // Asignar la imagen de la unidad a la carta

        }
    }

    //Metodo para mostrar la carta siguiente
    void NextCard()
    {
        if (currentIndex < playerCards.Length - 1)
        {
            currentIndex++;
            UpdateCardDisplay();
        }
    }

    //M�todo para mostrar la carta anterior
    void PreviousCard()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateCardDisplay();
        }
    }

    //Metodo para volver al Main Menu
    void VolverMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}

