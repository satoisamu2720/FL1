using UnityEngine;
using UnityEngine.UI;

public class Approachingenemy : MonoBehaviour, ISwordDamageable
{
    [Header("行動パラメータ")]
    public float speed = 3f;
    public float rushDistance = 1.5f;
    public float waitTime = 1.5f;
    public bool startImmediate = false;

    [Header("敵ステータス")]
    public int maxHP = 2;
    public GameObject itemPrefab;
    public GameObject arrowUIPrefab;

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
        if (isDead) return;

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
        if (state == State.Idle)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                moveDirection = (player.position - transform.position).normalized;
                startPosition = transform.position;
                state = State.Rushing;
                rushTimer = 1f;
            }
        }
        else if (state == State.Rushing)
        {
            transform.Translate(moveDirection * speed * Time.deltaTime);
            float traveled = Vector2.Distance(startPosition, transform.position);
            rushTimer -= Time.deltaTime;

            if (traveled >= rushDistance || rushTimer <= 0f)
            {
                randomChoice = Random.Range(0.1f, 1.0f);
                waitTimer = randomChoice;
                state = State.Idle;
            }
        }
    }

    void HandleArrow()
    {
        if (arrowInstance == null || mainCamera == null || isDead) return;

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        bool isOffScreen =
            viewportPos.x < 0f || viewportPos.x > 1f ||
            viewportPos.y < 0f || viewportPos.y > 1f || viewportPos.z < 0f;

        arrowInstance.gameObject.SetActive(isOffScreen);

        if (isOffScreen)
        {
            Vector3 dir = (transform.position - player.position).normalized;
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

            Vector3 screenPos = screenCenter + new Vector3(dir.x, dir.y, 0) * 150f;
            screenPos.x = Mathf.Clamp(screenPos.x, 30f, Screen.width - 30f);
            screenPos.y = Mathf.Clamp(screenPos.y, 30f, Screen.height - 30f);

            arrowInstance.position = screenPos;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrowInstance.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    void UpdateAnimation()
    {
        if (animator == null || isDead) return;

        if (state == State.Rushing)
        {
            if (moveDirection.x > 0) animator.Play("zombie_Right");
            else animator.Play("zombie_Left");
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

        if (currentHP <= 0) Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (itemPrefab != null)
            Instantiate(itemPrefab, transform.position, Quaternion.identity);

        if (arrowInstance != null)
            Destroy(arrowInstance.gameObject);

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
}
