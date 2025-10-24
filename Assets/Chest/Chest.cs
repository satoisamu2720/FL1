using UnityEngine;

public class Chest : MonoBehaviour
{

    [Header("�󔠐ݒ�")]
    public int chestID;          // ��ID
    public int itemIDToGive;    // �󔠂���o��A�C�e��ID
    public bool isOpened = false; // �󔠂��J���Ă��邩�ǂ���



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void OpenChest(PlayerInventory playerInventory)
    {
        if(isOpened) return;

        isOpened = true;
        Debug.Log("�󔠂��J���܂����I �A�C�e��ID: " + itemIDToGive);
        playerInventory.AddItem(itemIDToGive); // �v���C���[�̃C���x���g���ɃA�C�e����ǉ����鏈���������ɏ���

    }
}
