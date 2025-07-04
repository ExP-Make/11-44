using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New GhostKillPassive", menuName = "Enemy/Passives/Ghost Kill")]
public class GhostKillPassive : ScriptableSpecialPassive
{
    [Header("Ghost Kill Specifics")]
    [Tooltip("Number of minions to spawn on death")]
    public int minionSpawnCount = 3;
    public GameObject minionPrefab;
    
    private bool isSubscribed = false;

    protected override void OnActivate()
    {
        if (!isSubscribed)
        {
            EventBus.Subscribe("GhostKill", OnGhostKillEventReceived);
            isSubscribed = true;
        }
    }

    protected override void OnDeactivate()
    {
        if (isSubscribed)
        {
            EventBus.Unsubscribe("GhostKill", OnGhostKillEventReceived);
            isSubscribed = false;
        }
    }

    private void OnGhostKillEventReceived(object[] args)
    {
        if (currentEnemy != null && currentEnemy.IsAlive())
        {
            currentEnemy.TakeDamage(currentEnemy.GetCurrentHealth());
        }
    }

    public override void Activate(UniversalEnemyController enemy)
    {
        base.Activate(enemy);
        if (trigger == PassiveTrigger.OnDeath)
        {
            SpawnMinions(minionPrefab, minionSpawnCount, 2f);
        }
    }

    protected override IEnumerator PassiveEffect()
    {
        // Logic is handled in Activate, no coroutine needed.
        yield return null;
    }

    public override void PassiveUpdate(UniversalEnemyController enemy)
    {
        // No continuous update logic is needed for this passive.
    }
} 