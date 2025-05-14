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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.material = outlineMaterial; // 외곽선 표시
        }
    }

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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.material = defaultMaterial; // 원래 머티리얼로 복원
        }
    }
}