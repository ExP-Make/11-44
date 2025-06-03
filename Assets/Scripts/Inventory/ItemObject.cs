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

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log(spriteRenderer);
         
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

    /// <summary>
    /// 플레이어가 아이템에 접촉 시 외곽선 표시
    /// TODO: 이후 Material이 아닌 별도 이미지로 바꾸도록 변경 필요
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.material = outlineMaterial; // 외곽선 표시
        }
    }

    /// <summary>
    /// 플레이어가 접촉 중,  F키 입력 시 아이템 획득 처리. 
    /// TODO: 이후 모바일 버튼 입력으로 대체 필요
    /// </summary>
    /// <param name="other">접촉 오브젝트 (플레이어)</param>
    private void OnTriggerStay2D(Collider2D other)
    {
        Inventory inventory = other.GetComponent<Inventory>();
        if (other.CompareTag("Player") && inventory != null && itemData != null && Input.GetKeyDown(KeyCode.F))
        {
            inventory.AddItem(itemData, quantity);
            Debug.Log($"아이템 획득: {itemData.itemName} x{quantity}");
            Destroy(gameObject); // 아이템을 줍고 나면 오브젝트 삭제
        }
    }

    /// <summary>
    /// 아이템에서 떨어지면 외곽선 해제
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.material = defaultMaterial; // 원래 머티리얼로 복원
        }
    }
}