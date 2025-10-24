using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item Data", fileName = "NewItemData")]
public class ItemData : ScriptableObject
{
    public int itemID;           // ��ӂ�ID
    public string itemName;      // �\����
    public Sprite icon;          // UI�A�C�R��
    public string description;   // ������
    public bool usable; // �g�p�\���i�|�E���e�Ȃǁj
    public ItemType itemType;    // �A�C�e�����

    [Header("�A�C�e���̃I�u�W�F�N�g")]
    public GameObject itemPrefab;
    [Header("���͏���")]
    public int mpCost = 0;       // �g�p���ɏ���閂��

    public ItemData(int id, string name, bool usable)
    {
        itemID = id;
        itemName = name;
        this.usable = usable;
    }
    public enum ItemType
    {
        Consumable,  // ����A�C�e���i��F�񕜖�j
        Equipment,   // �����i��F���A���j
        Tool         // �g�p�n�i��F�|�A���e�j
    }
}
