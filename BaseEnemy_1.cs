using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

//Author:  Benjamin Boese
//Date: 11-6-2019
//Purpose: For making a base enemy using inheritance and this script being the parent class
[System.Serializable]
public class BaseEnemy : MonoBehaviour
{
    [Space(5)]
    [Header("Nav Info")]
    [Tooltip("Drag the necessary Nav Points in this array")]
    [SerializeField]
    public Transform[] navPoints;//The array for the Nav points in the scene
    [Tooltip("This is just for reference only")]
    [SerializeField]
    protected int navIndex = 0;//the index for incrimenting the nav points

    [Space(5)]
    [Header("Spawned Bool")]
    [Tooltip("The bool for if the enemy has been spawned")]
    [SerializeField]
    protected bool enemySpawned = false;//The bool for if the enemy has spawned
    [Header("Enemy Dead Bool")]
    [Tooltip("The bool for if the enemy is dead or not")]
    protected bool enemyDead = false; //The bool for if the enemy is dead

    [Space(5)]
    [Header("Enemy Health")]
    [Tooltip("Set the current enemy health here")]
    [SerializeField]
    protected float curEnemyHealth;//The current health of the enemy
    [Tooltip("Set the max enemy health here and it should be the same number as the current enemy health")]
    [SerializeField]
    protected float maxEnemyHealth;//the max health of the enemy should be the same as the current health of the enemy
    [Tooltip("Put the enemy slider health bar here")]
    [SerializeField]
    protected Slider enemySlider;//the health slider goes here from the canvas

    [Space(5)]
    [Header("The prefab of the enemy")]
    [Tooltip("Put the enemy prefab here")]
    [SerializeField]
    protected GameObject enemyPrefab;//This is the enemy prefab

    [Space(5)]
    [Header("The nav mesh agent")]
    [Tooltip("Put the nav mesh agent component here")]
    [SerializeField]
    protected NavMeshAgent enemyNavAgent;//the enemy nav agent

    [Header("The enemy transfrom")]
    [Tooltip("Put the enemy transform here")]
    [SerializeField]
    protected Transform enemyTransform;//the transform of the enemy prefab

    [Header("Enemy state")]
    [Tooltip("This is the enum for the current enemy state")]
    [SerializeField]
    protected GlobalVariables.EnemyStates enemyStates;//the enum for the enemy states

    [Header("The player prefab")]
    [Tooltip("Put the Player prefab in this slot")]
    [SerializeField]
    protected GameObject playerPrefab; //The prefab of the player goes here

    protected Vector3 destination; //This is just showing the position of the enemy in vector 3

    [Header("Was the enemy hit?")]
    [Tooltip("This is the bool for if the enemy was hit")]
    [SerializeField]
    protected bool enemyHit = false; //A bool for if the enemy has been hit

    [Header("Is the enemy alerted?")]
    [Tooltip("This is the bool for if the player is within distance of the enemy and the enemy has been alerted")]
    [SerializeField]
    protected bool enemyAlerted = false; //a bool for if the enemy has been alerted

    [Header("Distance from player")]
    [Tooltip("This is just for reference only. Nothing should be put here")]
    [SerializeField]
    protected float distanceFromPlayer; //This shows the distance that the player is away from the enemy
    [Tooltip("Put a number here of how close the player has to be to alert the enemy")]
    [SerializeField]
    protected float safeDistance; //This is the float for setting the distance from the player before the enemy reacts

    [Header("The death effects")]
    [Tooltip("The death effects prefab for when the enemy dies goes here")]
    [SerializeField]
    protected GameObject deathEffect; //the game object that will house the death effect prefab
    [Tooltip("The parent transform of the enemy goes here")]
    [SerializeField]
    protected Transform deathLocation; //the transform of the death location
    [Space(5)]
    [Header("Sound Effects")]
    [Tooltip("The sound effect of the enemy dying goes here")]
    [SerializeField]
    protected AudioSource dying; //The sound when the enemy dies

    [Space(5)]
    [Header("Ranged attack Distance")]
    [Tooltip("This is for reference only.  It just shows the distance the enemy is from the player")]
    [SerializeField]
    protected float attackDistanceFromPlayer; //this shows the distance that the enemy is from the player for shooting
    
    [Tooltip("Enter a number here for how close the player has to be before the enemy will fire at them")]
    [SerializeField]
    protected float attackDistanceTooFar; //this is the float for setting the distance from the player to have the enemy shoot
    
    [Space(5)]
    [Header("Enemy Shooting Time")]
    [Tooltip("This is for reference only.  This shows the amount of time between shots")]
    [SerializeField]
    protected float timeBetweenShots;

    //[Header("Time to wait until continue moving")]
    //[SerializeField]
    ////protected float timeToWait;
    //[SerializeField]
    //protected float timeWaited;

    [Tooltip("Put the amount of time between shots here")]
    [SerializeField]
    protected float startTimeBetweenShots;

    [Space(5)]
    [Header("The enemies bullet")]
    [Tooltip("Put the enemy bullet prefab here")]
    [SerializeField]
    protected GameObject enemyProjectile;

    [Tooltip("The spawn point transform for the bullet prefab goes here")]
    [SerializeField]
    protected Transform bulletSpawnPoint;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //Debug.Log("setting the enemy health at start");
        curEnemyHealth = maxEnemyHealth;
        //Debug.Log("The nav agent does not autobrake");
        enemyNavAgent.autoBraking = false;
        //Debug.Log("The starting state the enemy is in");
        enemyStates = GlobalVariables.EnemyStates.Moving;
        //Debug.Log("Enemy is not dead");
        enemyDead = false;
        //Debug.Log("Setting the enemy is alerted to false at start");
        enemyAlerted = false;
        //Debug.Log("Setting the player at start");
        playerPrefab = GameObject.FindWithTag("Player");
        //Debug.Log("Enemy hit bool is set to false at start");
        enemyHit = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //Debug.Log("runing the enemy state in update");
        EnemyState();
        //Debug.Log("calculating enemy health" + curEnemyHealth);
        enemySlider.value = CalculatingEnemyHealth();
        //Debug.Log("Always checking if player is invisible");
        //PlayerInvisible();
    }

    protected virtual float CalculatingEnemyHealth() //calculating the enemy health
    {
        return (curEnemyHealth / maxEnemyHealth);
    }

    //public void InstaKill() //killing the enemy instantly from the debug menu
    //{
    //    Destroy(enemyPrefab);
    //}

    protected virtual void EnemyState() //The state that the enemy is in
    {
        switch (enemyStates)
        {
            case GlobalVariables.EnemyStates.Moving:
                //Debug.Log("Enemy is Moving");
                Moving();
                break;
            case GlobalVariables.EnemyStates.Idle:
                //Debug.Log("Enemy is Idle");
                Idle();
                break;
            case GlobalVariables.EnemyStates.Chasing:
                //Debug.Log("Enemy is Chasing");
                Chase();
                break;
            case GlobalVariables.EnemyStates.RangeAttack:
                //Debug.Log("Enemy is attacking");
                RangeAttack();
                break;
            case GlobalVariables.EnemyStates.PlayerInvisible:
                //Debug.Log("Enemy is in Player Invisible state");
                //PlayerInvisible();
                break;
        }
    }
    protected virtual void EnemyShooting()
    {
        if (timeBetweenShots <= 0)
        {
            //Debug.Log("Base enemy Firing the enemy projectile at the player");
            Instantiate(enemyProjectile, bulletSpawnPoint.position, Quaternion.identity);
            //Debug.Log("Base enemy resetting the time between shots");
            timeBetweenShots = startTimeBetweenShots;
            FacingTarget(playerPrefab.transform.position);
        }
        else
        {
            timeBetweenShots -= Time.deltaTime;
        }
    }
    protected virtual void PlayerInvisible()
    {
        if (playerPrefab.GetComponent<PhoenixMovement>().invisible == true)
        {
            //Debug.Log("Enemy is not alerted and player is invisible");
            enemyAlerted = false;
            //Debug.Log("Enemy is going into Moving state");
            //enemyStates = GlobalVariables.EnemyStates.Moving;
        }
        else
        {
            //Debug.Log("Enemy is alerted and player is not invisible");
            enemyAlerted = true;
            //Debug.Log("Enemy is going into range Attack");
            enemyStates = GlobalVariables.EnemyStates.Chasing;
        }
    }
    protected virtual void FacingTarget(Vector3 destination)//the function for the enemy to face the player when the player is within a certain distance of the enemy
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, .5f);
    }

    protected virtual void RangeAttack()//the function for when the enemy has been hit by the player to fire back at the player
    {
        attackDistanceFromPlayer = Vector3.Distance(enemyTransform.position, playerPrefab.transform.position);
        //Debug.Log("Base enemy In the ranged attack function");
        if (!playerPrefab.GetComponent<PhoenixMovement>().invisible)
        {
            if (enemyHit == true)
            {
                if (attackDistanceFromPlayer <= attackDistanceTooFar)
                {
                    EnemyShooting();
                    //Debug.Log("Base Enemy is switching to Chasing");
                    enemyStates = GlobalVariables.EnemyStates.Chasing;
                }

                if (attackDistanceFromPlayer > attackDistanceTooFar) //Can't use equal to here, you use equal to on the other if check the code wouldn't know which to pick if it was exaclty equal as both if checks would qualify.
                {
                    //Debug.Log("Base enemy going back to moving state");
                    enemyStates = GlobalVariables.EnemyStates.Moving;
                    //Debug.Log("Base enemy setting the enemy hit to false");
                    enemyHit = false; //If the opening if check, checks for this bool make sure you change it last as changing it could kick it out before the code finishes executing the lines below.
                }
            }
            else if (!enemyHit)
            {
                //Debug.Log("Base enemy going back to moving state");
                enemyStates = GlobalVariables.EnemyStates.Moving;
            }
            else return;
        }

        else if (playerPrefab.GetComponent<PhoenixMovement>().invisible)
        {
            //Debug.Log("Base enemy is going back to moving state");
            enemyStates = GlobalVariables.EnemyStates.Moving;
        }
    }

    protected virtual void Chase() //The chase state for when the player is within range of the enemy also goes back to the moving state when the player is out of range of the enemy
    {
        //Debug.Log("Setting the enemy alerted to true");
        enemyAlerted = true;
        distanceFromPlayer = Vector3.Distance(enemyTransform.position, playerPrefab.transform.position);
        //Debug.Log("in the chasing function");
        if (!playerPrefab.GetComponent<PhoenixMovement>().invisible)
        {
            if (enemyAlerted == true)
            {
                if (distanceFromPlayer <= safeDistance)
                {
                    //Debug.Log("Enemy has been alerted");
                    //enemyAlerted = true;
                    destination = playerPrefab.transform.position;
                    enemyNavAgent.destination = destination;
                    //Debug.Log("Enemy Shooting");
                    EnemyShooting();
                }
                if (distanceFromPlayer > safeDistance)
                {
                    //Debug.Log("Switching to moving from chasing");
                    enemyStates = GlobalVariables.EnemyStates.Moving;
                    //Debug.Log("Setting the bool for enemy alterted to false");
                    enemyAlerted = false;
                }
            }
            else if (!enemyAlerted)
            {
                //Debug.Log("Switching to moving from chasing");
                enemyStates = GlobalVariables.EnemyStates.Moving;
            }
            else return;
        }


        else if (playerPrefab.GetComponent<PhoenixMovement>().invisible)
        {
            enemyAlerted = false;
            enemyStates = GlobalVariables.EnemyStates.Moving;
        }
    }

    protected virtual void Idle() //The Idle state the enemy is in
    {
        if (enemyNavAgent != null)
        {
            float timer = 0;
            if (timer != 0)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0;

                PickNextNavPoint();

                enemyStates = GlobalVariables.EnemyStates.Moving;
            }
        }
        else
        {
            return;
        }

    }

    protected virtual void Moving()//the moving state the enemy is in
    {
        enemySpawned = true;

        distanceFromPlayer = Vector3.Distance(enemyTransform.position, playerPrefab.transform.position);

        if (enemySpawned == true && !enemyNavAgent.pathPending && enemyNavAgent.remainingDistance < .5f && enemyPrefab != null && distanceFromPlayer >= safeDistance)
        {
            PickNextNavPoint();
        }
        if (!playerPrefab.GetComponent<PhoenixMovement>().invisible)
        {
            enemyAlerted = true;
            if(enemyAlerted == true)
            {
                if (distanceFromPlayer <= safeDistance)
                {
                    enemyStates = GlobalVariables.EnemyStates.Chasing;
                }
            }
            if (!enemyAlerted)
            {
                enemyStates = GlobalVariables.EnemyStates.Moving;
            }
            else return;

        }

        else if(playerPrefab.GetComponent<PhoenixMovement>().invisible && distanceFromPlayer <= safeDistance)
        {
            enemyAlerted = false;
            enemyStates = GlobalVariables.EnemyStates.Moving;
        }
    }

    protected virtual void PickNextNavPoint() //the AI picking the next nav point
    {
        if (navPoints.Length == 0)
        {
            return;
        }
        enemyNavAgent.destination = navPoints[navIndex].transform.position;
        navIndex = (navIndex + 1) % navPoints.Length;
    }

    protected virtual void Death()//the function for the death of the enemy 
    {
        if(curEnemyHealth <= 0)
        {
            enemyDead = true;
            Destroy(gameObject);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)//the function for if the player is damaging the enemy on collision with projectiles
    {
        if(collision.gameObject.GetComponent<PlayerDamage>() != null)
        {
            curEnemyHealth -= collision.gameObject.GetComponent<PlayerDamage>().damage;
            //Debug.Log("base enemy setting the enemyHIt bool to true");
            enemyHit = true;
    
            if(curEnemyHealth <= 0)
            {
                GameObject deathFx = (GameObject)Instantiate(deathEffect, deathLocation.position, deathLocation.rotation);
                Destroy(deathFx, 2f);
    
                //Debug.Log("Playing the DeathNoise");
                DeathNoise();
                Death();
                enemyPrefab = null;
            }
            else if(enemyHit == true)
            {

                //Debug.Log("Base Enemy going into ranged attack");
                enemyStates = GlobalVariables.EnemyStates.RangeAttack;
            }
        }
    }

    protected virtual void DeathNoise()//the noise the enemy makes when they die
    {
        //Debug.Log("Playing the death noise");
        dying.Play(0);
    }
}

