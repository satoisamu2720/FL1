using UnityEngine;
using UnityEngine.UI;

public class Approachingenemy : MonoBehaviour
{
    [Header("行動パラメータ")]
    public float speed = 3f;             // 移動速度
    public float rushDistance = 1.5f;    // 突進距離
    public float waitTime = 1.5f;        // 次の突進までの待ち時間
    public bool startImmediate = false;  // すぐ突進するか

    [Header("敵ステータス")]
    public int maxHP = 2;                // 最大HP
    public GameObject itemPrefab;        // ドロップアイテム
    public GameObject arrowUIPrefab;     // 画面外表示用UI

    [Header("無敵設定")]
    [SerializeField] private float invincibilityDuration = 2f;

    private int currentHP;
    private Transform player;
    private Vector2 moveDirection;
    private Vector2 startPosition;
    private float waitTimer = 0f;

    private enum State { Idle, Rushing }
    private State state = State.Idle;
    private Camera mainCamera;
    private RectTransform arrowInstance;

    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    private bool isDead = false;
    private float rushTimer = 0f;
    private float randomChoice;

    private SpriteRenderer spriteRenderer;
    private Color originColor;
    private Animator animator;

    private enum FacingDirection { Left, Right }
    private FacingDirection lastDirection = FacingDirection.Right;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        currentHP = maxHP;

        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;
        animator = GetComponent<Animator>();

        if (arrowUIPrefab != null && GameObject.Find("Canvas") != null)
        {
            GameObject arrowObj = Instantiate(arrowUIPrefab, GameObject.Find("Canvas").transform);
            arrowInstance = arrowObj.GetComponent<RectTransform>();
        }

        randomChoice = Random.Range(0.1f, 1.0f);

        if (startImmediate)
        {
            moveDirection = (player != null ? (player.position - transform.position).normalized : Vector2.down);
            startPosition = transform.position;
            state = State.Rushing;
            rushTimer = 1f;
        }
        else
        {
            waitTimer = randomChoice;
        }
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (player == null) return;
        }

        HandleStateMachine();
        HandleArrow();
        HandleInvincibility();
        UpdateAnimation();
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
                    rushTimer = 1f;
                }
                break;

            case State.Rushing:
                transform.Translate(moveDirection * speed * Time.deltaTime);
                float traveled = Vector2.Distance(startPosition, transform.position);
                rushTimer -= Time.deltaTime;

                if (traveled >= rushDistance || rushTimer <= 0f)
                {
                    randomChoice = Random.Range(0.1f, 1.0f);
                    waitTimer = randomChoice;
                    state = State.Idle;
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
            viewportPos.y < 0f || viewportPos.y > 1f || viewportPos.z < 0f;

        arrowInstance.gameObject.SetActive(isOffScreen);

        if (isOffScreen)
        {
            Vector3 dir = (transform.position - mainCamera.transform.position).normalized;
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            Vector3 screenDir = new Vector3(dir.x, dir.y, 0).normalized;

            Vector3 screenPos = screenCenter + screenDir * 150f;
            screenPos.x = Mathf.Clamp(screenPos.x, 30f, Screen.width - 30f);
            screenPos.y = Mathf.Clamp(screenPos.y, 30f, Screen.height - 30f);

            arrowInstance.position = screenPos;

            float angle = Mathf.Atan2(screenDir.y, screenDir.x) * Mathf.Rad2Deg;
            arrowInstance.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        if (state == State.Rushing)
        {
            float x = moveDirection.x;
            if (x > 0.01f)
            {
                lastDirection = FacingDirection.Right;
                animator.Play("zombie_Right");
            }
            else if (x < -0.01f)
            {
                lastDirection = FacingDirection.Left;
                animator.Play("zombie_Left");
            }
            else
            {
                animator.Play(lastDirection == FacingDirection.Right ? "zombie_Right" : "zombie_Left");
            }
        }
        else
        {
            animator.Play(lastDirection == FacingDirection.Right ? "zombie_Right" : "zombie_Left");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        currentHP -= damage;
        StartInvincibility();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }

        if (arrowInstance != null)
        {
            Destroy(arrowInstance.gameObject);
        }

        Destroy(gameObject);
    }

    private void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }

    private void HandleInvincibility()
    {
        if (!isInvincible) return;

        invincibilityTimer -= Time.deltaTime;
        float alpha = Mathf.PingPong(Time.time * 10f, 1f);
        spriteRenderer.color = new Color(1f, 0f, 0f, alpha);

        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
            spriteRenderer.color = originColor;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state == State.Rushing && collision.collider.CompareTag("Wall"))
        {
            state = State.Idle;
            waitTimer = waitTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }
}
