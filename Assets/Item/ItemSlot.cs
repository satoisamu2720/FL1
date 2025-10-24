using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public int itemID; // �A�C�e�����Ƃ�ID
    public Image icon;

    public void OnSelectItem()
    {
        Debug.Log("Selected item ID: " + itemID);
        Inventory.Instance.AddItem(itemID);
    }
}
