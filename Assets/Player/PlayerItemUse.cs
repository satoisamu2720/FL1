using UnityEngine;
using UnityEngine.UI;

public class PlayerItemUse : MonoBehaviour
{
    [Header("UI")]
    public Image equippedItemIcon;
    public Sprite defaultSprite;

    [Header("MP�񕜐ݒ�")]
    public float mpRecoveryInterval = 1f; // 1�񕜂��Ƃ̊Ԋu�i�b�j
    public float mpRecoveryDelay = 1.5f;  // �����ǂꂭ�炢�ŉ񕜂��ĊJ���邩

    private float recoveryTimer = 0f;     // �����̃f�B���C�v���p
    private float intervalTimer = 0f;     // �񕜊Ԋu�p�^�C�}�[
    private Vector2 lastMoveDir = Vector2.down;

    void Start()
    {
        Inventory.Instance.AddItem(0); // �|
        Inventory.Instance.AddItem(1); // ���e
        Inventory.Instance.AddItem(2); // ��

        var item = Inventory.Instance.GetSelectedItem();
        Debug.Log($"�I�𒆃A�C�e��: {item.itemName}");
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isPause)
        {
            // �����X�V
            Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (inputDir.sqrMagnitude > 0.01f)
                lastMoveDir = inputDir.normalized;

            // �A�C�e���g�p
            if (Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.JoystickButton2))
            {
                if (UseEquippedItem())
                {
                    // �g�p������f�B���C���Z�b�g
                    recoveryTimer = 0f;
                    intervalTimer = 0f;
                }
            }

            // MP������
            AutoRecoverMP();
        }

        UpdateEquippedItemUI();
    }

    void AutoRecoverMP()
    {
        if (Status.Instance.PlayerMP >= Status.Instance.MaxMP) return;

        recoveryTimer += Time.deltaTime;

        // ��莞�Ԍo�ߌ�ɉ񕜂��J�n
        if (recoveryTimer >= mpRecoveryDelay)
        {
            intervalTimer += Time.deltaTime;

            // ���Ԋu���Ƃ�1��
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
            Debug.Log("�����A�C�e��������܂���");
            return false;
        }

        if (!Status.Instance.UseMP(item.mpCost))
        {
            Debug.Log("MP������܂���I");
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
                Debug.Log($"{item.itemName} ���g�p�����I");
                break;
        }
        return true;
    }

    void ShootArrow(ItemData bowItem)
    {
        if (bowItem.itemPrefab == null)
        {
            Debug.LogWarning("�|��Prefab�i��j���ݒ肳��Ă��܂���I");
            return;
        }

        Vector3 spawnPos = transform.position + (Vector3)lastMoveDir * 0.5f;
        float angle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        GameObject arrow = Instantiate(bowItem.itemPrefab, spawnPos, rot);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = lastMoveDir * 10f;

        Debug.Log("���������I");
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
