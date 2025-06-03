/**
* 아이템 데이터용 클래스
*/
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    [TextArea] public string description;
    public Sprite[] icon;
    public ItemType itemType;

    // Equipment
    public int attackPower;
    public int defensePower;

    // Consumable
    public int healAmount;
}
