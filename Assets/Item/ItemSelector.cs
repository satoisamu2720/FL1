using UnityEngine;
using UnityEngine.UI;

public class ItemSelector : MonoBehaviour
{
    [Header("UI�Q��")]
    public GridLayoutGroup itemGrid;
    public RectTransform cursor;

    [Header("�ݒ�")]
    public int columns = 4;

    private int currentIndex = 0;
    private RectTransform[] slots;

    void Start()
    {
        // �X���b�g���擾
        slots = new RectTransform[itemGrid.transform.childCount];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = itemGrid.transform.GetChild(i).GetComponent<RectTransform>();
        }

        UpdateCursor();
        // �e�L�X�gUI���g��Ȃ��̂ŁAUpdateItemInfo()�͍폜
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // �ړ��i�L�[�{�[�h + ���X�e�B�b�N�ȈՑΉ��j
        if (Input.GetKeyDown(KeyCode.D) || Input.GetAxisRaw("Horizontal") > 0.5f) { Move(1, 0); }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetAxisRaw("Horizontal") < -0.5f) { Move(-1, 0); }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetAxisRaw("Vertical") > 0.5f) { Move(0, 1); }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetAxisRaw("Vertical") < -0.5f) { Move(0, -1); }

        // ����{�^���ő����E�g�p�i�L�[�{�[�h:Space / Xbox: A�j
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            SelectItem();
        }
    }
    void Move(int x, int y)
    {
        int newIndex = currentIndex + x + (y * columns);

        if (newIndex >= 0 && newIndex < slots.Length)
        {
            currentIndex = newIndex;
            UpdateCursor();
        }
    }

    void UpdateCursor()
    {
        if (cursor != null && currentIndex < slots.Length)
        {
            cursor.position = slots[currentIndex].position;
        }
    }

    void SelectItem()
    {
        var item = GetItemBySlot(currentIndex);
        if (item == null)
        {
            Debug.Log("��X���b�g");
            return;
        }

        if (!item.usable)
        {
            Inventory.Instance.EquipItem(item.itemID);
            Debug.Log($"{item.itemName} �𑕔����܂���");
        }
        else
        {
            Inventory.Instance.EquipWithoutAdding(item.itemID);
            Debug.Log($"{item.itemName} �𑕔����܂���");
        }

    }

    ItemData GetItemBySlot(int index)
    {
        var ownedIDs = Inventory.Instance.GetOwnedIDs();
        if (index < 0 || index >= ownedIDs.Count) return null;
        return Inventory.Instance.itemDatabase.GetItemByID(ownedIDs[index]);
    }
}
