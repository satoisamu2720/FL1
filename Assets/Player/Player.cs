using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastMoveDir = Vector2.down; // �����͉�����

    [Header("�U���p�����[�^")]
    public GameObject swordHitbox;        // �U������I�u�W�F�N�g
    public float attackDuration = 0.5f;  // �U���S�̂̎���
    public float swingAngle = 90f;        // �X�C���O�̊p�x��
    public float hitboxDistance = 1.0f;   // �v���C���[����̋���

    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (swordHitbox != null) swordHitbox.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isPause)
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isPause && !isAttacking)
        {
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            if (movement != Vector2.zero)
            {
                lastMoveDir = movement; // �Ō�ɓ������������L��
            }

            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        rb.MovePosition(rb.position + movement * Status.Instance.PlayerSpeed * Time.deltaTime);
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(SwingAttack());
        }
    }

    private System.Collections.IEnumerator SwingAttack()
    {
        isAttacking = true;
        swordHitbox.SetActive(true);

        // ��{�������Z�o�i8�����j
        float baseAngle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(baseAngle / 45f) * 45f;

        // �X�C���O�̊J�n�E�I���p�x
        float startAngle = snappedAngle - swingAngle / 2f;
        float endAngle = snappedAngle + swingAngle / 2f;

        float elapsed = 0f;

        while (elapsed < attackDuration)
        {
            float t = elapsed / attackDuration;
            // �J�n����I���Ɍ����Ċp�x���
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);

            // offset�v�Z
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
        isAttacking = false;
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
