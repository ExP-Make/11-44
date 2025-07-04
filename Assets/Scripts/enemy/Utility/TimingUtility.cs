using System.Collections;
using UnityEngine;

public static class TimingUtility
{
    public static IEnumerator WaitAndDo(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
} 