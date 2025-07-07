using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; } 
    public Inventory inventory { get; private set; } 
    public PlayerManager playerManager { get; private set; } 
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        inventory = GetComponent<Inventory>();
        playerManager = GetComponent<PlayerManager>();

        if (inventory == null || playerManager == null)
            Debug.LogError("Inventory 또는 PlayerManager를 찾을 수 없습니다.");
    }
}
