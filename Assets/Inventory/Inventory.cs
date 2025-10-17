using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public ItemDatabase itemDatabase;

    private List<int> ownedItemIDs = new List<int>();
    private int selectedIndex = 0;

    //�������̃A�C�e����ێ�
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
            Debug.Log($"{item.itemName} �𑕔����܂���");
        }
    }

    //���ݑ������A�C�e�����擾
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
