//this is an inherited class of Base Enemy.
public class AnotherChildOfBaseEnemy : BaseEnemy
{
    protected override void Start()
    {
        SetMaximumEnemyHealth(75);
        SetSafeDistance(300);
        SetRangeAttackDistance(25f);
        SetAttackRange(4f);
        SetChaseRange(7f);
        SetStartTimeBetweenShots(3f);
        SetDamageTime(1.5f);
        SetWalkRadius(15f);
        SetWalkSpeed(4f);
        SetChaseSpeed(8f);
        SetDeathTime(1.5f);
        SetStartTimeBetweenHits(1.5f);
        SetRunAwayStartTime(5f);
        SetEnemyTagName(gameObject.tag = "Enemy");
        base.Start();
    }
}
