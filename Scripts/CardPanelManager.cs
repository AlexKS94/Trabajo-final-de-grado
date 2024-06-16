using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameEnums;

public class CardPanelManager : MonoBehaviour
{
    public Transform cardPanel;             // Panel donde se mostrar�n las cartas
    public GameObject cardButtonPrefab;     // Prefab del bot�n de la carta
    public Card[] playerCards;              // Lista de cartas del jugador
    public Transform[] spawnPoints;         // Puntos de aparici�n de unidades

    private List<GameObject> instantiatedButtons = new List<GameObject>(); // Lista de botones instanciados

    void Start()
    {
        //Creamos los botones de las cartas la funci�n CreateCardButtons()
        CreateCardButtons();
    }

    //Funci�n para crear los botones de las cartas
    private void CreateCardButtons()
    {
        //Crearemos un boton en el panel de cartas para cada carta en la lista
        foreach (Card card in playerCards)
        {
            GameObject button = Instantiate(cardButtonPrefab, cardPanel);
            Button btnComponent = button.GetComponent<Button>();
            btnComponent.onClick.AddListener(() => OnCardButtonClicked(card));
            // Asigna la imagen de la carta al bot�n
            Image btnImage = button.GetComponent<Image>();
            btnImage.sprite = card.cardImage;
            instantiatedButtons.Add(button);
        }
    }
    
    //Funci�n de comportamiento de los botones (cartas)
    private void OnCardButtonClicked(Card card)
    {
        //Comprobamos coste y si el juego esta en pausa
        if (!GameManager.Instance.getIsPaused() && GameManager.Instance.UseCoins(card.coinCost))
        {
            //Realizamos la corutina de spawn de la unidad de la cata
            StartCoroutine(SpawnUnit(card));
        }
        
    }

    //Invocaci�n las unidades
    private IEnumerator SpawnUnit(Card card)
    {
            //Obtengo el spawn
            Transform spawnPoint = GetSpawnPoint();
            if (spawnPoint != null)
            {
                GameObject unit = Instantiate(card.unitPrefab, spawnPoint.position, Quaternion.identity);
                //Colocar las unidades en la jerarqu�a correcta
                Transform parentTransform = GameObject.Find("Units/HumanUnits").transform;
                unit.transform.SetParent(parentTransform);
                //Espera de 1 segundos entre cada aparici�n
                yield return new WaitForSeconds(1f); 
            }
    }

    //Obtener la posici�n del spawn del equipo azul
    private Transform GetSpawnPoint()
    {
        GameObject baseObject = GameObject.FindGameObjectWithTag("BlueBase");
        return baseObject?.transform;
    }
}
