using UnityEngine;
using UnityEngine.UI;

public class Approachingenemy : MonoBehaviour, ISwordDamageable
{
    [Header("行動パラメータ")]
    public float speed = 3f;
    public float rushDistance = 1.5f;
    public bool startImmediate = false;

    [Header("敵ステータス")]
    public int maxHP = 2;
    public GameObject itemPrefab;
    public GameObject arrowUIPrefab;

    [Header("無敵設定")]
    [SerializeField] private float invincibilityDuration = 0.4f;

    [Header("ノックバック設定")]
    public float knockbackForce = 4f;
    public float knockbackDuration = 0.1f;

    private int currentHP;
    private Transform player;

    private enum State { Idle, Rushing }
    private State state = State.Idle;

    private Vector2 moveDirection;
    private Vector2 startPosition;
    private float waitTimer;
    private float rushTimer;

    private bool isDead;
    private bool isInvincible;
    private float invincibilityTimer;

    private Vector2 knockbackDirection;
    private float knockbackTimer;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originColor;
    private Animator animator;

    private Camera mainCamera;
    private RectTransform arrowInstance;

    // ===============================
    // 初期化
    // ===============================
    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        currentHP = maxHP;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;
        animator = GetComponent<Animator>();

        mainCamera = Camera.main;

        if (arrowUIPrefab != null && GameObject.Find("Canvas") != null)
        {
            arrowInstance = Instantiate(
                arrowUIPrefab,
                GameObject.Find("Canvas").transform
            ).GetComponent<RectTransform>();
        }

        waitTimer = Random.Range(0.2f, 1f);

        if (startImmediate && player != null)
        {
            moveDirection = (player.position - transform.position).normalized;
            startPosition = transform.position;
            state = State.Rushing;
            rushTimer = 1f;
        }
    }

    // ===============================
    // 見た目・UIのみ
    // ===============================
    void Update()
    {
        if (isDead) return;

        HandleArrow();
        HandleInvincibility();
    }

    // ===============================
    // 物理処理専用
    // ===============================
    void FixedUpdate()
    {
        if (isDead) return;

        HandleKnockback();
        HandleStateMachine();
    }

    // ===============================
    // 行動制御（velocityのみ）
    // ===============================
    void HandleStateMachine()
    {
        if (knockbackTimer > 0f)
            return;

        if (state == State.Idle)
        {
            rb.linearVelocity = Vector2.zero;
            waitTimer -= Time.fixedDeltaTime;

            if (waitTimer <= 0f && player != null)
            {
                moveDirection = (player.position - transform.position).normalized;
                startPosition = transform.position;
                rushTimer = 1f;
                state = State.Rushing;
            }
        }
        else if (state == State.Rushing)
        {
            rb.linearVelocity = moveDirection * speed;
            rushTimer -= Time.fixedDeltaTime;

            float traveled = Vector2.Distance(startPosition, rb.position);
            if (traveled >= rushDistance || rushTimer <= 0f)
            {
                waitTimer = Random.Range(0.2f, 1f);
                state = State.Idle;
            }
        }
    }

    // ===============================
    // ノックバック（壁はCollisionで止める）
    // ===============================
    void HandleKnockback()
    {
        if (knockbackTimer > 0f)
        {
            rb.linearVelocity = knockbackDirection * knockbackForce;
            knockbackTimer -= Time.fixedDeltaTime;

            if (knockbackTimer <= 0f)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    // ===============================
    // ダメージ
    // ===============================
    public void TakeDamage(int damage, Vector2 sourcePosition)
    {
        if (isDead || isInvincible) return;

        currentHP -= damage;
        StartInvincibility();

        knockbackDirection =
            ((Vector2)transform.position - sourcePosition).normalized;
        knockbackTimer = knockbackDuration;

        if (currentHP <= 0)
            Die();
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, player != null
            ? (Vector2)player.position
            : (Vector2)transform.position);
    }

    void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }

    void HandleInvincibility()
    {
        if (!isInvincible) return;

        invincibilityTimer -= Time.deltaTime;
        float a = Mathf.PingPong(Time.time * 20f, 1f);
        spriteRenderer.color = new Color(1f, 0f, 0f, a);

        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
            spriteRenderer.color = originColor;
        }
    }

    // ===============================
    // 壁判定（Tagのみ）
    // ===============================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            knockbackTimer = 0f;
            rb.linearVelocity = Vector2.zero;
        }
    }

    // ===============================
    // 剣・プレイヤー（Trigger）
    // ===============================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Sword"))
        {
            TakeDamage(1, other.transform.position);
        }

        if (other.CompareTag("Player"))
        {
            Player p = other.GetComponent<Player>();
            if (p != null)
                p.TakeDamage(1);
        }
    }

    // ===============================
    // UI矢印
    // ===============================
    void HandleArrow()
    {
        if (arrowInstance == null || mainCamera == null) return;

        Vector3 vp = mainCamera.WorldToViewportPoint(transform.position);
        bool off =
            vp.x < 0 || vp.x > 1 ||
            vp.y < 0 || vp.y > 1 || vp.z < 0;

        arrowInstance.gameObject.SetActive(off);
        if (!off || player == null) return;

        Vector3 dir = (transform.position - player.position).normalized;
        Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f);
        Vector3 pos = center + dir * 150f;

        pos.x = Mathf.Clamp(pos.x, 30, Screen.width - 30);
        pos.y = Mathf.Clamp(pos.y, 30, Screen.height - 30);

        arrowInstance.position = pos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrowInstance.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    // ===============================
    // 死亡
    // ===============================
    void Die()
    {
        isDead = true;

        if (itemPrefab != null)
            Instantiate(itemPrefab, transform.position, Quaternion.identity);

        if (arrowInstance != null)
            Destroy(arrowInstance.gameObject);

        MapManager.Instance.EnemyDefeated();
        Destroy(gameObject);
    }
}
