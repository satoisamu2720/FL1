using UnityEngine;

public class PlayerItemUse : MonoBehaviour
{
    private Vector2 lastMoveDir = Vector2.down; // ��������
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
            // �v���C���[�̈ړ��������擾�i�����̍X�V�j
            Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (inputDir.sqrMagnitude > 0.01f)
                lastMoveDir = inputDir.normalized;

            // �| or ���e�g�p
            if (Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.JoystickButton2))
            {
                UseSelectedItem();
            }

            // �A�C�e���I��ؑ�
            if (Input.GetKeyDown(KeyCode.E) || (Input.GetKeyDown(KeyCode.JoystickButton5)))
            {
                Inventory.Instance.NextItem();
            }

            // �A�C�e���I��ؑ�
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
            Debug.Log("�A�C�e�����I������Ă��܂���B");
            return;
        }

        // ���̓`�F�b�N
        if (!Status.Instance.UseMP(item.mpCost))
        {
            Debug.Log("���͂�����܂���I");
            return;
        }

        Debug.Log($"�g�p: {item.itemName}, ����MP: {item.mpCost}");

        switch (item.itemID)
        {
            case 0: // �|
                ShootArrow(item);
                break;
            case 1: // ���e
                if (item.itemPrefab != null)
                    Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                break;
            default:
                Debug.Log("�g�p��������`�̃A�C�e��");
                break;
        }
    }
    void ShootArrow(ItemData bowItem)
    {
        if (bowItem.itemPrefab == null)
        {
            Debug.LogWarning("�|��Prefab�i��j���ݒ肳��Ă��܂���I");
            return;
        }

        // ��̐����ʒu�i�v���C���[�̏����O�j
        Vector3 spawnPos = transform.position + (Vector3)lastMoveDir * 0.5f;

        // �������v�Z�i�v���C���[�������Ă�������j
        float angle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        GameObject arrow = Instantiate(bowItem.itemPrefab, spawnPos, rot);

        // ��ɑ��x��^����
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = lastMoveDir * 10f; // �� ���ˑ��x�i�����j
        }

        Debug.Log("���������I");
    }
}
