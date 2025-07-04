using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New RandomAOEBlastPattern", menuName = "Enemy/Attack Patterns/Random AOE Blast")]
public class RandomAOEBlastPattern : ScriptableAttackPattern
{
    [Header("AOE Blast Specific")]
    public GameObject aoePrefab;
    public int numberOfBlasts = 5;
    public float blastRadius = 3f;
    public float delayBetweenBlasts = 0.5f;

    public override IEnumerator ExecuteAttack(UniversalEnemyController controller, Transform player)
    {
        for (int i = 0; i < numberOfBlasts; i++)
        {
            Vector2 randomPosition = (Vector2)player.position + Random.insideUnitCircle * blastRadius;
            
            if (aoePrefab != null)
            {
                GameObject.Instantiate(aoePrefab, randomPosition, Quaternion.identity);
            }

            yield return new WaitForSeconds(delayBetweenBlasts);
        }
    }
} 