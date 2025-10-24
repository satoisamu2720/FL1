using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public ItemDatabase itemDatabase;

    private List<int> ownedItemIDs = new List<int>();
    private int selectedIndex = 0;

    //�������̃A�C�e����ێ�
    private ItemData equippedItem;

    private int equippedItemID = -1;

    private bool hasSword = false;

    public void AddItem(int id)
    {
        var item = itemDatabase.GetItemByID(id);
        if (item == null)
        {
            Debug.LogWarning($"�A�C�e��ID {id} ��������܂���ł����B");
            return;
        }

        // ���łɏ������Ă���ꍇ�͒ǉ����Ȃ�
        if (ownedItemIDs.Contains(id))
        {
            Debug.Log($"{item.itemName} �͂��łɏ������Ă��܂��B");
            return;
        }

        // �g�p�\�ȃA�C�e���i�|�E���e�Ȃǁj�̓C���x���g���ɒǉ�
        if (item.usable)
        {
            ownedItemIDs.Add(id);
            Debug.Log($"{item.itemName} ���C���x���g���ɒǉ����܂����B");
        }
        else if (item.itemType == ItemData.ItemType.Equipment)
        {
            // �����^�C�v�͑������X�g�ɓo�^���邪�A�C���x���g���ɂ͒ǉ����Ȃ�
            EquipItem(id);
            Debug.Log($"{item.itemName} �𑕔����X�g�ɓo�^���܂����i�C���x���g���ɂ͒ǉ����܂���j�B");
        }
    }


    public void EquipItem(int itemID)
    {
        var item = itemDatabase.GetItemByID(itemID);
        if (item == null) return;

        equippedItem = item;

        if (item.itemID == 0)
            hasSword = true; // ����o�^�ς݂ɂ���
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
            Debug.LogWarning($"�A�C�e��ID {itemID} �͑��݂��܂���");
            return;
        }

        equippedItem = item;
        equippedItemID = itemID;
        Debug.Log($"{item.itemName} �𑕔����܂����i�C���x���g���ɂ͒ǉ����܂���j");
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
