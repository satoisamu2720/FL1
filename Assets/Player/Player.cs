using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastMoveDir = Vector2.down; // 初期は下向き

    [Header("攻撃パラメータ")]
    public GameObject swordHitbox;        // 攻撃判定オブジェクト
    public float attackDuration = 0.5f;  // 攻撃全体の時間
    public float swingAngle = 90f;        // スイングの角度幅
    public float hitboxDistance = 1.0f;   // プレイヤーからの距離

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
                lastMoveDir = movement; // 最後に動いた方向を記憶
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

        // 基本方向を算出（8方向）
        float baseAngle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(baseAngle / 45f) * 45f;

        // スイングの開始・終了角度
        float startAngle = snappedAngle - swingAngle / 2f;
        float endAngle = snappedAngle + swingAngle / 2f;

        float elapsed = 0f;

        while (elapsed < attackDuration)
        {
            float t = elapsed / attackDuration;
            // 開始から終了に向けて角度補間
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);

            // offset計算
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
