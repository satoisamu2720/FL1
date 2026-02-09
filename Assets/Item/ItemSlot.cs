using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public int itemID;
    public Image icon;

    void Start()
    {
        Refresh();

    }

    public void Refresh()
    {
        if (Inventory.Instance == null) return;

        bool hasItem = Inventory.Instance.GetOwnedIDs().Contains(itemID);

        gameObject.SetActive(hasItem);

    }

    public void OnSelectItem()
    {
        Debug.Log("Selected item ID: " + itemID);
    }
}
