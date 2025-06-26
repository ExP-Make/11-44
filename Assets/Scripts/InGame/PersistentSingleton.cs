using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance;

    protected virtual void Awake()
    {
        // 이미 존재하는 인스턴스가 있으면 자신은 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 처음 생성된 경우
        Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}