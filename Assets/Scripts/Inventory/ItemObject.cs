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
            DialogManager.Instance.StartDialog(itemData.dialogSequence);
            currentInventory.AddItem(itemData, quantity); // 아이템 획득 처리
            currentInventory.RegisterObtainedItem(itemData.id); // 아이템 획득 현황 등록
            Destroy(gameObject);
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