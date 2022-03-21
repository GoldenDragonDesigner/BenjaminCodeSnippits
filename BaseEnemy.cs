using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class BaseEnemy : MonoBehaviour
{
    #region Variables

    protected bool IsEnemySpawned;
    protected bool IsEnemyDead;
    protected bool IsEnemyAlerted;
    protected bool IsEnemyMeleeAttacking;
    protected bool IsEnemyRangeAttacking;

    protected GameObject EnemyPrefab;
    protected NavMeshAgent EnemyNavAgent;
    protected Transform EnemyTransform;

    [Header("Code the values in a child class do not set any of the following variables in the inspector")]
    [Tooltip("Set the enemy health in the Child script using SetMaxiumEnemyHealth(???)")]
    [SerializeField]
    protected float MaximumEnemyHealth;
    protected float CurrentEnemyHealth;
    protected float DistanceFromPlayer; //This shows the distance that the player is away from the enemy
    [Tooltip("Set the distance that the enemy needs to be  in the Child script using SetSafeDistance(??) to interact with the player")]
    [SerializeField]
    protected float SafeDistance;
    [Tooltip("Set the number  in the Child script using SetRangeAttackDistance(???) for how close the player has to be before the enemy will fire at them when the enemy is alerted because they were fired on")]
    [SerializeField]
    protected float RangeAttackDistance;
    [Tooltip("Set the number  in the Child script using SetAttackRange(????) for when the enemy will melee attack the player")]
    [SerializeField]
    protected float AttackRange;
    [Tooltip("Set the number  in the Child script using SetChaseRange(???) for when the enemy is to chase the player")]
    [SerializeField]
    protected float ChaseRange;

    //[SerializeField]
    protected float RangeAttackDistanceFromPlayer;
    [Tooltip("Put the amount of time between shots  in the Child script using SetStartTimeBetweenShots(?????)")]
    [SerializeField]
    protected float StartTimeBetweenShots;
    protected float TimeBetweenShots;
    [SerializeField]
    [Tooltip("This is the time the coroutine delays setting the enemyHit back to false in order to play that animation")]
    protected float DamageTime;
    [SerializeField]
    [Tooltip("This is the walking radius of the enemy")]
    protected float WalkRadius;
    [SerializeField]
    [Tooltip("This is setting how fast the enemy will move when it is walking around to time with its animation")]
    public float WalkSpeed;
    [SerializeField]
    [Tooltip("This is setting how fast the enemy will move when its running to be able to time with its animation")]
    public float ChaseSpeed;
    [SerializeField]
    [Tooltip("This is how long before the game object will destroy itself so the animation can play")]
    protected float DeathTime;
    [Tooltip("Put the amount of time between melee hits the enemy will do")]
    [SerializeField]
    protected float StartTimeBetweenHits;
    [SerializeField]
    protected float TimeBetweenHits;
    [Tooltip("Set how long the Enemy will run away or evade the player")]
    [SerializeField]
    protected float RunAwayStartTime;
    protected float RunAwayTime;

    [Header("Strings for the base enemy animation names")]
    [SerializeField]
    [Tooltip("The animation is a ....... and set  in the Child script using SetRunAnimationName(???)")]
    protected string RunAnimationName;
    [SerializeField]
    [Tooltip("The animation is a ..... and set  in the Child script using SetWalkAnimationName(???)")]
    protected string WalkAnimationName;
    [SerializeField]
    [Tooltip("The animation is a Trigger and set  in the Child script using SetDieAnimationName(???)")]
    protected string DieAnimationName;
    [SerializeField]
    [Tooltip("The animation is a trigger and set  in the Child script using SetTakeDamageAnimationName(???)")]
    protected string TakeDamageAnimationName;
    [SerializeField]
    [Tooltip("The animation is a trigger and set in the Child script using SetMeleeAttackAnimationName(???)")]
    protected string MeleeAttackAnimationName;
    [Tooltip("Set the name of this Enemy  in the Child script using SetEnemyTagName(gameObject.tag = '???' ")]
    [SerializeField]
    protected string EnemyTagName;

    protected GameObject PlayerPrefab;

    protected GlobalVariables.AIStates EnemyStates;

    protected Vector3 EnemyPosition;

    protected Animator Animator;

    protected int AnimationVelocity;

    //protected bool IsEnemyHit;
    //protected bool IsEnemyMoving;
    //protected bool IsIdle;
    //[Tooltip("This is the bool for if the enemy is picking from nav points")]
    //protected bool pickingFromPoints;
    //[Tooltip("This is the bool for if the enemy is counting down")]
    //protected bool countingDown;
    //[Tooltip("Put the enemy slider health bar here")]
    //protected Slider EnemySlider;
    //[Tooltip("Enter the number for when the enemy will runn ")]
    //[SerializeField]
    //protected float runAwayDistance;
    //This is just showing the position of the enemy in vector 3
    //this shows the distance that the enemy is from the player for shooting
    //this is the time between shots
    #endregion Variables

    #region Awake, Start, and Update

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        EnemyPrefab = gameObject;
        EnemyNavAgent = GetComponent<NavMeshAgent>();
        EnemyTransform = GetComponent<Transform>();
        CurrentEnemyHealth = MaximumEnemyHealth;
        TimeBetweenShots = StartTimeBetweenShots;
        TimeBetweenHits = StartTimeBetweenHits;
        RunAwayTime = RunAwayStartTime;
        EnemyStates = GlobalVariables.AIStates.Idle;
        IsEnemyDead = false;
        IsEnemyAlerted = false;
        PlayerPrefab = GlobalVariables.Player.gameObject;
        Animator = GetComponent<Animator>();
        ResetAttackRange();
        AnimationVelocity = Animator.StringToHash("Velocity");
    }

    protected virtual void Update()
    {
        EnemyState();
        CalculatingEnemyHealth();
        DistanceFromPlayer = Vector3.Distance(EnemyTransform.position, PlayerPrefab.transform.position);
    }

    #endregion Awake, Start, and Update

    #region UtilityFunctions

    #region GettersAndSetters

    public bool GetIsEnemySpawned()
    {
        return IsEnemySpawned;
    }

    public bool GetIsEnemyDead()
    {
        return IsEnemyDead;
    }

    public bool GetIsEnemyAlerted()
    {
        return IsEnemyAlerted;
    }

    protected virtual void SetIsEnemyDead(bool isEnemyDead)
    {
        IsEnemyDead = isEnemyDead;
    }

    public bool GetIsEnemyMeleeAttacking()
    {
        return IsEnemyMeleeAttacking;
    }

    public GameObject GetEnemyPrefab()
    {
        return EnemyPrefab;
    }

    protected virtual void SetEnemyPrefab(GameObject enemyPrefab)
    {
        EnemyPrefab = enemyPrefab;
    }

    protected virtual NavMeshAgent GetEnemyNavAgent()
    {
        return EnemyNavAgent;
    }

    protected virtual void SetEnemyNavAgent(NavMeshAgent enemyNavAgent)
    {
        EnemyNavAgent = enemyNavAgent;
    }

    public Transform GetEnemyTransform()
    {
        return EnemyTransform;
    }

    protected virtual void SetEnemyTransform(Transform enemyTransform)
    {
        EnemyTransform = enemyTransform;
    }

    public float GetMaximumEnemyHealth()
    {
        return MaximumEnemyHealth;
    }

    protected virtual void SetMaximumEnemyHealth(float maximumEnemyHealth)
    {
        MaximumEnemyHealth = maximumEnemyHealth;
    }

    public float GetCurrentEnemyHealth()
    {
        return CurrentEnemyHealth;
    }

    protected virtual void SetCurrentEnemyHealth(float currentEnemyHealth)
    {
        CurrentEnemyHealth = currentEnemyHealth;
    }

    protected virtual float GetRangeAttackDistance()
    {
        return RangeAttackDistance;
    }

    protected virtual void SetRangeAttackDistance(float rangeAttackDistance)
    {
        RangeAttackDistance = rangeAttackDistance;
    }

    protected virtual float GetAttackRange()
    {
        return AttackRange;
    }

    protected virtual void SetAttackRange(float attackRange)
    {
        AttackRange = attackRange;
    }

    protected virtual float GetChaseRange()
    {
        return ChaseRange;
    }

    protected virtual void SetChaseRange(float chaseRange)
    {
        ChaseRange = chaseRange;
    }

    protected virtual float GetStartTimeBetweenShots()
    {
        return StartTimeBetweenShots;
    }

    protected virtual void SetStartTimeBetweenShots(float startTimeBetweenShots)
    {
        StartTimeBetweenShots = startTimeBetweenShots;
    }

    protected virtual float GetDamageTime()
    {
        return DamageTime;
    }

    protected virtual void SetDamageTime(float damageTime)
    {
        DamageTime = damageTime;
    }

    public float GetDistanceFromPlayer()
    {
        return DistanceFromPlayer;
    }

    protected virtual float GetWalkRadius()
    {
        return WalkRadius;
    }

    protected virtual void SetWalkRadius(float walkRadius)
    {
        WalkRadius = walkRadius;
    }

    protected virtual float GetWalkSpeed()
    {
        return WalkSpeed;
    }

    protected virtual void SetWalkSpeed(float walkSpeed)
    {
        WalkSpeed = walkSpeed;
    }

    protected virtual float GetChaseSpeed()
    {
        return ChaseSpeed;
    }

    protected virtual void SetChaseSpeed(float chaseSpeed)
    {
        ChaseSpeed = chaseSpeed;
    }

    protected virtual float GetDeathTime()
    {
        return DeathTime;
    }

    protected virtual void SetDeathTime(float deathTime)
    {
        DeathTime = deathTime;
    }

    protected virtual float GetStartTimeBetweenHits()
    {
        return StartTimeBetweenHits;
    }

    protected virtual void SetStartTimeBetweenHits(float startTimeBetweenHits)
    {
        StartTimeBetweenHits = startTimeBetweenHits;
    }

    protected virtual float GetTimeBetweenHits()
    {
        return TimeBetweenHits;
    }

    protected virtual void SetTimeBetweenHits(float timeBetweenHits)
    {
        TimeBetweenHits = timeBetweenHits;
    }

    protected virtual string GetRunAnimationName()
    {
        return RunAnimationName;
    }

    protected virtual void SetRunAnimationName(string runAnimationName)
    {
        RunAnimationName = runAnimationName;
    }

    protected virtual string GetWalkAnimationName()
    {
        return WalkAnimationName;
    }

    protected virtual void SetWalkAnimationName(string walkAnimationName)
    {
        WalkAnimationName = walkAnimationName;
    }

    protected virtual string GetDieAnimationName()
    {
        return DieAnimationName;
    }

    protected virtual void SetDieAnimationName(string dieAnimationName)
    {
        DieAnimationName = dieAnimationName;
    }

    protected virtual string GetTakeDamageAnimationName()
    {
        return TakeDamageAnimationName;
    }

    protected virtual void SetTakeDamageAnimationName(string takeDamageAnimationName)
    {
        TakeDamageAnimationName = takeDamageAnimationName;
    }

    protected virtual string GetMeleeAttackAnimationName()
    {
        return MeleeAttackAnimationName;
    }

    protected virtual void SetMeleeAttackAnimationName(string meleeAttackAnimationName)
    {
        MeleeAttackAnimationName = meleeAttackAnimationName;
    }

    protected virtual GameObject GetPlayerPrefab()
    {
        return PlayerPrefab;
    }

    protected virtual void SetPlayerPrefab(GameObject playerPrefab)
    {
        PlayerPrefab = playerPrefab;
    }

    protected virtual Vector3 GetEnemyPosition()
    {
        return EnemyPosition;
    }

    protected virtual float GetRangeAttackDistanceFromPlayer()
    {
        return RangeAttackDistanceFromPlayer;
    }

    protected virtual void SetRangeAttackDistanceFromPlayer(float rangeAttackDistanceFromPlayer)
    {
        RangeAttackDistanceFromPlayer = rangeAttackDistanceFromPlayer;
    }

    protected virtual float GetTimeBetweenShots()
    {
        return TimeBetweenShots;
    }

    protected virtual float GetRunAwayStartTime()
    {
        return RunAwayStartTime;
    }

    protected virtual void SetRunAwayStartTime(float runAwayStartTime)
    {
        RunAwayStartTime = runAwayStartTime;
    }

    protected virtual float GetRunAwayTime()
    {
        return RunAwayTime;
    }

    protected virtual Animator GetAnimator()
    {
        return Animator;
    }

    protected virtual int GetAnimationVelocity()
    {
        return AnimationVelocity;
    }

    public void SetAnimationVelocity(int animationVelocity)
    {
        AnimationVelocity = animationVelocity;
    }

    public string GetEnemyTagName()
    {
        return EnemyTagName;
    }

    protected virtual void SetEnemyTagName(string enemyTagName)
    {
        EnemyTagName = enemyTagName;
    }

    protected virtual float GetSafeDistance()
    {
        return SafeDistance;
    }

    protected virtual void SetSafeDistance(float safeDistance)
    {
        SafeDistance = safeDistance;
    }

    #endregion GettersAndSetters

    //the function for the enemy to face the player when the player is within a certain distance of the enemy
    protected virtual void FacingTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, .5f);
    }

    protected virtual Vector3 PickNextLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        Vector3 finalPosition = Vector3.zero;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    protected virtual void DebugLog(string log)
    {
        Debug.Log(log);
    }

    protected virtual void DebugLog(float log)
    {
        Debug.Log(log);
    }

    protected virtual void DebugLog(int log)
    {
        Debug.Log(log);
    }

    protected virtual void DebugLog(bool log)
    {
        Debug.Log(log);
    }

    #endregion UtilityFunctions

    #region EnemyBehaviour

    #region EnemyEnum
    //The state that the enemy is in
    protected virtual void EnemyState()
    {
        switch (EnemyStates)
        {
            case GlobalVariables.AIStates.Idle:
                Idle();
                break;
            case GlobalVariables.AIStates.FindingNextLocation:
                //FindingNextLocation();
                break;
            case GlobalVariables.AIStates.Moving:
                Moving();
                break;
            case GlobalVariables.AIStates.Attacking:
                Attacking();
                break;
            case GlobalVariables.AIStates.Chasing:
                Chase();
                break;
            case GlobalVariables.AIStates.Searching:
                //Searching();
                break;
            case GlobalVariables.AIStates.RangeAttack:
                RangeAttack();
                break;
            case GlobalVariables.AIStates.Activate:
                //Activate();
                break;
            case GlobalVariables.AIStates.CountingDown:
                //CountingDown();
                break;
            case GlobalVariables.AIStates.Evade:
                
            //    Evade(enemyTagName);
                break;
            case GlobalVariables.AIStates.Summoning:
                //Summoning();
                break;
        }
    }

    #endregion EnemyEnum

    #region Idle

    //The Idle state the enemy is in
    protected virtual void Idle()
    {
        IsEnemySpawned = true;
        if (EnemyNavAgent != null)
        {
            float timer = 0;
            if (timer != 0)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0;
                EnemyStates = GlobalVariables.AIStates.Moving;
            }
        }
    }

    #endregion Idle

    #region EnemyShooting

    protected virtual void EnemyShooting()//this function is empty due to be used in other script that inherit it.
    {

    }

    #endregion EnemyShooting

    #region MeleeAttack

    protected virtual void EnemyMeleeAttacking()
    {
        if (IsEnemyMeleeAttacking)
        {
            IsEnemyMeleeAttacking = true;
            Animator.SetTrigger(MeleeAttackAnimationName);
            EnemyNavAgent.isStopped = true;
            FacingTarget(PlayerPrefab.transform.position);
        }
        else
        {
            IsEnemyMeleeAttacking = false;
        }
    }

    #endregion MeleeAttack

    #region RangeAttack

    //the function for when the enemy has been hit by the player to fire back at the player and is used in child scripts
    protected virtual void RangeAttack()
    {

    }

    public void ResetAttackRange()
    {
        StartCoroutine(ResetWait());
    }

    IEnumerator ResetWait()
    {
        float ar = AttackRange;
        AttackRange = 0;
        yield return new WaitForSeconds(2f);
        AttackRange = ar;
        StopCoroutine(ResetWait());
    }

    #endregion RangeAttack

    #region Chase

    //The chase state for when the player is within range of the enemy also goes back to the moving state when the player is out of range of the enemy
    protected virtual void Chase()
    {
        if (EnemyNavAgent == null)
        {
            return;
        }

        if (DistanceFromPlayer < SafeDistance)
        {
            IsEnemyAlerted = true;
            DistanceFromPlayer = Vector3.Distance(EnemyTransform.position, PlayerPrefab.transform.position);
            if (IsEnemyAlerted)
            {
                if (DistanceFromPlayer < SafeDistance && DistanceFromPlayer > AttackRange)
                {
                    EnemyPosition = PlayerPrefab.transform.position;
                    EnemyNavAgent.destination = EnemyPosition;
                    Animator.SetBool(RunAnimationName, true);
                    EnemyNavAgent.speed = ChaseSpeed;
                    Animator.SetFloat(AnimationVelocity, EnemyNavAgent.velocity.magnitude);
                    //TODO need to see if this is correct or if the EnemyPosition needs to be changed to the player position or the position that the enemy needs to face.
                    FacingTarget(EnemyPosition);
                }

                if (DistanceFromPlayer < AttackRange)
                {
                    Animator.SetBool(RunAnimationName, false);
                    IsEnemyMeleeAttacking = true;
                    EnemyNavAgent.isStopped = true;
                    EnemyStates = GlobalVariables.AIStates.Attacking;
                }

                if (DistanceFromPlayer >= SafeDistance)
                {
                    Animator.SetBool(RunAnimationName, false);
                    IsEnemyAlerted = false;
                    EnemyStates = GlobalVariables.AIStates.Moving;
                }
            }
            if (!IsEnemyAlerted)
            {
                IsEnemyAlerted = false;
                EnemyStates = GlobalVariables.AIStates.Moving;
                Animator.SetBool(RunAnimationName, false);
            }
        }
        else if (DistanceFromPlayer >= SafeDistance)
        {
            Animator.SetBool(RunAnimationName, false);
            IsEnemyAlerted = false;
            EnemyStates = GlobalVariables.AIStates.Moving;
        }

    }

    #endregion Chase


    #region Moving

    //the moving state the enemy is in
    protected virtual void Moving()
    {
        //IsEnemyMoving = true;
        DistanceFromPlayer = Vector3.Distance(EnemyTransform.position, PlayerPrefab.transform.position);

        if (IsEnemySpawned && !EnemyNavAgent.pathPending && EnemyNavAgent.remainingDistance < .5f && EnemyPrefab != null && DistanceFromPlayer >= SafeDistance)
        {
            EnemyNavAgent.SetDestination(PickNextLocation(WalkRadius));
            //IsEnemyMoving = true;
            IsEnemyAlerted = false;
            Animator.SetBool(WalkAnimationName, true);
            EnemyNavAgent.speed = WalkSpeed;
            Animator.SetFloat(AnimationVelocity, EnemyNavAgent.velocity.magnitude);
        }

        if (DistanceFromPlayer <= SafeDistance)
        {
            //IsEnemyMoving = false;
            Animator.SetBool(WalkAnimationName, false);
            EnemyStates = GlobalVariables.AIStates.Chasing;
        }
        //else if (distanceFromPlayer >= safeDistance)
        //{
        //    //IsEnemyAlerted = false;
        //    //IsEnemyMoving = true;
        //    //anim.SetBool(walkAnimationName, true);
        //    //anim.SetFloat(_animationVelocity, enemyNavAgent.velocity.magnitude);
        //    //EnemyStates = GlobalVariables.AIStates.Moving;
        //}
    }

    #endregion Moving

    #region Attacking

    protected virtual void Attacking()
    {
        IsEnemyMeleeAttacking = true;
        DistanceFromPlayer = Vector3.Distance(EnemyTransform.position, PlayerPrefab.transform.position);

        if (DistanceFromPlayer < AttackRange)
        {
            IsEnemyMeleeAttacking = true;

            Animator.SetTrigger(MeleeAttackAnimationName);
            EnemyNavAgent.isStopped = true;
            FacingTarget(PlayerPrefab.transform.position);
            StartCoroutine(SettingTheAttackBackToFalse());

        }
        else if (DistanceFromPlayer > AttackRange && DistanceFromPlayer <= SafeDistance)
        {
            EnemyNavAgent.isStopped = false;
            IsEnemyMeleeAttacking = false;
            EnemyStates = GlobalVariables.AIStates.Chasing;
        }
        else if (DistanceFromPlayer >= SafeDistance)
        {
            EnemyNavAgent.isStopped = false;
            IsEnemyMeleeAttacking = false;
            EnemyStates = GlobalVariables.AIStates.Moving;
        }
    }

    protected virtual IEnumerator SettingTheAttackBackToFalse()
    {
        yield return new WaitForSeconds(5f);
        IsEnemyMeleeAttacking = false;
        EnemyNavAgent.isStopped = false;
        StopCoroutine(SettingTheAttackBackToFalse());
    }

    #endregion Attacking

    //#region Evade

    ////TODO if there are problems with the way the enemies move we may need to adjust this function
    //protected virtual void Evade(string tagName)
    //{
    //    Debug.Log("In Evade");
    //    if (runAwayTime <= 0 && distanceFromPlayer >= runAwayDistance)
    //    {
    //        Debug.Log("Far enough from the player to begin moving again");
    //        runAwayTime = runAwayStartTime;
    //        //enemyPrefab.gameObject.tag = tagName;
    //        EnemyStates = GlobalVariables.AIStates.Moving;
    //    }
    //    else
    //    {
    //        Debug.Log("Nope Nope running away");
    //        runAwayTime -= Time.deltaTime;
    //        if (distanceFromPlayer <= runAwayDistance)
    //        {
    //            Destination = PlayerPrefab.transform.position;
    //            enemyNavAgent.destination = -Destination + new Vector3(transform.position.x + 70f, transform.position.y, transform.position.z + 70f);
    //            enemyNavAgent.speed = chaseSpeed * 2;
    //            enemyPrefab.gameObject.tag = "Enemy";
    //        }
    //    }
    //}

    //#endregion Evade

    #region Death

    //the function for the death of the enemy 
    protected virtual void Death()
    {
        if (CurrentEnemyHealth > 0)
        {
            return;
        }

        if (!IsEnemyDead)
        {
            KillEnemy();
        }
       
    }

    private void KillEnemy()
    {
        SetIsEnemyDead(true);
        //IsEnemyDead = true;
        EnemyNavAgent.isStopped = true;
        //IsEnemyDead = true;
        Animator.SetTrigger(DieAnimationName);
        Destroy(gameObject, DeathTime);
        EnemyPrefab = null;
    }

    #endregion Death

    #endregion EnemyBehaviour

    #region HealthAndDamage

    protected virtual float CalculatingEnemyHealth()
    {
        return CurrentEnemyHealth / MaximumEnemyHealth;
    }

    //the function for if the player is damaging the enemy on collision
    public void DoDamage(float amount)
    {
        //IsEnemyHit = true;
        CurrentEnemyHealth -= amount;
        Animator.SetTrigger(TakeDamageAnimationName);

        StartCoroutine(SettingTheEnemyHItToFalse());

        if (CurrentEnemyHealth <= 0)
        {
            Death();
            //IsEnemyHit = false;
        }
    }

    protected IEnumerator SettingTheEnemyHItToFalse()
    {
        yield return new WaitForSeconds(DamageTime);
        //IsEnemyHit = false;
        StopCoroutine(SettingTheEnemyHItToFalse());
    }

    #endregion HealthAndDamage

    //#region OnTriggerEnterFunctions

    //protected virtual void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("GroundingAttack") || other.CompareTag("FrozenAttack"))
    //    {
    //        enemyNavAgent.isStopped = true;
    //    }

    //    if(other.CompareTag("Molten"))
    //    {
    //        DoDamage(FindObjectOfType<MoltenEnemyDamage>().initialDamage);
    //    }
    //}

    //protected virtual void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("GroundingAttack") || other.CompareTag("FrozenAttack"))
    //    {
    //        enemyNavAgent.isStopped = false;
    //    }
    //}

    //#endregion OnTriggerEnterFunctions

}