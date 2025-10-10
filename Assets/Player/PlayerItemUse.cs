using UnityEngine;

public class PlayerItemUse : MonoBehaviour
{
    private Vector2 lastMoveDir = Vector2.down; // 初期向き
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
            // プレイヤーの移動方向を取得（向きの更新）
            Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (inputDir.sqrMagnitude > 0.01f)
                lastMoveDir = inputDir.normalized;

            // 弓 or 爆弾使用
            if (Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.JoystickButton2))
            {
                UseSelectedItem();
            }

            // アイテム選択切替
            if (Input.GetKeyDown(KeyCode.E) || (Input.GetKeyDown(KeyCode.JoystickButton5)))
            {
                Inventory.Instance.NextItem();
            }

            // アイテム選択切替
            if (Input.GetKeyDown(KeyCode.Q) || (Input.GetKeyDown(KeyCode.JoystickButton4)))
            {
                Status.Instance.RecoverMP(10);
            }

        }
    }

    void UseSelectedItem()
    {
        var item = Inventory.Instance.GetSelectedItem();
        if (item == null)
        {
            Debug.Log("アイテムが選択されていません。");
            return;
        }

        // 魔力チェック
        if (!Status.Instance.UseMP(item.mpCost))
        {
            Debug.Log("魔力が足りません！");
            return;
        }

        Debug.Log($"使用: {item.itemName}, 消費MP: {item.mpCost}");

        switch (item.itemID)
        {
            case 0: // 弓
                ShootArrow(item);
                break;
            case 1: // 爆弾
                if (item.itemPrefab != null)
                    Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                break;
            default:
                Debug.Log("使用処理未定義のアイテム");
                break;
        }
    }
    void ShootArrow(ItemData bowItem)
    {
        if (bowItem.itemPrefab == null)
        {
            Debug.LogWarning("弓のPrefab（矢）が設定されていません！");
            return;
        }

        // 矢の生成位置（プレイヤーの少し前）
        Vector3 spawnPos = transform.position + (Vector3)lastMoveDir * 0.5f;

        // 向きを計算（プレイヤーが向いている方向）
        float angle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        GameObject arrow = Instantiate(bowItem.itemPrefab, spawnPos, rot);

        // 矢に速度を与える
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = lastMoveDir * 10f; // ← 発射速度（調整可）
        }

        Debug.Log("矢を放った！");
    }
}
