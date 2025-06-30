/**
* 씬 내 아이템 오브젝트에 적용하는 스크립트
*/
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemObject : MonoBehaviour
{
    public ItemData itemData; // 아이템 데이터
    public int quantity = 1; // 아이템 수량
    public Material outlineMaterial; // 하이라이트용 머티리얼
    private Material defaultMaterial; // 원래 머티리얼
    private SpriteRenderer spriteRenderer;
    private bool isPlayerinRange = false;
    private Inventory currentInventory;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //Debug.Log(spriteRenderer);

        // Collider2D 컴포넌트가 없으면 추가
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = itemData.icon[0];
            defaultMaterial = spriteRenderer.material;
        }
    }

    void Update()
    {
        if (!isPlayerinRange || itemData == null || currentInventory == null) return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!GameManager.Instance.isDialogOpen)
            {
                GameManager.Instance.ShowDialog($"{itemData.itemName}을 얻었다.");
            }
            else if (GameManager.Instance.isDialogOpen)
            {
                currentInventory.AddItem(itemData, quantity); // 아이템 획득 처리
                // TODO: 이미 획득한 아이템 맵에서 숨기기를 위한 작업
                // SaveData data = SaveSystem.LoadGame();
                // if (!data.obtainedItemIds.Contains(itemData.id)) 
                // {
                //     data.obtainedItemIds.Add(itemData.id); // 아이템 획득 기록 추가
                //     SaveSystem.SaveGame(data); // 저장
                // }
                GameManager.Instance.HideDialog();
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 플레이어가 아이템에 접촉 시 외곽선 표시
    /// TODO: 이후 Material이 아닌 별도 이미지로 바꾸도록 변경 필요
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory == null) return;
            isPlayerinRange = true;
            currentInventory = inventory; 
            spriteRenderer.material = outlineMaterial; // 외곽선 표시
        }
    }

    /// <summary>
    /// 아이템에서 떨어지면 외곽선 해제
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        isPlayerinRange = false;
        currentInventory = null; // 현재 인벤토리 초기화
        spriteRenderer.material = defaultMaterial; // 원래 머티리얼로 복원
    }
}