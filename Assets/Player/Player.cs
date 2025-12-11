using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastMoveDir = Vector2.down;
    private Vector2 attackDir;

    [Header("攻撃パラメータ")]
    public GameObject swordHitbox;
    public float attackDuration = 0.2f;
    public float hitboxDistance = 0.6f;
    public float swingArc = 75f;

    [Header("回転攻撃パラメータ")]
    public float spinDuration = 0.6f;
    public float spinSpeed = 720f;

    [Header("長押し判定")]
    public float holdThreshold = 0.3f;

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
    private float attackHoldTime = 0f;

    // =================================
    //        ★★ HP / ダメージ ★★
    // =================================
    [Header("HP パラメータ")]
    public int maxHP = 5;
    public int currentHP;

    [Header("無敵時間")]
    public float invincibleTime = 0.5f;
    private bool isInvincible = false;
    private float invTimer = 0f;

    private SpriteRenderer sprite;
    private Color originColor;
    private bool isDead = false;
    // =================================


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        originColor = sprite.color;

        currentHP = maxHP;

        sr = GetComponent<SpriteRenderer>();
        if (swordHitbox != null)
        {
            swordHitbox.SetActive(false);
        }
    }

    void Update()
    {
        if (isDead) return;

        HandleHPInvincible();  // ← 無敵時間制御

        if (GameManager.Instance != null && !GameManager.Instance.isPause)
        {
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
        if (isDead) return;

        if (GameManager.Instance != null && !GameManager.Instance.isPause)
        {
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            if (movement != Vector2.zero)
                lastMoveDir = movement;

            // プレイヤー移動
            switch (attackState)
            {
                case AttackState.None:
                    MovePlayer(Status.Instance.PlayerSpeed);
                    PushBlockCheck();  // ★ここ追加
                    break;

                case AttackState.Charge:
                    MovePlayer(Status.Instance.PlayerSpeed * 0.4f);
                    PushBlockCheck();  // ★チャージ中も押せる
                    break;
            }
        }
    }

    private void MovePlayer(float speed)
    {
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }


    // ================================
    //           攻撃処理
    // ================================
    private void HandleAttackInput()
    {
        if (Input.GetButtonDown("Fire1") && attackState == AttackState.None)
        {
            attackHoldTime = 0f;
            attackDir = (movement != Vector2.zero) ? movement : lastMoveDir;

            StartCoroutine(SwingAttack());
        }

        if (Input.GetButton("Fire1"))
        {
            attackHoldTime += Time.deltaTime;
        }

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

        bool horizontal = Mathf.Abs(dir.x) > Mathf.Abs(dir.y);
        bool isMoving = movement != Vector2.zero;

        Sprite s = null;

        if (!isMoving)
        {
            if (horizontal)
                s = (dir.x > 0) ? rightIdle : leftIdle;
            else
                s = (dir.y > 0) ? upIdle : downIdle;

            sr.sprite = s;
            return;
        }

        walkAnimTimer += Time.deltaTime;

        if (horizontal)
        {
            float t = walkAnimTimer % (walkAnimSpeed * 4f);

            if (t < walkAnimSpeed)
            {
               
                s = (dir.x > 0) ? rightWalk1 : leftWalk1;
            }
            else if (t < walkAnimSpeed * 2f)
            {
               
                s = (dir.x > 0) ? rightIdle : leftIdle;
            }
            else if (t < walkAnimSpeed * 3f)
            {
                
                s = (dir.x > 0) ? rightWalk2 : leftWalk2;
            }
            else
            {
                
                s = (dir.x > 0) ? rightIdle : leftIdle;
            }

            sr.sprite = s;
            return;
        }
        bool frame = (walkAnimTimer % (walkAnimSpeed * 2)) < walkAnimSpeed;

        if (dir.y > 0)
            s = frame ? upWalk1 : upWalk2;
        else
            s = frame ? downWalk1 : downWalk2;

        sr.sprite = s;
    }



    void Awake()

    // ================================
    //           ★ HP関連 ★
    // ================================
    private void HandleHPInvincible()
    {
        if (!isInvincible) return;

        invTimer -= Time.deltaTime;

        // 点滅表現
        float a = Mathf.PingPong(Time.time * 12f, 1f);
        sprite.color = new Color(1f, 1f, 1f, a);

        if (invTimer <= 0f)
        {
            isInvincible = false;
            sprite.color = originColor;
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isInvincible || isDead) return;

        currentHP -= dmg;

        if (currentHP <= 0)
        {
            Die();
            return;
        }

        isInvincible = true;
        invTimer = invincibleTime;
    }

    public void RecoverHP(int v)
    {
        currentHP = Mathf.Min(maxHP, currentHP + v);
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;

        Debug.Log("Player Dead");
        // ここでゲームオーバー画面とか
    }


    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void PushBlockCheck()
    {
        if (attackState != AttackState.None && attackState != AttackState.Charge)
            return;

        // movementが0でも、lastMoveDirを使う
        Vector2 dir = movement != Vector2.zero ? movement : lastMoveDir;
        if (dir == Vector2.zero) return;

        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            dir,
            0.6f,
            LayerMask.GetMask("Block")
        );

        if (hit.collider != null)
        {
            PushBlock block = hit.collider.GetComponent<PushBlock>();
            if (block != null)
            {
                block.TryPush(dir);
            }
        }
    }

}
