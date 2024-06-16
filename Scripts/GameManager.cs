using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameEnums;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int maxCoins = 10;                   //N�mero m�ximo de monedas
    public int currentCoins;                    //N�mero actual de monedas
    public float coinCooldown = 5f;             //Tiempo de cooldown de las monedas, por defecto 5
    public TextMeshProUGUI coinText;            //Referencia al texto de monedas en el Canvas

    public GameObject redWorkerPrefab;          //Obrero orco   equipo rojo
    public GameObject blueWorkerPrefab;         //Obrero humano equipo azul
    public List<GameObject> enemyUnits;         //Lista unidades enemigas
    public float workerSpawnInterval = 15f;     //Intervalo de spawn de workers de cada equipo, por defecto 15
    public float enemySpawnInterval = 25f;      //Intervalo spawn unidades enemigas, por defecto 25f (5 monedas)
    
    private Transform orcUnitsParent;           //posici�n donde insertar el prefab en la jerarquia
    private Transform humanUnitsParent;         //posici�n donde insertar el prefab en la jerarquia

    //botones menu en juego
    public Button pauseButton;                  //Boton para abrir el menu de pausa
    public GameObject pauseMenu;                //Panel de menu de pausa
    public Button resumeButton;                 //Boton volver al juego
    public Button mainMenuButton;               //Boton volver al menu principal

    //Juego pausado
    private bool isPaused = false;

    //Mientras esta 
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Hacer que este objeto persista entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //Asignamos los botones
        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        //Desactivamos el menu de pausa
        pauseMenu.SetActive(false);

        //Asignamos donde queremos que se a�adan en la jerarquia
        orcUnitsParent = GameObject.Find("Units/OrcUnits").transform;
        humanUnitsParent = GameObject.Find("Units/HumanUnits").transform;
        //Actualizamos la UI que muestra la informaci�n sobre las monedas
        UpdateCoinUI();
        //Inicializazmos la corutinas de generaci�n de recursos, invocaci�n de obreros e invocaci�n de unidades enemigas.
        StartCoroutine(GenerateCoins());
        StartCoroutine(SpawnWorkers());
        StartCoroutine(SpawnEnemyUnits());
    }

    //Funci�n para aumentar los recursos
    public void AddCoin()
    {
        //Si no se supera el n�mero m�ximo de monedas
        if (currentCoins < maxCoins)
        {
            //Aumentamos las monedas actuales
            currentCoins++;
            //Llamamos a la funci�n de actualizaci�n de la interfaz visual con los recursos
            UpdateCoinUI();
        }
    }
    //Funci�n para disminuir el n�mero de recursos
    public bool UseCoins(int amount)
    {
        //Comprueba que la cantidad de recursos que se quiere decrementar sea mayor al valor de recursos actual
        if (currentCoins >= amount)
        {
            //disminuye los recursos actuales
            currentCoins -= amount;
            //Llama a actualizar la UI de los recursos
            UpdateCoinUI();
            return true;
        }
        return false;
    }

    //Funci�n que modifica el valor del texto en el canvas del nivel
    void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
    }

    //Funci�n privada para la generaci�n de monedas a lo largo de la partida comprobando que la partida no este pausada
    private IEnumerator GenerateCoins()
    {
        while (true)
        {
            //Si pausa esta en false
            if (!isPaused)
            {
                //generamos moneda en funci�n del tiempo de coinCooldown
                yield return new WaitForSeconds(coinCooldown);
                AddCoin();
            }
            else
            {
                yield return null; 
            }
        }
    }

    //Generaci�n obreros ambos equipos
    private IEnumerator SpawnWorkers()
    {
        //Llamadas a la funci�n que invoca a los obreros
        //al inicio de la partida
        SpawnWorker(Team.Rojo);
        SpawnWorker(Team.Azul);

        //Invocaci�n de obreros a lo largo de la partida, comprobando que el juego no este pausado y en un intervalo adecuado
        while (true)
        {
            if (!isPaused)
            {
                //Esperamos el cd de invocaci�n
                yield return new WaitForSeconds(workerSpawnInterval);
                //Llamamos a la funci�n de invocaci�n para ambos equipos
                SpawnWorker(Team.Rojo);
                SpawnWorker(Team.Azul);
            }
            else
            {
                yield return null; 
            }
        }
    }

    //Funci�n de invocaci�n
    private void SpawnWorker(Team team)
    {
        //Seg�n el equipo seleccionamos el prefabricado correcto de la unidad
        GameObject workerPrefab = (team == Team.Rojo) ? redWorkerPrefab : blueWorkerPrefab;
        //Guardamos el punto de invocaci�n de la unidad en el mapa
        Transform spawnPoint = GetSpawn(team);
        if (spawnPoint != null)
        {
            //Invocamos la unidad en la posici�n almacenada
            GameObject worker = Instantiate(workerPrefab, spawnPoint.position, Quaternion.identity);
            //Asignamos el padre correcto en la jerarqu�a
            Transform parentTransform = (team == Team.Rojo) ? orcUnitsParent : humanUnitsParent;
            worker.transform.SetParent(parentTransform, true);
        }
    }

    //Funci�n para obtener el spawnPoint de las unidades a trav�s del tag de la base 
    private Transform GetSpawn(Team team)
    {
        //Segun el equipo se almacenar� un tag u otro
        string tag = team == Team.Rojo ? "RedBase" : "BlueBase";
        //Buscamos el objeto
        GameObject baseObject = GameObject.FindGameObjectWithTag(tag);
        return baseObject?.transform;
    }

    //Coroutine para la invocaci�n spawn de unidades enemigas mientras el juego no este pausado
    //Con la finalidad de hacer el juego m�s impredecible y a�adir un factor de suerte
    //la unidad seleccionada para invocaci�n ser� aleatoria entre las posibles opci�nes.
    IEnumerator SpawnEnemyUnits()
    {
        while (true)
        {
            if (!isPaused)
            {
                int randomIndex = UnityEngine.Random.Range(0, enemyUnits.Count);    //Genera un n�mero aleatorio entre 0 y la cantidad de prefabs en la lista
                GameObject enemyUnitPrefab = enemyUnits[randomIndex];               //Seleccionar el prefabricado con el indice aleatorio
                yield return new WaitForSeconds(enemySpawnInterval);                //Esperar el cooldown
                SpawnUnitRedTeam(enemyUnitPrefab, GetSpawn(Team.Rojo));             //Invocar la unidad
            }
            else
            {
                yield return null; 
            }
        }
    }

    //Funci�n para spawnear una unidad del equipo rojo en la ubicaci�n de su base
    void SpawnUnitRedTeam(GameObject unitPrefab, Transform spawnPoint)
    {
        GameObject unit = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
        unit.transform.SetParent(orcUnitsParent, true); 
    }

    //Botones
    // M�todo para pausar y reanudar el juego
    private void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    // M�todo para reanudar el juego
    private void ResumeGame()
    {
        //Este boton tiene un trato especial, hay que reproducir el sonido antes de desactivar el Menu de pausa
        //Obtenemos el componente de sonido, y lo ejecutamos
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
        //Ejecutamos corrutina para el delay y desactivar el menu de pausa
        StartCoroutine(MenuPausaDeactivationDenlay());
        isPaused = false;
        Time.timeScale = 1;
    }

    //Funci�n para esperar despues de ejecutar el boton de ResumeGame y de esta manera tenga sonido
    private IEnumerator MenuPausaDeactivationDenlay()
    {
        //Esperar un peque�o momento para permitir que el sonido se reproduzca
        yield return new WaitForSeconds(0.1f);
        pauseMenu.SetActive(false);
    }

    // M�todo para volver al men� principal
    private void ReturnToMainMenu()
    {
        isPaused = false;           //Pone isPaused a false
        Time.timeScale = 1;         //Asegurarse de que el tiempo est� corriendo antes de volver al men�
        Destroy(gameObject);        //Destruir el GameManager al volver al "Main Menu"
        SceneManager.LoadScene(0);  //Cambia a la escena de "Main Menu"
    }


    //Funci�n que devuelve un booleano conforme el juego esta pausado o no
    public Boolean getIsPaused()
    {
        return isPaused;
    }

    //acabar la partida cuando la bandera es destruida
    //Funci�n para manejar la destrucci�n de banderas y cambio de escena
    public void FlagDestroyed(Team team)
    {
        if (team == Team.Azul)
        {
            Destroy(gameObject);
            SceneManager.LoadScene(4);
        }
        else if (team == Team.Rojo)
        {
            Destroy(gameObject);
            SceneManager.LoadScene(5);
        }
    }
}



