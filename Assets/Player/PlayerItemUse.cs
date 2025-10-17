using UnityEngine;
using UnityEngine.UI;

public class PlayerItemUse : MonoBehaviour
{
    [Header("UI")]
    public Image equippedItemIcon; // EquippedItemIcon をアサイン
    public Sprite defaultSprite;   // 空のときの透明アイコンなど

    private Vector2 lastMoveDir = Vector2.down;
    void Start()
    {
        
        // 初期所持アイテム
        Inventory.Instance.AddItem(0); // 弓
        Inventory.Instance.AddItem(1); // 爆弾
        Inventory.Instance.AddItem(2); // 剣（常時判定用）

        var item = Inventory.Instance.GetSelectedItem();
        Debug.Log($"選択中アイテム: {item.itemName}");
    }
    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isPause)
        {
            Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (inputDir.sqrMagnitude > 0.01f) lastMoveDir = inputDir.normalized;

            if (Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.JoystickButton2))
            {
                UseEquippedItem();
            }

            if (Input.GetKeyDown(KeyCode.Q) || (Input.GetKeyDown(KeyCode.JoystickButton4)))
            {
                Status.Instance.RecoverMP(10);
            }


        }
        UpdateEquippedItemUI();
    }

    void UpdateEquippedItemUI()
    {
        var item = Inventory.Instance.GetEquippedItem();
        if (item != null && item.icon != null)
        {
            equippedItemIcon.sprite = item.icon;
            equippedItemIcon.color = Color.white;
        }
        else
        {
            equippedItemIcon.sprite = defaultSprite;
            equippedItemIcon.color = new Color(1, 1, 1, 0); // 透明にする
        }
    }

    void UseEquippedItem()
    {
        var item = Inventory.Instance.GetEquippedItem();
        if (item == null)
        {
            Debug.Log("装備アイテムがありません");
            return;
        }

        if (!Status.Instance.UseMP(item.mpCost))
        {
            Debug.Log("MPが足りません！");
            return;
        }

        switch (item.itemID)
        {
            case 0: ShootArrow(item); 
                break;
            case 1:
                if (item.itemPrefab != null)
                {
                    Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                }
                break;
            default: Debug.Log($"{item.itemName} を使用した！");
                break;
        }
    }

    void ShootArrow(ItemData bowItem)
    {
        if (bowItem.itemPrefab == null)
        {
            Debug.LogWarning("弓のPrefabが設定されていません！");
            return;
        }

        Vector3 spawnPos = transform.position + (Vector3)lastMoveDir * 0.5f;
        float angle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        GameObject arrow = Instantiate(bowItem.itemPrefab, spawnPos, rot);

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = lastMoveDir * 10f;
        }
    }
    
}
