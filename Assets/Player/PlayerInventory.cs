using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<int> itemIDs = new List<int>();

    public void AddItem(int itemID)
    {
        itemIDs.Add(itemID);
        Debug.Log("�C���x���g��:{id}��ǉ�" + itemID);
    }

    public bool HasItem(int itemID)
    {
        return itemIDs.Contains(itemID);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
