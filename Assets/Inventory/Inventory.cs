using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public ItemDatabase itemDatabase;

    private List<int> ownedItemIDs = new List<int>();
    private int selectedIndex = 0;

    //装備中のアイテムを保持
    private ItemData equippedItem;

    private int equippedItemID = -1;

    private bool hasSword = false;

    public void AddItem(int id)
    {
        var item = itemDatabase.GetItemByID(id);
        if (item == null)
        {
            Debug.LogWarning($"アイテムID {id} が見つかりませんでした。");
            return;
        }

        // すでに所持している場合は追加しない
        if (ownedItemIDs.Contains(id))
        {
            Debug.Log($"{item.itemName} はすでに所持しています。");
            return;
        }

        // 使用可能なアイテム（弓・爆弾など）はインベントリに追加
        if (item.usable)
        {
            ownedItemIDs.Add(id);
            Debug.Log($"{item.itemName} をインベントリに追加しました。");
        }
        else if (item.itemType == ItemData.ItemType.Equipment)
        {
            // 装備タイプは装備リストに登録するが、インベントリには追加しない
            EquipItem(id);
            Debug.Log($"{item.itemName} を装備リストに登録しました（インベントリには追加しません）。");
        }
    }


    public void EquipItem(int itemID)
    {
        var item = itemDatabase.GetItemByID(itemID);
        if (item == null) return;

        equippedItem = item;

        if (item.itemID == 0)
            hasSword = true; // 剣を登録済みにする
    }

    public bool HasSword()
    {
        return hasSword;
    }
    public void EquipWithoutAdding(int itemID)
    {
        ItemData item = itemDatabase.GetItemByID(itemID);
        if (item == null)
        {
            Debug.LogWarning($"アイテムID {itemID} は存在しません");
            return;
        }

        equippedItem = item;
        equippedItemID = itemID;
        Debug.Log($"{item.itemName} を装備しました（インベントリには追加しません）");
    }

    public ItemData GetEquippedItem()
    {
        return equippedItem;
    }

    public ItemData GetSelectedItem()
    {
        if (ownedItemIDs.Count == 0) return null;
        return itemDatabase.GetItemByID(ownedItemIDs[0]);
    }

    public List<int> GetOwnedIDs() => ownedItemIDs;


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
