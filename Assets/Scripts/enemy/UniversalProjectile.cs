using UnityEngine;

public class UniversalProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float damage = 10f;
    public float lifeTime = 5f;
    public bool canHoming = false;
    public float homingStrength = 2f;
    public float homingRange = 8f;
    
    [Header("Area Effect")]
    public bool hasAreaEffect = false;
    public float explosionRadius = 2f;
    public float areaDamageMultiplier = 0.7f;
    
    [Header("Visual Effects")]
    public GameObject trailEffect;
    public GameObject impactEffect;
    public GameObject explosionEffect;
    
    [Header("Audio")]
    public AudioClip fireSound;
    public AudioClip impactSound;
    public AudioClip explosionSound;
    
    private Rigidbody2D rb;
    private Transform player;
    private float spawnTime;
    private bool hasExploded;
    private AudioSource audioSource;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        spawnTime = Time.time;
        
        if (canHoming)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        
        if (trailEffect != null)
        {
            Instantiate(trailEffect, transform.position, transform.rotation, transform);
        }
        
        if (fireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSound);
        }
        
        Destroy(gameObject, lifeTime);
    }
    
    void Update()
    {
        if (canHoming && player != null && !hasExploded)
        {
            PerformHoming();
        }
        
        if (Time.time - spawnTime > lifeTime)
        {
            Explode();
        }
    }
    
    void PerformHoming()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= homingRange)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            Vector2 currentVelocity = rb.linearVelocity;
            
            Vector2 newVelocity = Vector2.Lerp(currentVelocity.normalized, directionToPlayer, homingStrength * Time.deltaTime);
            rb.linearVelocity = newVelocity * currentVelocity.magnitude;
            
            float angle = Mathf.Atan2(newVelocity.y, newVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DealDamageToPlayer(other);
            
            if (hasAreaEffect)
            {
                Explode();
            }
            else
            {
                Impact();
            }
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Obstacle"))
        {
            if (hasAreaEffect)
            {
                Explode();
            }
            else
            {
                Impact();
            }
        }
    }
    
    void DealDamageToPlayer(Collider2D player)
    {
        var playerManager = player.GetComponent<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.TakeDamage(damage);
        }
    }
    
    void Impact()
    {
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, transform.rotation);
        }
        
        if (impactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(impactSound);
        }
        
        Destroy(gameObject);
    }
    
    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;
        
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }
        
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                float damageMultiplier = 1f - (distance / explosionRadius);
                float finalDamage = damage * damageMultiplier * areaDamageMultiplier;

                var playerManager = hit.GetComponent<PlayerManager>();
                if (playerManager != null)
                {
                    playerManager.TakeDamage(finalDamage);
                }
            }
        }
        
        Destroy(gameObject, 0.5f);
    }
    
    public void Initialize(float projectileDamage, float projectileLifetime, bool enableHoming)
    {
        damage = projectileDamage;
        lifeTime = projectileLifetime;
        canHoming = enableHoming;
    }
    
    void OnDrawGizmosSelected()
    {
        if (canHoming)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, homingRange);
        }
        
        if (hasAreaEffect)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
} 