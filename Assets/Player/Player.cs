using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastMoveDir = Vector2.down;
    private Vector2 attackDir;

    [Header("攻撃パラメータ")]
    public GameObject swordHitbox;        // 剣の当たり判定プレハブ
    public float attackDuration = 0.2f;   // 通常攻撃(スウィング)の時間
    public float hitboxDistance = 0.6f;   // プレイヤーからの距離
    public float swingArc = 75f;          // スウィングの回転角度

    [Header("回転攻撃パラメータ")]
    public float spinDuration = 0.6f;     // 回転攻撃の時間
    public float spinSpeed = 720f;        // 回転速度（°/秒）

    [Header("長押し判定")]
    public float holdThreshold = 0.3f;    // 長押しと判定するまでの時間

    [Header("止まっている時の画像")]
    public Sprite upIdle;
    public Sprite downIdle;
    public Sprite leftIdle;
    public Sprite rightIdle;

    [Header("歩いている時の画像")]
    public Sprite upWalk1;
    public Sprite upWalk2;
    public Sprite downWalk1;
    public Sprite downWalk2;
    public Sprite leftWalk1;
    public Sprite leftWalk2;
    public Sprite rightWalk1;
    public Sprite rightWalk2;

    private SpriteRenderer sr;
    private float walkAnimTimer = 0f;
    public float walkAnimSpeed = 0.15f; // 歩きアニメの速度

    private enum AttackState { None, Swing, Charge, Spin }
    private AttackState attackState = AttackState.None;

    private float attackHoldTime = 0f; // ボタン押しっぱなし時間計測

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (swordHitbox != null)
        {
            swordHitbox.SetActive(false);
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isPause)
        {
            var equipped = Inventory.Instance.GetEquippedItem();

            // 剣を装備済みなら常に使える
            if (Inventory.Instance.HasSword())
            {
                HandleAttackInput();
            }
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.B))
        {
            Status.Instance.TakeDamage(1);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Status.Instance.RecoverHP(1);
        }
        UpdateAnimation();
#endif
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
                    // チャージ中も低速で移動
                    MovePlayer(Status.Instance.PlayerSpeed * 0.4f);
                    break;

                default:
                    // Swing, Spin中は動けない
                    break;
            }
        }
    }

    private void MovePlayer(float speed)
    {
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }
   

    private void HandleAttackInput()
    {
        // 押した瞬間：通常攻撃開始
        if (Input.GetButtonDown("Fire1") && attackState == AttackState.None)
        {
            attackHoldTime = 0f;

            // 攻撃開始時の方向を固定
            attackDir = (movement != Vector2.zero) ? movement : lastMoveDir;

            StartCoroutine(SwingAttack());
        }

        // 押している間：ホールド時間をカウント
        if (Input.GetButton("Fire1"))
        {
            attackHoldTime += Time.deltaTime;
        }

        // 離した瞬間：回転斬り発動
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

        // 基本の角度を計算
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

        // スウィング後：まだ押していて、かつ長押し判定を超えていたら突きへ
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
        // 押した瞬間の方向に固定
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

        // 攻撃開始方向を基準に回転
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

    private void UpdateAnimation()
    {
        Vector2 dir = (movement != Vector2.zero) ? movement : lastMoveDir;

        //向き判定
        bool horizontal = Mathf.Abs(dir.x) > Mathf.Abs(dir.y);

        string state = movement == Vector2.zero ? "idle" : "walk";

        Sprite s = null;

        // ---- Idle ----
        if (state == "idle")
        {
            if (horizontal)
            {
                s = (dir.x > 0) ? rightIdle : leftIdle;
            }
            else
            {
                s = (dir.y > 0) ? upIdle : downIdle;
            }
            sr.sprite = s;
            return;
        }

        // ---- Walk Animation ----
        walkAnimTimer += Time.deltaTime;
        bool frame = (walkAnimTimer % (walkAnimSpeed * 2)) < walkAnimSpeed;

        if (horizontal)
        {
            if (dir.x > 0)
                s = frame ? rightWalk1 : rightWalk2;
            else
                s = frame ? leftWalk1 : leftWalk2;
        }
        else
        {
            if (dir.y > 0)
                s = frame ? upWalk1 : upWalk2;
            else
                s = frame ? downWalk1 : downWalk2;
        }

        sr.sprite = s;
    }


    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
