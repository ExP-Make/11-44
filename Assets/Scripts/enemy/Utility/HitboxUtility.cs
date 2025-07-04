using UnityEngine;

public static class HitboxUtility
{
    public static Collider2D[] BoxHit(Vector2 center, Vector2 size, float angle, LayerMask mask)
    {
        return Physics2D.OverlapBoxAll(center, size, angle, mask);
    }

    public static Collider2D[] CircleHit(Vector2 center, float radius, LayerMask mask)
    {
        return Physics2D.OverlapCircleAll(center, radius, mask);
    }
} 