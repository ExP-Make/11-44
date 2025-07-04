using UnityEngine;

public static class ProjectileUtility
{
    public static GameObject FireProjectile(GameObject prefab, Vector2 position, Vector2 direction, float speed, float damage, float lifeTime = 5f, Transform parent = null)
    {
        if (prefab == null) return null;
        GameObject obj = Object.Instantiate(prefab, position, Quaternion.identity, parent);
        var rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * speed;

        var proj = obj.GetComponent<UniversalProjectile>();
        if (proj != null)
            proj.Initialize(damage, lifeTime, false);

        return obj;
    }

    public static void FireSpread(GameObject prefab, Vector2 position, Vector2 direction, float speed, float damage, int count, float spreadAngle, float lifeTime = 5f, Transform parent = null)
    {
        float startAngle = -spreadAngle * (count - 1) / 2f;
        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + spreadAngle * i;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * direction;
            FireProjectile(prefab, position, dir, speed, damage, lifeTime, parent);
        }
    }
} 