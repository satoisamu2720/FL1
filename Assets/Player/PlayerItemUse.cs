using UnityEngine;
using UnityEngine.UI;

public class PlayerItemUse : MonoBehaviour
{
    [Header("UI")]
    public Image equippedItemIcon;
    public Sprite defaultSprite;

    [Header("MP回復設定")]
    public float mpRecoveryInterval = 1f; // 1回復ごとの間隔（秒）
    public float mpRecoveryDelay = 1.5f;  // 消費後どれくらいで回復を再開するか

    private float recoveryTimer = 0f;     // 消費後のディレイ計測用
    private float intervalTimer = 0f;     // 回復間隔用タイマー
    private Vector2 lastMoveDir = Vector2.down;

    void Start()
    {
        Inventory.Instance.AddItem(0); // 弓
        Inventory.Instance.AddItem(1); // 爆弾
        Inventory.Instance.AddItem(2); // 剣

        var item = Inventory.Instance.GetSelectedItem();
        Debug.Log($"選択中アイテム: {item.itemName}");
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isPause)
        {
            // 向き更新
            Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (inputDir.sqrMagnitude > 0.01f)
                lastMoveDir = inputDir.normalized;

            // アイテム使用
            if (Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.JoystickButton2))
            {
                if (UseEquippedItem())
                {
                    // 使用したらディレイリセット
                    recoveryTimer = 0f;
                    intervalTimer = 0f;
                }
            }

            // MP自動回復
            AutoRecoverMP();
        }

        UpdateEquippedItemUI();
    }

    void AutoRecoverMP()
    {
        if (Status.Instance.PlayerMP >= Status.Instance.MaxMP) return;

        recoveryTimer += Time.deltaTime;

        // 一定時間経過後に回復を開始
        if (recoveryTimer >= mpRecoveryDelay)
        {
            intervalTimer += Time.deltaTime;

            // 一定間隔ごとに1回復
            if (intervalTimer >= mpRecoveryInterval)
            {
                intervalTimer = 0f;
                Status.Instance.RecoverMP(1);
            }
        }
    }

    bool UseEquippedItem()
    {
        var item = Inventory.Instance.GetEquippedItem();
        if (item == null)
        {
            Debug.Log("装備アイテムがありません");
            return false;
        }

        if (!Status.Instance.UseMP(item.mpCost))
        {
            Debug.Log("MPが足りません！");
            return false;
        }

        switch (item.itemID)
        {
            case 0:
                ShootArrow(item);
                break;
            case 1:
                if (item.itemPrefab != null)
                    Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                break;
            default:
                Debug.Log($"{item.itemName} を使用した！");
                break;
        }
        return true;
    }

    void ShootArrow(ItemData bowItem)
    {
        if (bowItem.itemPrefab == null)
        {
            Debug.LogWarning("弓のPrefab（矢）が設定されていません！");
            return;
        }

        Vector3 spawnPos = transform.position + (Vector3)lastMoveDir * 0.5f;
        float angle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        GameObject arrow = Instantiate(bowItem.itemPrefab, spawnPos, rot);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = lastMoveDir * 10f;

        Debug.Log("矢を放った！");
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
            equippedItemIcon.color = new Color(1, 1, 1, 0);
        }
    }
}
