using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

//Author:  Benjamin Boese
//Date: 11-6-2019
//Purpose: For making a boss
public class Boss : MonoBehaviour
{
    [Header("Health of Boss")]
    public float bossHealth; //This is the current health of the boss
    public float maxBossHealth; //This is the max health of the boss
    [Header("Health Bar slider for Boss goes here")]
    public Slider bossHealthBar; //This is the UI for the bosses health bar
    [Header("Health of Wave Spawner")]
    public float waveSpawnerCurrentHealth; //This is the current health of the wave spawner
    public float waveSpanwerMaxHealth; //This is the max health of the wave spawner
    [Header("Health Bar Slider for the Wave spawner")]
    public Slider waveSpawnerSlider; //This is the UI for the wave spawners health
    [Header("Death Effects")]
    public GameObject deathEffect; //This is the bosses death effect 
    public Transform deathLocation; //This is the bosses death location
    public GameObject spawnerDeathEffect; //This is the spawners death effect
    public Transform SpawnerDeathLocaiton; //This is the spawners death location
    [Header("Boss is being attacked?")]
    public bool bossBeingAttacked = false; //A bool for if the boss is being attacked or not
    [Header("Wave Spawner being attacked?")]
    public bool waveSpawnerBeingAttacked = false; //A bool for if the wave spawner is being attacked or not
    [Header("Boss Movement")]
    public Transform[] navPoints; //The points for which the boss moves on the nav mesh
    public Transform specialNavPoint; //The point the boss goes to when they are in their special attack
    public int navIndex = 0; //The integer for the nav index
    [Header("Boss Prefab")]
    public GameObject bossPrefab; //This is for the boss prefab
    [Header("Boss Nav mesh agent")]
    public NavMeshAgent bossAgent; //This is referencing the nav mesh agent for the boss
    [Header("The enum for the bosses current state")]
    public GlobalVariables.BossStates bossStates; //This is referencing the enum for the boss states
    [Header("The player Prefab")]
    public GameObject playerPrefab; //The prefab for the player goes here
    public Transform playerTransform;
    [Header("Enemy Projectile")]
    public GameObject bossProjectile; //The prefab for the bosses projectile goes here
    [Header("The Wave Spawn point")]
    public Transform bossProjectileSpawnPoint; //The point at which the enemies spawns
    [Header("WaveSpawner Prefab goes here")]
    public GameObject waveSpawner; //The prefab for the wave spawner
    [Header("bullet spawn point")]
    [Header("The bullet spawn point transform")]
    public Transform bulletSpawningPoint; //The transform of where the bullet spawns
    [Header("Is the boss Dead?")]
    public bool bossDead; //A bool for if the boss is dead
    [Header("Is the wave Spawner Dead?")]
    public bool wavespawnerDead; //A bool for if the wave spawner is dead
    //[Header("Game object for the line PFIs")]
    //public GameObject[] linePFIs;//An array for the line PFIs that are to show the player where the bullets will go
    [Header("The starting color for Lerp")]
    public Color startingLerpColor = Color.yellow; //The starting lerp color
    [Header("The ending color for Lerp")]
    public Color endingLerpColor = Color.red;//The ending lerp color
    [Header("The starting Lerp time")]
    public float startingLerpColorTime; //The time counting down for the lerp color time
    [Header("The max Lerp time")]
    public float maxStaringLerpColorTime; //the max time the color will be starting at when lerping
    [Header("The percentage for the Lerp")]
    public float colorToLerp; //The number 0  or 1 for setting the lerp color time
    [Header("The time the AI is at the Nav Point")]
    public float timeAtNavPoint; //The time the boss is at the nav point
    [Header("The amount of time the AI is allowed to be at nav point")]
    private float startTimeAtNavPoint; //The amount of time the boss is allowed to be at the nav point
    [Header("The starting time between shots")]
    public float startTimeBetweenShots; //the time that sets when a shot will be fired
    [Header("The actual time between shots counting down")]
    private float timeBetweenShots; //the time counting down for when the shots will be fired
    [Header("The door to the next area")]
    public GameObject wall; //The wall that allows the player access to the next area
    [Header("For Reference only.  The distance from the player")]
    public float distanceFromPlayer; //the distance that the player is from the enemy
    [Header("The distance that the enemy is from the player before being alerted")]
    public float safeDistance;//this is set in the inspector for the distance between the enemy and the player before enemy is alerted
    [Header("Is the boss alerted?")]
    public bool bossAlerted = false; //a bool for setting if the boss has been alerted or not
    [Header("The bosses Transform goes here")]
    public Transform bossTransform; //the transform of the boss
    [Header("for reference only.  The movement of the enemy to the player")]
    public Vector3 destination; //the vector 3 for the movement of the enemy to the player



    private void Start()
    {
        //Debug.Log("setting the boss agents stopping distance at start");
        bossAgent.stoppingDistance = 0.5f; //This is the stopping distance that the boss agent will stop at
        //Debug.Log("Setting the wall active at start");
        wall.SetActive(true); //Setting the wall active at start until the boss is defeated
        //Debug.Log("setting the line PFI to inactive at start");
        //foreach(GameObject line in linePFIs) //a for each loop setting the line PFIs inactive at start
        //{
            //line.gameObject.SetActive(false);
        //}
        //Debug.Log("Setting the boss health at start");
        bossHealth = maxBossHealth;
        //Debug.Log("Setting the wave health at start");
        waveSpawnerCurrentHealth = waveSpanwerMaxHealth;
        //Debug.Log("calculating health for the wave spawner");
        waveSpawnerSlider.value = CalculatingWaveSpawnerHealth();
        //Debug.Log("setting the calculating health to the health bar on start");
        bossHealthBar.value = CalculatingBossHealth();
        //Debug.Log("Boss starts at moving");
        bossStates = GlobalVariables.BossStates.Moving;
        //Debug.Log("setting the wave spawner on");
        waveSpawner.SetActive(true);
        //Debug.Log("Is the boss Dead");
        bossDead = false;
        //Debug.Log("Is the wave spawner dead");
        wavespawnerDead = false;
        //Debug.Log("The boss agent's autobraking is set to false at start");
        bossAgent.autoBraking = false;
        //Debug.Log("setting the bossAlerted to false at start");
        bossAlerted = false;
        //Debug.Log("Setting the player prefab at start");
        playerPrefab = GameObject.FindWithTag("Player");
        //Debug.Log("Setting the Player prefab transform at start");
        playerTransform = GameObject.FindWithTag("Player").transform;
        //Debug.Log("setting the is the boss being attacked bool to false at start");
        bossBeingAttacked = false;
    }

    private void Update()
    {
        //Debug.Log("Calculating Health" + bossHealth);
        bossHealthBar.value = CalculatingBossHealth();
        //Debug.Log("Calculating Wave spawner Health" + waveSpawnerCurrentHealth);
        waveSpawnerSlider.value = CalculatingWaveSpawnerHealth();
        //Debug.Log("Continiuosly checking the boss state");
        BossState();
    }

    void BossState()//This is the function for what state the boss is in
    {
        if (!bossDead)
        {
            //Debug.Log("inside the if boss dead statement");
            switch (bossStates)
            {
                case GlobalVariables.BossStates.Idle:
                    //Debug.Log("Idle state");
                    Idle();
                    break;
                case GlobalVariables.BossStates.FirstAttack:
                    //Debug.Log("Boss is attacking with first attack");
                    FirstAttack();
                    break;
                case GlobalVariables.BossStates.Moving:
                    //Debug.Log("Boss is Moving");
                    Moving();
                    break;
                case GlobalVariables.BossStates.MoveToPosition:
                    //Debug.Log("Moving to Position");
                    MovingToPosition();
                    break;
                case GlobalVariables.BossStates.Chase:
                    //Debug.Log("Boss is chasing");
                    Chase();
                    break;
                case GlobalVariables.BossStates.Attacking:
                    //Debug.Log("Boss is attacking");
                    Attacking();
                    break;
            }
        }
    }

    void Attacking() //The function for the boss attacking the player if the player is within chasing or attacking distance
    {
        distanceFromPlayer = Vector3.Distance(bossTransform.position, playerPrefab.transform.position);
        //Debug.Log("Boss in the attack function");
        if(distanceFromPlayer <= safeDistance)
        {
            //Debug.Log("boss bool was set to true");
            bossBeingAttacked = true;
            if(timeBetweenShots <= 0)
            {
                //Debug.Log("Boss firing the projectile");
                Instantiate(bossProjectile, bossProjectileSpawnPoint.position, bossProjectileSpawnPoint.rotation);
                timeBetweenShots = startTimeBetweenShots;
                FaceTarget(playerPrefab.transform.position);
            }
            else
            {
                timeBetweenShots -= Time.deltaTime;
            }
        }

        if(distanceFromPlayer >= safeDistance && bossBeingAttacked == true)
        {
            bossBeingAttacked = false;
            bossStates = GlobalVariables.BossStates.Moving;
        }
    }

    void Chase()//this is the function for the boss chasing the player when the player is within a certain distance
    {
        distanceFromPlayer = Vector3.Distance(bossTransform.position, playerPrefab.transform.position);
        //Debug.Log("Boss is in the chasing function");
        if(bossAlerted == true && distanceFromPlayer <= safeDistance)
        {
            bossAlerted = true;
            //Debug.Log("setting the destination for the boss to move too");
            destination = playerPrefab.transform.position;
            //Debug.Log("moving the boss agent to destination");
            bossAgent.destination = destination;
            if(timeBetweenShots <= 0)
            {
                //Debug.Log("Boss is firing at player while chasing");
                Instantiate(bossProjectile, bulletSpawningPoint.position, bulletSpawningPoint.rotation);
                timeBetweenShots = startTimeBetweenShots;
                FaceTarget(playerPrefab.transform.position);
            }
            else
            {
                timeBetweenShots -= Time.deltaTime;
            }
        }
        if(distanceFromPlayer >= safeDistance && bossAlerted == true)
        {
            //Debug.Log("boss Alerted is false");
            bossAlerted = false;
            //Debug.Log("boss is going back to the moving state");
            bossStates = GlobalVariables.BossStates.Moving;
        }
        else if(bossBeingAttacked == true)
        {
            bossBeingAttacked = true;
            bossStates = GlobalVariables.BossStates.Attacking;
        }
    }

    void MovingToPosition() //the function for moving the boss to the special navpoint
    {
        if(bossAgent.destination != specialNavPoint.position)
        {
            //Debug.Log("setting the boss agent to the special nav point");
            bossAgent.destination = specialNavPoint.position;
            //Debug.Log("having the boss face the player with every tried shot");
            FaceTarget(playerTransform.position);

        }

        //Debug.Log("Inside the moving to position function");
        if(bossAgent.remainingDistance <= bossAgent.stoppingDistance)
        {
            if (!bossAgent.hasPath || bossAgent.velocity.sqrMagnitude == 0f)
            {
                //Debug.Log("boss state is going into first attack");
                bossStates = GlobalVariables.BossStates.FirstAttack;
            }
        }
        
    }

    private void FaceTarget(Vector3 destination)//The function for having the boss look at the player when in the special attack
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, .5f);
    }

    void FirstAttack() //the function for the boss doing the first attack
    {
        //Debug.Log("Boss is stopped");
        bossAgent.isStopped = true;

        //Debug.Log("Boss doing first Attack");
        if(bossAgent.isStopped == true)
        {
            //Debug.Log("Bosses Line PFIs are setactive and then lerped before shooting");
            //LerpingBossColor();
            //if(endingLerpColor == Color.red)
            //{
                if (timeBetweenShots <= 0)
                {
                    //Debug.Log("Firing the projectile at 25 degree");
                    Instantiate(bossProjectile, bulletSpawningPoint.transform.position, bulletSpawningPoint.transform.rotation * Quaternion.Euler(0, 25, 0));
                    //Debug.Log("Firing the projectile at 0 degrees");
                    Instantiate(bossProjectile, bulletSpawningPoint.transform.position, bulletSpawningPoint.transform.rotation);
                    //Debug.Log("Firing the projectile at -25 degrees");
                    Instantiate(bossProjectile, bulletSpawningPoint.transform.position, bulletSpawningPoint.transform.rotation * Quaternion.Euler(0, -25, 0));
                    timeBetweenShots = startTimeBetweenShots;
                    FaceTarget(playerTransform.position);
                }
                else
                {
                    timeBetweenShots -= Time.deltaTime;
                }
            //}
        }
    }

    //void LerpingBossColor() //the lerping color for the line PFIs function
    //{
    //    startingLerpColorTime += Time.deltaTime;
    //    //Debug.Log("setting the Line PFI Active");
    //    foreach(GameObject line in linePFIs)
    //    {
    //        line.gameObject.SetActive(true);
    //        if (line)
    //        {
    //            //Debug.Log("Lerping the color of the lines");
    //            line.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(startingLerpColor, endingLerpColor, colorToLerp);
    //            //Debug.Log("setting the percentage of the color to lerp");
    //            colorToLerp = startingLerpColorTime / maxStaringLerpColorTime;
    //        }
    //    }
    //}

    void Idle() //the boss is at an idle state function
    {
        if (bossAgent != null)
        {
            //Debug.Log("Iside the Idle function");
            timeAtNavPoint = 0;
            //float timer = 0;
            if (timeAtNavPoint != 0)
            {
                // bossStates = GlobalVariables.BossStates.Idle;
                //Debug.Log("time at nav point counting up" + timeAtNavPoint);
                timeAtNavPoint += Time.deltaTime;
            }
            else
            {
                timeAtNavPoint = 0;
                PickNextNavPoint();
                bossStates = GlobalVariables.BossStates.Moving;
            }
        }
        else
        {
            return;
        }

    }

    void PickNextNavPoint() //the function for picking the next nav point
    {
        //Debug.Log("In the pick next nav point function");
        if (navPoints.Length == 0)
            return;
        bossAgent.destination = navPoints[navIndex].transform.position;
        navIndex = (navIndex + 1) % navPoints.Length;
    }

    void FindDestination() //The function for finding the next destination
    {
        //Debug.Log("Boss finding the next destination");
        bossPrefab.GetComponent<NavMeshAgent>().SetDestination(navPoints[navIndex].transform.position);
    }

    void Moving() //The boss is moving between the array of nav points while the wave spawner is still active
    {
        bossDead = false;
        //Debug.Log("In the moving function");

        distanceFromPlayer = Vector3.Distance(bossTransform.position, playerPrefab.transform.position);
        if (!bossAgent.pathPending && bossAgent.remainingDistance < 0.5f && wavespawnerDead == false && !bossDead && bossPrefab != null)
        {
            //Debug.Log("Picking the next nav point");         
            PickNextNavPoint();
        }
        else if(distanceFromPlayer <= safeDistance)
        {
            //Debug.Log("Boss is: " + distanceFromPlayer + " from player");
            //Debug.Log("setting the bossAlerted to true");
            bossAlerted = true;
            //Debug.Log("boss is going into chase state");
            bossStates = GlobalVariables.BossStates.Chase;
        }
    }

    private void OnCollisionEnter(Collision collision)//this is the function for if the boss or wave spawner is being attacked by the player
    {
        if (collision.gameObject.GetComponent<PlayerDamage>() != null)
        {
            //Debug.Log("Wave Spawner Being Attacked");
            waveSpawnerBeingAttacked = true;
            if (waveSpawnerBeingAttacked == true)
            {
                //Debug.Log("hitting the wave spawner for damage.");
                waveSpawnerCurrentHealth -= collision.gameObject.GetComponent<PlayerDamage>().damage;
                if (waveSpawnerCurrentHealth <= 0)
                {
                    //Debug.Log("wave spawner defeated");
                    //Debug.Log("doing the destory effect");
                    GameObject deathFx = (GameObject)Instantiate(deathEffect, deathLocation.position, deathLocation.rotation);
                    Destroy(deathFx, 2f);
                    //Debug.Log("destroying the wave spawner");
                    Destroy(waveSpawner);
                    //Debug.Log("setting the wave spawner to null");
                    waveSpawner = null;
                    //waveSpawnerSlider = null;
                    //Debug.Log("Boss moving into position for first Attack");
                    bossStates = GlobalVariables.BossStates.MoveToPosition;
                }
            }

            if (waveSpawner == null) //<<<<<<<<<<<Need to figure out a way to fire this if statement off 
                                     //after the boss moves to the special nav point
            {
                //Debug.Log("Boss is being attacked by Player");
                bossBeingAttacked = true;
                //Debug.Log("Hitting the boss for damage.  The bosses Health is now: " + bossHealth);
                bossHealth -= collision.gameObject.GetComponent<PlayerDamage>().damage;

                if (bossHealth <= 0)
                {
                    //Debug.Log("Instantiating the death effect");
                    GameObject deathFx = (GameObject)Instantiate(deathEffect, deathLocation.position, deathLocation.rotation);
                    Destroy(deathFx, 2f);

                    //Debug.Log("Boss is Dead");
                    Destroy(gameObject);
                    //Debug.Log("Setting the wall inactive once boss is dead");
                    wall.SetActive(false);
                }

            }
        }
    }

    float CalculatingBossHealth() //this just calculates the bosses health
    {
        return bossHealth / maxBossHealth;
    }

    float CalculatingWaveSpawnerHealth() //This just calculates the spawners health
    {
        return waveSpawnerCurrentHealth / waveSpanwerMaxHealth;
    }

    public void InstaKill()//this is for the debug Menu for killing the AI
    {
        Destroy(this.gameObject);
        //Debug.Log("Enemy InstaKilled");
    }
}
