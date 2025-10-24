using UnityEngine;

public class Chest : MonoBehaviour
{

    [Header("宝箱設定")]
    public int chestID;          // 宝箱ID
    public int itemIDToGive;    // 宝箱から出るアイテムID
    public bool isOpened = false; // 宝箱が開いているかどうか



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void OpenChest(PlayerInventory playerInventory)
    {
        if(isOpened) return;

        isOpened = true;
        Debug.Log("宝箱が開きました！ アイテムID: " + itemIDToGive);
        playerInventory.AddItem(itemIDToGive); // プレイヤーのインベントリにアイテムを追加する処理をここに書く

    }
}
