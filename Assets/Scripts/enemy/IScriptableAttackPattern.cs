using UnityEngine;
using System.Collections;

public interface IScriptableAttackPattern
{
    float Cooldown { get; }
    float Range { get; }
    IEnumerator ExecuteAttack(UniversalEnemyController controller, Transform player);
    bool CanExecute(UniversalEnemyController controller, Transform player);
}

public abstract class ScriptableAttackPattern : ScriptableObject, IScriptableAttackPattern
{
    [Header("Base Attack Properties")]
    [SerializeField] private float range = 5f;
    [SerializeField] private float cooldown = 2f;
    
    public float Range => range;
    public float Cooldown => cooldown;
    
    public abstract IEnumerator ExecuteAttack(UniversalEnemyController controller, Transform player);

    public virtual bool CanExecute(UniversalEnemyController controller, Transform player)
    {
        if (player == null) return false;
        
        float distanceToPlayer = Vector2.Distance(controller.transform.position, player.position);
        return distanceToPlayer <= range;
    }
}