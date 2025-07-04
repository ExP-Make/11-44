using UnityEngine;
using System.Collections.Generic;

public class BossPhasePassive : ScriptableSpecialPassive
{
    [System.Serializable]
    public class Phase
    {
        public string phaseName;
        public float healthThreshold;
        public ScriptableAttackPattern[] newAttackPatterns;
        public float speedMultiplier = 1f;
        public bool isInvulnerableDuringTransition = false;
    }

    [Header("Boss Phase Settings")]
    public List<Phase> phases;
    private int currentPhaseIndex = -1;

    public override void Activate(UniversalEnemyController enemy)
    {
        base.Activate(enemy);
        CheckPhaseTransition();
    }

    public override void PassiveUpdate(UniversalEnemyController enemy)
    {
        base.PassiveUpdate(enemy);
        CheckPhaseTransition();
    }
    
    void CheckPhaseTransition()
    {
        if (currentEnemy == null || currentPhaseIndex >= phases.Count - 1) return;

        float healthPercentage = currentEnemy.GetHealthPercentage();
        if (healthPercentage <= phases[currentPhaseIndex + 1].healthThreshold)
        {
            currentPhaseIndex++;
            TransitionToPhase(phases[currentPhaseIndex]);
        }
    }

    void TransitionToPhase(Phase phase)
    {
        Debug.Log($"Transitioning to boss phase: {phase.phaseName}");

        if (phase.isInvulnerableDuringTransition)
        {
            currentEnemy.EnemyHealth.SetInvulnerable(true, 2f); // 2초간 무적
        }
        
        currentEnemy.enemyData.attackPatterns = phase.newAttackPatterns;
        currentEnemy.enemyData.moveSpeed *= phase.speedMultiplier;

        // Optionally, trigger visual effects for phase transition
        if (activationEffect != null)
        {
            Instantiate(activationEffect, currentEnemy.transform.position, Quaternion.identity);
        }
    }

    protected override System.Collections.IEnumerator PassiveEffect()
    {
        // This passive's logic is handled in Update, not in a coroutine.
        yield return null;
    }
} 