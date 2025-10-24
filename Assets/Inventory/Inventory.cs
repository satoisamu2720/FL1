using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public ItemDatabase itemDatabase; // �� Inspector �� ItemDatabase.asset �����蓖�Ă�

    private List<int> ownedItemIDs = new List<int>();
    private int selectedIndex = 0;

    public void AddItem(int id)
    {
        if (!ownedItemIDs.Contains(id)) ownedItemIDs.Add(id);
    }

    public ItemData GetSelectedItem()
    {
        if (ownedItemIDs.Count == 0) return null;
        int id = ownedItemIDs[selectedIndex];
        return itemDatabase.GetItemByID(id);
    }

    public void NextItem()
    {
        if (ownedItemIDs.Count == 0) return;
        int next =  selectedIndex = (selectedIndex + 1) % ownedItemIDs.Count;
        while (ownedItemIDs[next] == 2)
        {
            next = (next + 1 )% ownedItemIDs.Count;
        }

        selectedIndex = next;

        Debug.Log($"�I��: {GetSelectedItem()?.itemName} (ID:{GetSelectedItem()?.itemID})");
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
