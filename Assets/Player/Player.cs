using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastMoveDir = Vector2.down;
    private Vector2 attackDir;

    [Header("�U���p�����[�^")]
    public GameObject swordHitbox;        // ���̓����蔻��v���n�u
    public float attackDuration = 0.2f;   // �ʏ�U��(�X�E�B���O)�̎���
    public float hitboxDistance = 0.6f;   // �v���C���[����̋���
    public float swingArc = 75f;          // �X�E�B���O�̉�]�p�x

    [Header("��]�U���p�����[�^")]
    public float spinDuration = 0.6f;     // ��]�U���̎���
    public float spinSpeed = 720f;        // ��]���x�i��/�b�j

    [Header("����������")]
    public float holdThreshold = 0.3f;    // �������Ɣ��肷��܂ł̎���

    private enum AttackState { None, Swing, Charge, Spin }
    private AttackState attackState = AttackState.None;

    private float attackHoldTime = 0f; // �{�^���������ςȂ����Ԍv��

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (swordHitbox != null) swordHitbox.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isPause)
        {
            var equipped = Inventory.Instance.GetEquippedItem();

            // ���𑕔��ς݂Ȃ��Ɏg����
            if (Inventory.Instance.HasSword())
            {
                HandleAttackInput();
            }
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isPause )
        {
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            if (movement != Vector2.zero)
            {
                lastMoveDir = movement;
            }

            switch (attackState)
            {
                case AttackState.None:
                    MovePlayer(Status.Instance.PlayerSpeed);
                    break;

                case AttackState.Charge:
                    // �`���[�W�����ᑬ�ňړ�
                    MovePlayer(Status.Instance.PlayerSpeed * 0.4f);
                    break;

                default:
                    // Swing, Spin���͓����Ȃ�
                    break;
            }
        }
    }

    private void MovePlayer(float speed)
    {
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }
    public void Recover(float amount)
    {
        Status.Instance.PlayerHP = Mathf.Min(Status.Instance.PlayerHP + amount, Status.Instance.MaxHP);
        Debug.Log($"�񕜁I ����HP: {Status.Instance.PlayerHP}/{Status.Instance.MaxHP}");
    }


    private void HandleAttackInput()
    {
        // �������u�ԁF�ʏ�U���J�n
        if (Input.GetButtonDown("Fire1") && attackState == AttackState.None)
        {
            attackHoldTime = 0f;

            // �U���J�n���̕������Œ�
            attackDir = (movement != Vector2.zero) ? movement : lastMoveDir;

            StartCoroutine(SwingAttack());
        }

        // �����Ă���ԁF�z�[���h���Ԃ��J�E���g
        if (Input.GetButton("Fire1"))
        {
            attackHoldTime += Time.deltaTime;
        }

        // �������u�ԁF��]�a�蔭��
        if (Input.GetButtonUp("Fire1"))
        {
            if (attackState == AttackState.Charge)
            {
                StartCoroutine(SpinAttack());
            }
        }

    }


    private System.Collections.IEnumerator SwingAttack()
    {
        attackState = AttackState.Swing;
        swordHitbox.SetActive(true);

        // ��{�̊p�x���v�Z
        float baseAngle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - swingArc / 2f;
        float endAngle = baseAngle + swingArc / 2f;

        float elapsed = 0f;

        while (elapsed < attackDuration)
        {
            float t = elapsed / attackDuration;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);

            Vector3 offset = new Vector3(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad),
                0
            ) * hitboxDistance;

            swordHitbox.transform.localPosition = offset;
            swordHitbox.transform.localRotation = Quaternion.Euler(0, 0, currentAngle);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // �X�E�B���O��F�܂������Ă��āA������������𒴂��Ă�����˂���
        if (Input.GetButton("Fire1") && attackHoldTime >= holdThreshold)
        {
            attackState = AttackState.Charge;
            HoldThrust();
        }
        else
        {
            swordHitbox.SetActive(false);
            attackState = AttackState.None;
        }
    }

    private void HoldThrust()
    {
        // �������u�Ԃ̕����ɌŒ�
        float baseAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(baseAngle / 45f) * 45f;

        Vector3 offset = new Vector3(
            Mathf.Cos(snappedAngle * Mathf.Deg2Rad),
            Mathf.Sin(snappedAngle * Mathf.Deg2Rad),
            0
        ) * hitboxDistance;

        swordHitbox.transform.localPosition = offset;
        swordHitbox.transform.localRotation = Quaternion.Euler(0, 0, snappedAngle);
    }

    private System.Collections.IEnumerator SpinAttack()
    {
        attackState = AttackState.Spin;
        float elapsed = 0f;
        swordHitbox.SetActive(true);

        // �U���J�n��������ɉ�]
        float baseAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

        while (elapsed < spinDuration)
        {
            float currentAngle = baseAngle + elapsed * spinSpeed;

            Vector3 offset = new Vector3(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad),
                0
            ) * hitboxDistance;

            swordHitbox.transform.localPosition = offset;
            swordHitbox.transform.localRotation = Quaternion.Euler(0, 0, currentAngle);

            elapsed += Time.deltaTime;
            yield return null;
        }

        swordHitbox.SetActive(false);
        attackState = AttackState.None;
    }

    public void TakeDamage(float amount)
    {
        Status.Instance.PlayerHP -= amount;
        if (Status.Instance.PlayerHP <= 0)
        {
            Status.Instance.PlayerHP = 0;
            Debug.Log("�v���C���[�͂��ꂽ�I");
        }
    }
    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
