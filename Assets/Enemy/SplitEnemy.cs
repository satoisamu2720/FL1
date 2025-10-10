using System.Collections.Generic;
using UnityEngine;

public class SplitEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float rushDistance = 5f;
    public float waitTime = 1.5f;
    public float rushTimeout = 1f;
    public float separationDistance = 1.5f;

    [Header("HP Settings")]
    public int maxHP = 1;
    public GameObject itemPrefab;

    private Transform player;
    private Vector2 moveDirection;
    private Vector2 startPosition;
    private float waitTimer = 0f;
    private float rushTimer = 0f;
    private int currentHP;

    private enum State { Idle, Rushing }
    private State state = State.Idle;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originColor;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    [SerializeField] private float invincibilityDuration = 0.5f;

    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;

        waitTimer = 0f; // 即行動開始
        currentHP = maxHP;
    }

    void Update()
    {
        // プレイヤーがいなければ何もしない（nullチェック）
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            return;
        }

        HandleSeparation();
        HandleStateMachine();
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
                    rushTimer = rushTimeout;
                    state = State.Rushing;
                }
                break;

            case State.Rushing:
                Vector2 newPos = rb.position + moveDirection * speed * Time.deltaTime;
                rb.MovePosition(newPos);

                float traveled = Vector2.Distance(startPosition, rb.position);
                rushTimer -= Time.deltaTime;

                if (traveled >= rushDistance || rushTimer <= 0f)
                {
                    state = State.Idle;
                    waitTimer = waitTime;
                }
                break;
        }
    }

    void HandleSeparation()
    {
        GameObject[] allSplitEnemies = GameObject.FindGameObjectsWithTag("MiniEnemy");

        foreach (var other in allSplitEnemies)
        {
            if (other == this.gameObject) continue;

            float dist = Vector2.Distance(transform.position, other.transform.position);
            if (dist < separationDistance)
            {
                Vector2 pushDir = (transform.position - other.transform.position).normalized;
                rb.MovePosition(rb.position + pushDir * Time.deltaTime * 1f);
            }
        }
    }

    void HandleInvincibility()
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

    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return;

        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
            return;
        }

        StartInvincibility();
    }

    void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }

    public void Die()
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

        Destroy(gameObject, 0.3f);
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
