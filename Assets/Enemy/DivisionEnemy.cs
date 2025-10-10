using UnityEngine;
using UnityEngine.UI;

public class DivisionEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float rushDistance = 5f;
    public float waitTime = 1.5f;

    [Header("Prefab Settings")]
    public GameObject itemPrefab;
    public GameObject arrowUIPrefab;
    public GameObject miniEnemyPrefab;

    [Header("Split Settings")]
    public int numberOfSplits = 2;
    public float splitSpreadAngle = 90f;

    [Header("HP Settings")]
    public int maxHP = 5;
    private int currentHP;

    private Transform player;
    private Vector2 moveDirection;
    private Vector2 startPosition;
    private float waitTimer = 0f;

    private enum State { Idle, Rushing }
    private State state = State.Idle;

    private Camera mainCamera;
    private RectTransform arrowInstance;

    [Header("Invincibility Settings")]
    [SerializeField] private float invincibilityDuration = 2f;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    private SpriteRenderer spriteRenderer;
    private Color originColor;

    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;
        mainCamera = Camera.main;

        currentHP = maxHP;
        waitTimer = waitTime;

        // 画面外矢印の生成
        if (arrowUIPrefab != null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                GameObject arrowObj = Instantiate(arrowUIPrefab, canvas.transform);
                arrowInstance = arrowObj.GetComponent<RectTransform>();
            }
        }

        // ミニ敵なら即ラッシュ開始
        if (player != null && state == State.Idle && gameObject.CompareTag("MiniEnemy"))
        {
            moveDirection = (player.position - transform.position).normalized;
            startPosition = transform.position;
            state = State.Rushing;
        }
    }

    void Update()
    {
        if (isDead) return;

        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            return;
        }

        HandleStateMachine();
        HandleArrow();
        HandleInvincibility();
    }

    void HandleStateMachine()
    {
        switch (state)
        {
            case State.Idle:
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    moveDirection = (player.position - transform.position).normalized;
                    startPosition = transform.position;
                    state = State.Rushing;
                }
                break;

            case State.Rushing:
                Vector2 newPos = rb.position + moveDirection * speed * Time.deltaTime;
                rb.MovePosition(newPos);

                float traveled = Vector2.Distance(startPosition, rb.position);
                if (traveled >= rushDistance)
                {
                    state = State.Idle;
                    waitTimer = waitTime;
                }
                break;
        }
    }

    void HandleArrow()
    {
        if (arrowInstance == null || mainCamera == null) return;

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        bool isOffScreen =
            viewportPos.x < 0f || viewportPos.x > 1f ||
            viewportPos.y < 0f || viewportPos.y > 1f ||
            viewportPos.z < 0f;

        arrowInstance.gameObject.SetActive(isOffScreen);

        if (isOffScreen)
        {
            Vector3 screenPos = new Vector3(
                Mathf.Clamp(viewportPos.x, 0.05f, 0.95f) * Screen.width,
                Mathf.Clamp(viewportPos.y, 0.05f, 0.95f) * Screen.height,
                0f
            );
            arrowInstance.position = screenPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            if (!isInvincible)
            {
                TakeDamage(1);
            }
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            StartInvincibility();
        }
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
        float alpha = Mathf.PingPong(Time.time * 10f, 0.5f) + 0.5f;
        spriteRenderer.color = new Color(1f, 0f, 0f, alpha);

        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
            spriteRenderer.color = originColor;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;

        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }

        if (arrowInstance != null)
        {
            Destroy(arrowInstance.gameObject);
        }

        // 通常の敵だけ分裂（ミニ敵は分裂しない）
        if (!CompareTag("MiniEnemy"))
        {
            Split();
        }

        Destroy(gameObject, 0.2f);
    }

    void Split()
    {
        if (miniEnemyPrefab == null || numberOfSplits <= 0) return;

        float angleStep = splitSpreadAngle / (numberOfSplits - 1);
        float startAngle = -splitSpreadAngle / 2;

        for (int i = 0; i < numberOfSplits; i++)
        {
            GameObject mini = Instantiate(miniEnemyPrefab, transform.position, Quaternion.identity);
            mini.tag = "MiniEnemy";

            float angle = startAngle + angleStep * i;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

            Rigidbody2D miniRB = mini.GetComponent<Rigidbody2D>();
            if (miniRB != null)
            {
                miniRB.AddForce(direction * 3f, ForceMode2D.Impulse);
            }
        }
    }
}
