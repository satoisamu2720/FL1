using UnityEngine;

public class ShootEnemy : MonoBehaviour
{
    public static ShootEnemy Instance { get; private set; }

    [Header("Movement Settings")]
    public float speed = 2f;
    public float minDistanceFromPlayer = 3f;

    [Header("HP Settings")]
    public int maxHP = 3;
    private int currentHP;

    [Header("Attack Settings")]
    public GameObject bulletPrefab;
    public float shootInterval = 2f;
    private float shootTimer;

    [Header("Drop Item")]
    public GameObject itemPrefab;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 2f;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Color originColor;
    private Animator animator;

    private bool isDead = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        currentHP = maxHP;
        shootTimer = shootInterval;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (player == null) return;
        }

        // �ˌ��^�C�}�[
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootInterval;
        }

        MoveAwayFromPlayer();
        UpdateAnimation();
        HandleInvincibility();
    }

    void Shoot()
    {
        if (bulletPrefab != null && player != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Vector2 direction = (player.position - transform.position).normalized;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * 12f;
            }
            Destroy(bullet, 5f);
        }
    }

    void MoveAwayFromPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < minDistanceFromPlayer)
        {
            Vector2 direction = (transform.position - player.position).normalized;
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        float x = player.position.x - transform.position.x;
        if (x > 0.1f)
        {
            animator.Play("zombie_Right");
        }
        else if (x < -0.1f)
        {
            animator.Play("zombie_Left");
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

    void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }

    void HandleInvincibility()
    {
        if (!isInvincible) return;

        invincibilityTimer -= Time.deltaTime;
        float alpha = Mathf.PingPong(Time.time * 10f, 1f);
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1f, 0f, 0f, alpha);

        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
            if (spriteRenderer != null)
                spriteRenderer.color = originColor;
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (itemPrefab != null)
            Instantiate(itemPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
