using System.Collections;
using UnityEngine;

public class PeriodicHealPassive : ScriptableSpecialPassive
{
    [Header("Heal Settings")]
    [Tooltip("힐 주기(초). ScriptableSpecialPassive.intervalTime 를 덮어쓴다")]
    public float healInterval = 3f;

    [Tooltip("회복량을 MaxHP 대비 백분율로 지정")] public float healPercent = 0.01f; // 1%

    protected override void OnActivate()
    {
        if (intervalTime <= 0f) intervalTime = healInterval;
    }

    protected override IEnumerator PassiveEffect()
    {
        if (currentEnemy != null && currentEnemy.IsAlive())
        {
            float healAmount = currentEnemy.GetMaxHealth() * healPercent;
            currentEnemy.Heal(healAmount);
        }
        yield return new WaitForSeconds(intervalTime);
    }

    protected override bool AllowReactivation() => true;

    public override void PassiveUpdate(UniversalEnemyController enemy)
    {
        // This passive activates on an interval, so no continuous update logic is needed here.
    }
} 