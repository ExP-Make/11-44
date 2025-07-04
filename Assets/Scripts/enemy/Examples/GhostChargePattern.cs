using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New GhostChargePattern", menuName = "Enemy/Attack Patterns/Ghost Charge")]
public class GhostChargePattern : ScriptableAttackPattern
{
    [Header("Charge Specific")]
    public float chargeSpeed = 15f;
    public float chargeDuration = 0.5f;
    public float damage = 10f;
    public float hitboxRadius = 0.5f;
    public TrailRenderer trail;

    public override IEnumerator ExecuteAttack(UniversalEnemyController controller, Transform player)
    {
        if (player == null) yield break;

        Vector2 direction = (player.position - controller.transform.position).normalized;
        float originalSpeed = controller.GetOriginalSpeed();
        
        controller.SetSpeed(chargeSpeed);
        
        if(trail) trail.emitting = true;

        float timer = 0f;
        while(timer < chargeDuration)
        {
            controller.Move(direction);
            
            Collider2D[] hits = HitboxUtility.CircleHit(controller.transform.position, hitboxRadius, controller.PlayerLayer);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    hit.GetComponent<PlayerManager>()?.TakeDamage(damage);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if(trail) trail.emitting = false;
        controller.SetSpeed(originalSpeed);
    }
} 