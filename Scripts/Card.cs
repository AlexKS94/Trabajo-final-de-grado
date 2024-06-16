
using UnityEngine;

//Incorporo en el panel de unity la creaci�n de cartas lo cual me facilitar� el trabajo
[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public string cardName;             //Nombre de la carta
    public int coinCost;                //Coste de monedas
    public int numUnits;                //N�mero de unidades (se puede ampliar a que las cartas invoquen m�s unidades)
    public GameObject unitPrefab;       //La unidad que invoca
    public Sprite cardImage;            //Imagen para la carta
}

