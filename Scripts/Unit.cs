
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static GameEnums;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class Unit : MonoBehaviour
{
    //variables de la unidad
    public Team team;                       //equipo de la unidad
    public int health;                      //vida
    public int attackDamage;                //daño
    public int armor;                       //armadura 
    public int armorPenetration;            //penetración de armadura
    public float attackRange;               //rango de ataque
    public float attackCooldown = 5.0f;     //tiempo entre ataques (1s) por defecto
    private float lastAttackTime;           //tiempo del último ataque
   
    //Objetos que necesito para las cordenadas del Navmesh
    public LayerMask enemyLayer;            //Layer unidad enemiga    
    public Flag enemyFlag;                  //Bandera enemiga para atacar
    private Unit currentEnemy;              //Unidad enemiga
    private NavMeshAgent agent;             //Agente de navMeshAgent

    private Animator animator;              //Componente Animator
    
    //Escalado de las unidades para suprimir el efecto de profundidad
    private Vector3 initialScale;           //escala inicial
    private Vector3 finalScale;             //escala final
    private Vector3 initialPosition;        //posición inicial para calcular la distancia máxima entre las banderas


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();               //Obtenemos el navMeshAgent                         
        lastAttackTime = Time.time - attackCooldown;        //Obtenemos el tiempo y le restamos el tiempo entre ataque para obtener el tiempo del último ataque
        animator = GetComponent<Animator>();                //Obtenemos Animator para las animaciones
        
        //si estamos el rango del agente es mayor que el de nuestra unidad lo modificacos
        if (attackRange <= agent.radius)
        {
            attackRange = attackRange + agent.radius; // Incrementar ligeramente el rango de ataque
        }
        //Dependiento del equipo marcaremos la bandera enemiga como la azul o la roja
        if (team == Team.Rojo)
        {
            //Guardar la bandera enemiga 
            enemyFlag = GameObject.FindGameObjectWithTag("BlueFlag").GetComponent<Flag>(); 
            //Escalado de unidades
            initialScale = new Vector3(2f, 10f, 2f);                                        
            finalScale = new Vector3(3f, 40f, 3f);
        }
        else
        {
            //Equipo Azul
            //Guardar la bandera enemiga 
            enemyFlag = GameObject.FindGameObjectWithTag("RedFlag").GetComponent<Flag>();
            //Escalado de unidades
            initialScale = new Vector3(3f, 40f, 3f);
            finalScale = new Vector3(2f, 10f, 2f);
        }
        
        initialPosition = transform.position;
        //Transformación escala unidad a initial
        transform.localScale = initialScale;
    }

    //Método que se irá ejecutando a lo largo de la vida de la unidad
    void Update()
    {
        //actualizamos la animación en función de triggers
        UpdateAnimation();
        //Buscamos enemigos
        if (currentEnemy == null)
        {
            CheckForEnemies();
        }
        //Este condicional es muy importante. Si hay enemigo y estamos a rango los atacaremos
        //Si hay enemigos pero no estamos a rango se dirigirá hacia la unidad enemiga y la unidad rotará para apuntar al enemigo
        //Si no hay unidad enemiga, en función del rango la unidad se dirigirá a ella o empezará a atacarla.
        if (currentEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, currentEnemy.transform.position);
            if (distanceToEnemy <= attackRange)
            {
                agent.isStopped = true;
                AttackEnemy(currentEnemy);
            }
            else
            {
                agent.isStopped = false;
                //Almacenamos la posición intermedia entre ambas unidades que es donde se atacaran
                Vector3 midPoint = (transform.position + currentEnemy.transform.position) / 2;
                agent.SetDestination(midPoint);
            }
        }
        else
        {
            
            float distanceToFlag = Vector3.Distance(transform.position, enemyFlag.transform.position);
            if (distanceToFlag <= attackRange)
            {
                agent.isStopped = true;
                AttackFlag(enemyFlag);
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(enemyFlag.transform.position);
                AdjustRotation();
            }
        }
        AdjustScale();
    }


    //Buscar enemigos
    void CheckForEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
        foreach (Collider hitCollider in hitColliders)
        {
            Unit potentialEnemy = hitCollider.GetComponent<Unit>();
            if (potentialEnemy != null && potentialEnemy.team != this.team)
            {
                currentEnemy = potentialEnemy;
                return;
            }
        }
        currentEnemy = null;
    }

    //Función para atacar a los enemigos
    void AttackEnemy(Unit enemy)
    {
        animator.ResetTrigger("IsWalking");
        if (Time.time > lastAttackTime + attackCooldown)
        {
            AdjustRotationAtEnemy(enemy.transform);
            animator.SetTrigger("Attack");
            int damageDealt = attackDamage + armorPenetration;
            StartCoroutine(animationTime());
            enemy.TakeDamage(damageDealt);
            lastAttackTime = Time.time;
        }
    }
    
    //Función para atacar la bandera enemiga
    void AttackFlag(Flag flag)
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {
            AdjustRotationAtEnemy(flag.transform);
            animator.SetTrigger("Attack");
            StartCoroutine(animationTime());
            flag.TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }
    }

    //Función para esperar antes de cambiar de animación ya que se ejecuta toda la animación
    private IEnumerator animationTime()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length/2); //Para la rutina hasta que se completa la animación
    }

    //Ajuste de rotación
    void AdjustRotation()
    {
        if (agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            Vector3 direction = agent.velocity.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    //Ajuste de rotación
    void AdjustRotationAtEnemy(Transform enemy)
    {
        Vector3 direction = (enemy.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    //Función para ajustar la escala en función de la distancia respecto a la base
    void AdjustScale()
    {
        float maxDistance = Vector3.Distance(initialPosition, enemyFlag.transform.position);
        float distanceToEnemyBase = Vector3.Distance(transform.position, enemyFlag.transform.position);
        float t = distanceToEnemyBase / maxDistance;

        if (this.team == GameEnums.Team.Rojo)
        {
            this.transform.localScale = Vector3.Lerp(initialScale, finalScale, t);
        }
        else
        {
            this.transform.localScale = Vector3.Lerp(initialScale, finalScale, t);
        }
    }

    //Función para cambiar de estado en el Animator entre IsWalking a true o false
    void UpdateAnimation()
    {
        if (agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
        
    }

    //Función recibir daño
    public void TakeDamage(int damage)
    {
        //Si tiene armadura superior a 0 y mas armadura que daño entrante
        if (armor > 0 && armor >= damage)
        {
            //restamos el daño a la armadura
            armor -= damage;
        }
        //Si tiene armadura superior a 0 y menos armadura que daño entrante
        else if (armor > 0 && armor < damage)
        {
            //restamos a la salud el daño menos la armadura
            health -= (damage - armor);
            //la armadura pasa a ser 0
            armor = 0;
        }
        //si la armadura es 0
        else if (armor == 0)
        {
            //restamos el daño a la vida
            health -= damage;
        }
        //si la vida es menor que 0 se muere
        if (health <= 0)
        {
            Die();
        }
        //en caso de que sea mayor que 0
        else
        {
            //habilitamos la animacion de tomar daño TakeDamage
            animationTime();
            animator.SetTrigger("TakeDamage");
        }
    }

    //Morir
    void Die()
    {
        agent.isStopped = true;                         // Detenemos el movimiento
        animator.SetTrigger("Die");                     //Activamos el trigger Die y empezamos la corutina de eliminación de la unidad.
        StartCoroutine(DeleteAfterDeathAnimation());    //Llamamos a la corutina
    }

    // Corutina para remover la unidad después de la animación de muerte
    IEnumerator DeleteAfterDeathAnimation()
    {
        // Esperar el tiempo de la animación de muerte
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }
}

