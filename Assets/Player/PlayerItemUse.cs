using UnityEngine;
using UnityEngine.UI;

public class PlayerItemUse : MonoBehaviour
{
    [Header("UI")]
    public Image equippedItemIcon; // EquippedItemIcon ���A�T�C��
    public Sprite defaultSprite;   // ��̂Ƃ��̓����A�C�R���Ȃ�

    private Vector2 lastMoveDir = Vector2.down;
    void Start()
    {
        
        // ���������A�C�e��
        Inventory.Instance.AddItem(0); // �|
        Inventory.Instance.AddItem(1); // ���e
        Inventory.Instance.AddItem(2); // ���i�펞����p�j

        var item = Inventory.Instance.GetSelectedItem();
        Debug.Log($"�I�𒆃A�C�e��: {item.itemName}");
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
            equippedItemIcon.color = new Color(1, 1, 1, 0); // �����ɂ���
        }
    }

    void UseEquippedItem()
    {
        var item = Inventory.Instance.GetEquippedItem();
        if (item == null)
        {
            Debug.Log("�����A�C�e��������܂���");
            return;
        }

        if (!Status.Instance.UseMP(item.mpCost))
        {
            Debug.Log("MP������܂���I");
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
            default: Debug.Log($"{item.itemName} ���g�p�����I");
                break;
        }
    }

    void ShootArrow(ItemData bowItem)
    {
        if (bowItem.itemPrefab == null)
        {
            Debug.LogWarning("�|��Prefab���ݒ肳��Ă��܂���I");
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
