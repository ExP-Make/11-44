/**
* 씬 내 아이템 오브젝트에 적용하는 스크립트
*/
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemObject : MonoBehaviour
{
    public ItemData itemData; // 아이템 데이터
    public int quantity = 1; // 아이템 수량

    private void Start()
    {
        // Collider2D 컴포넌트가 없으면 추가
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        // 아이템 아이콘 적용
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && itemData != null)
        {
            spriteRenderer.sprite = itemData.icon;
        }
        else
        {
            Debug.LogWarning("아이템 데이터가 설정되지 않았습니다.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null && itemData != null)
            {
                inventory.AddItem(itemData, quantity);
                Debug.Log($"아이템 획득: {itemData.itemName} x{quantity}");
                Destroy(gameObject); // 아이템을 줍고 나면 오브젝트 삭제
            }
        }
    }
}