using UnityEngine;

public static class EffectUtility
{
    public static void PlayEffect(GameObject effectPrefab, Vector3 position, float duration = 1f)
    {
        if (effectPrefab == null) return;
        var obj = Object.Instantiate(effectPrefab, position, Quaternion.identity);
        Object.Destroy(obj, duration);
    }

    public static void PlaySoundAtPoint(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
} 