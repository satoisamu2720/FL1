using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item Data", fileName = "NewItemData")]
public class ItemData : ScriptableObject
{
    public int itemID;           // 一意のID
    public string itemName;      // 表示名
    public Sprite icon;          // UIアイコン
    public string description;   // 説明文
    public bool usable; // 使用可能か（弓・爆弾など）
    public ItemType itemType;    // アイテム種別

    [Header("アイテムのオブジェクト")]
    public GameObject itemPrefab;
    [Header("魔力消費")]
    public int mpCost = 0;       // 使用時に消費する魔力

    public ItemData(int id, string name, bool usable)
    {
        itemID = id;
        itemName = name;
        this.usable = usable;
    }
    public enum ItemType
    {
        Consumable,  // 消費アイテム（例：回復薬）
        Equipment,   // 装備（例：剣、盾）
        Tool         // 使用系（例：弓、爆弾）
    }
}
