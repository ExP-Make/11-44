using UnityEngine;

public static class ComponentUtility
{
    public static T GetOrAdd<T>(this GameObject go) where T : Component
    {
        var comp = go.GetComponent<T>();
        return comp != null ? comp : go.AddComponent<T>();
    }
} 