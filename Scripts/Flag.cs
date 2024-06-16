using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Flag : MonoBehaviour
{
    //variables
    public GameEnums.Team Team;                             //equipo
    public int maxHealth = 250;                             //Vida máxima
    private int currentHealth;                              //Vida actual
    public GameObject blueHealthBarPrefab;                  //Barra de vida equipo azul
    public GameObject redHealthBarPrefab;                   //Barra de vida equipo rojo
    private HealthBar healthBar;                            //Barra de vida

    //Función que se ejecuta al inicio
    private void Start()
    {
        //Almacenamos el valor de la vida máxima en la vida actual
        currentHealth = maxHealth;
        // Instanciar el prefab de la barra de vida según el tag
        GameObject healthBarInstance;
        if (gameObject.CompareTag("BlueFlag"))
        {
            healthBarInstance = Instantiate(blueHealthBarPrefab, transform);
        }
        else 
        {
            healthBarInstance = Instantiate(redHealthBarPrefab, transform);
        }
        //Inicializamos el componente de barra de vida con el valor de la vida actual / vida máxima 
        healthBar = healthBarInstance.GetComponent<HealthBar>();
        healthBar.Initialize(currentHealth); 
    }

    //Función para recibir daño
    public void TakeDamage(int damage)
    {
        //Disminuimos la salud actual
        currentHealth -= damage;
        //Si es inferior o igual a 0 destruirmo llamamos a la función de destrucción de bandera
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            DestroyFlag();
        }
        //Actualizamos la salud de la bandera.
        UpdateHealthBar();
    }
    
    //Método para atualizar la barra de vida
    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
    }
    
    //Función para destruir la bandera
    void DestroyFlag()
    {
        GameManager.Instance.FlagDestroyed(Team); 
        transform.Find("Flag").gameObject.SetActive(false);
    }
}
