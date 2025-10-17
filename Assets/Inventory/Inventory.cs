using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public ItemDatabase itemDatabase;

    private List<int> ownedItemIDs = new List<int>();
    private int selectedIndex = 0;

    //装備中のアイテムを保持
    private ItemData equippedItem;

    public void AddItem(int id)
    {
        if (!ownedItemIDs.Contains(id)) ownedItemIDs.Add(id);
    }

    public List<int> GetOwnedIDs() => ownedItemIDs;

    public ItemData GetSelectedItem()
    {
        if (ownedItemIDs.Count == 0) return null;
        int id = ownedItemIDs[selectedIndex];
        return itemDatabase.GetItemByID(id);
    }

    public void EquipItem(int id)
    {
        var item = itemDatabase.GetItemByID(id);
        if (item != null)
        {
            equippedItem = item;
            Debug.Log($"{item.itemName} を装備しました");
        }
    }

    //現在装備中アイテムを取得
    public ItemData GetEquippedItem()
    {
        return equippedItem;
    }

    public bool HasItem(int id) => ownedItemIDs.Contains(id);

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
