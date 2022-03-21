/*
 * This child class is a template as to how to add and set up variables for enemies
 * just call the setters of each of the variables that need to be changed.
 * 
 */

public class ChildOfBaseEnemy : BaseEnemy
{
    protected override void Start()
    {
        SetMaximumEnemyHealth(100f);
        SetSafeDistance(20f);
        SetRangeAttackDistance(15f);
        SetAttackRange(4f);
        SetChaseRange(7f);
        SetStartTimeBetweenShots(3f);
        SetDamageTime(1.5f);
        SetWalkRadius(50f);
        SetWalkSpeed(3f);
        SetChaseSpeed(6f);
        SetDeathTime(1.5f);
        SetStartTimeBetweenHits(1.5f);
        SetRunAwayStartTime(5f);
        SetEnemyTagName(gameObject.tag = "Enemy");
        base.Start();
    }
}