using UnityEngine;
using UnityEngine.UI;

public class ItemSelector : MonoBehaviour
{
    [Header("UI参照")]
    public GridLayoutGroup itemGrid;
    public RectTransform cursor;

    [Header("設定")]
    public int columns = 4;

    private int currentIndex = 0;
    private RectTransform[] slots;

    void Start()
    {
        // スロットを取得
        slots = new RectTransform[itemGrid.transform.childCount];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = itemGrid.transform.GetChild(i).GetComponent<RectTransform>();
        }

        UpdateCursor();
        // テキストUIを使わないので、UpdateItemInfo()は削除
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // 移動（キーボード + 左スティック簡易対応）
        if (Input.GetKeyDown(KeyCode.D) || Input.GetAxisRaw("Horizontal") > 0.5f) { Move(1, 0); }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetAxisRaw("Horizontal") < -0.5f) { Move(-1, 0); }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetAxisRaw("Vertical") > 0.5f) { Move(0, 1); }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetAxisRaw("Vertical") < -0.5f) { Move(0, -1); }

        // 決定ボタンで装備・使用（キーボード:Space / Xbox: A）
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
            Debug.Log("空スロット");
            return;
        }

        // 装備可能なら装備だけ行う
        if (!item.usable)
        {
            Inventory.Instance.EquipItem(item.itemID);
            Debug.Log($"{item.itemName} を装備しました");
        }
        else
        {
            // usable=true でも、ここでは「装備」として扱う
            Inventory.Instance.EquipItem(item.itemID);
            Debug.Log($"{item.itemName} を装備しました（使用はゲーム中で）");
        }

        // ポーズ解除は外側（PauseMenuなど）で制御
    }

    ItemData GetItemBySlot(int index)
    {
        var ownedIDs = Inventory.Instance.GetOwnedIDs();
        if (index < 0 || index >= ownedIDs.Count) return null;
        return Inventory.Instance.itemDatabase.GetItemByID(ownedIDs[index]);
    }

    void UseItem(ItemData item)
    {
        switch (item.itemID)
        {
            case 0: // 弓
                Debug.Log("弓を使用 → 矢を放つ");
                break;
            case 1: // 爆弾
                Instantiate(item.itemPrefab, Player.Instance.transform.position, Quaternion.identity);
                Debug.Log("爆弾を投げた！");
                break;
        }
    }
}
