/**
* 아이템 런타임 인스턴스화
*/
public class InventoryItem
{
    public ItemData itemData; // 아이템 데이터
    public int quantity; // 아이템 수량
    public bool isEquipped; // 장착 여부
    public bool isObtained; // 획득 여부

    public InventoryItem(ItemData _itemData, int _quantity = 1)
    {
        itemData = _itemData;
        quantity = _quantity;
        isEquipped = false;
        isObtained = true; // 기본적으로 아이템은 획득된 상태로 시작
    }
}