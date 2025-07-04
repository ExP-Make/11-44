using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SpawnEvent
{
    public string eventName;
    public GameObject enemyPrefab;
    public int enemyCount;
    public float spawnDelay = 0.5f;
    public bool useRandomSpawnPoint = true;
    public Transform specificSpawnPoint;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Event-Based Spawn Settings")]
    public SpawnEvent[] spawnEvents;
    public Transform[] spawnPoints;
    
    [Header("Area Spawn Settings")]
    public bool useAreaSpawn = false;
    public Vector2 spawnAreaSize = new Vector2(10f, 10f);
    public float minDistanceFromPlayer = 5f;
    
    [Header("Spawn Validation")]
    public LayerMask groundLayer;
    public bool checkGroundBeforeSpawn = true;
    public bool checkPlayerDistance = true;
    
    [Header("Auto Spawn (Optional)")]
    public bool enableAutoSpawn = false;
    public float autoSpawnInterval = 2f;
    public int maxAutoSpawnEnemies = 10;
    public GameObject[] autoSpawnPrefabs;
    
    private List<GameObject> activeEnemies = new List<GameObject>();
    private Transform player;
    private Dictionary<string, SpawnEvent> eventDictionary;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (spawnPoints.Length == 0)
        {
            CreateDefaultSpawnPoints();
        }
        
        InitializeEventDictionary();
        
        if (enableAutoSpawn)
        {
            StartCoroutine(AutoSpawnRoutine());
        }
    }
    
    void Update()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
    }
    
    void InitializeEventDictionary()
    {
        eventDictionary = new Dictionary<string, SpawnEvent>();
        
        foreach (SpawnEvent spawnEvent in spawnEvents)
        {
            if (!eventDictionary.ContainsKey(spawnEvent.eventName))
            {
                eventDictionary.Add(spawnEvent.eventName, spawnEvent);
            }
        }
    }
    
    void CreateDefaultSpawnPoints()
    {
        GameObject spawnPointsParent = new GameObject("SpawnPoints");
        spawnPointsParent.transform.SetParent(transform);
        
        spawnPoints = new Transform[4];
        Vector3[] positions = {
            transform.position + Vector3.left * 5f,
            transform.position + Vector3.right * 5f,
            transform.position + Vector3.up * 5f,
            transform.position + Vector3.down * 5f
        };
        
        for (int i = 0; i < 4; i++)
        {
            GameObject spawnPoint = new GameObject($"SpawnPoint_{i}");
            spawnPoint.transform.SetParent(spawnPointsParent.transform);
            spawnPoint.transform.position = positions[i];
            spawnPoints[i] = spawnPoint.transform;
        }
    }
    
    public void TriggerSpawnEvent(string eventName)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            SpawnEvent spawnEvent = eventDictionary[eventName];
            StartCoroutine(ExecuteSpawnEvent(spawnEvent));
            Debug.Log($"Spawn event triggered: {eventName}");
        }
        else
        {
            Debug.LogWarning($"Spawn event not found: {eventName}");
        }
    }
    
    IEnumerator ExecuteSpawnEvent(SpawnEvent spawnEvent)
    {
        for (int i = 0; i < spawnEvent.enemyCount; i++)
        {
            Vector3 spawnPosition = GetSpawnPosition(spawnEvent);
            
            if (spawnPosition != Vector3.zero)
            {
                GameObject spawnedEnemy = Instantiate(spawnEvent.enemyPrefab, spawnPosition, Quaternion.identity);
                activeEnemies.Add(spawnedEnemy);
                
                Debug.Log($"Enemy spawned: {spawnEvent.enemyPrefab.name} at {spawnPosition}");
            }
            
            if (i < spawnEvent.enemyCount - 1)
            {
                yield return new WaitForSeconds(spawnEvent.spawnDelay);
            }
        }
    }
    
    Vector3 GetSpawnPosition(SpawnEvent spawnEvent)
    {
        Vector3 spawnPosition = Vector3.zero;
        
        if (!spawnEvent.useRandomSpawnPoint && spawnEvent.specificSpawnPoint != null)
        {
            spawnPosition = spawnEvent.specificSpawnPoint.position;
        }
        else if (useAreaSpawn)
        {
            spawnPosition = GetAreaSpawnPosition();
        }
        else if (spawnPoints.Length > 0)
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            spawnPosition = randomSpawnPoint.position;
        }
        
        return spawnPosition;
    }
    
    Vector3 GetAreaSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;
        
        for (int attempts = 0; attempts < 10; attempts++)
        {
            Vector2 randomOffset = new Vector2(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
            );
            
            spawnPosition = transform.position + (Vector3)randomOffset;
            
            if (checkPlayerDistance && player != null && Vector2.Distance(spawnPosition, player.position) < minDistanceFromPlayer)
            {
                continue;
            }
            
            if (checkGroundBeforeSpawn && !IsValidSpawnPosition(spawnPosition))
            {
                continue;
            }
            
            break;
        }
        
        return spawnPosition;
    }
    
    bool IsValidSpawnPosition(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position + Vector3.up, Vector2.down, 2f, groundLayer);
        return hit.collider != null;
    }
    
    public void SpawnSpecificEnemy(GameObject enemyPrefab, int count, float delay = 0.5f)
    {
        StartCoroutine(SpawnEnemiesWithDelay(enemyPrefab, count, delay));
    }
    
    IEnumerator SpawnEnemiesWithDelay(GameObject enemyPrefab, int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetAreaSpawnPosition();
            if (spawnPosition != Vector3.zero)
            {
                GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                activeEnemies.Add(spawnedEnemy);
            }
            
            if (i < count - 1)
            {
                yield return new WaitForSeconds(delay);
            }
        }
    }
    
    public void SpawnAtSpecificPoint(GameObject enemyPrefab, Transform spawnPoint, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            activeEnemies.Add(spawnedEnemy);
        }
    }
    
    IEnumerator AutoSpawnRoutine()
    {
        while (enableAutoSpawn)
        {
            if (activeEnemies.Count < maxAutoSpawnEnemies && autoSpawnPrefabs.Length > 0)
            {
                GameObject randomEnemy = autoSpawnPrefabs[Random.Range(0, autoSpawnPrefabs.Length)];
                Vector3 spawnPosition = GetAreaSpawnPosition();
                
                if (spawnPosition != Vector3.zero)
                {
                    GameObject spawnedEnemy = Instantiate(randomEnemy, spawnPosition, Quaternion.identity);
                    activeEnemies.Add(spawnedEnemy);
                }
            }
            
            yield return new WaitForSeconds(autoSpawnInterval);
        }
    }
    
    public void ClearAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        activeEnemies.Clear();
    }
    
    public void StopAllSpawning()
    {
        StopAllCoroutines();
        enableAutoSpawn = false;
    }
    
    public void EnableAutoSpawn()
    {
        enableAutoSpawn = true;
        StartCoroutine(AutoSpawnRoutine());
    }
    
    public int GetActiveEnemyCount()
    {
        return activeEnemies.Count;
    }
    
    public List<string> GetAvailableEvents()
    {
        List<string> events = new List<string>();
        foreach (SpawnEvent spawnEvent in spawnEvents)
        {
            events.Add(spawnEvent.eventName);
        }
        return events;
    }
    
    void OnDrawGizmosSelected()
    {
        if (useAreaSpawn)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, spawnAreaSize);
        }
        
        if (checkPlayerDistance && player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
        
        if (spawnPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                }
            }
        }
    }
} 