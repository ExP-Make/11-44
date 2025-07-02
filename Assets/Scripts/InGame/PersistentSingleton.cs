using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        // �̹� �����ϴ� �ν��Ͻ��� ������ �ڽ��� �ı�
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // ó�� ������ ���
        Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}