using UnityEngine;

public class BulletEnemy : MonoBehaviour, ISwordDamageable
{
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
    public float invincibilityDuration = 0.5f;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Color originColor;

    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        currentHP = maxHP;
        shootTimer = shootInterval;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;
    }

    void Update()
    {
        if (isDead) return;

        if (player == null) return;

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootInterval;
        }

        MoveAwayFromPlayer();
        HandleInvincibility();
    }

    void Shoot()
    {
        if (bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Vector2 direction = (player.position - transform.position).normalized;

        bullet.GetComponent<Bullets>().SetDirection(direction);
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

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        currentHP -= damage;
        StartInvincibility();

        if (currentHP <= 0) Die();
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
        float alpha = Mathf.PingPong(Time.time * 25f, 1f);
        spriteRenderer.color = new Color(1f, 0f, 0f, alpha);

        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
            spriteRenderer.color = originColor;
        }
    }

    void Die()
    {
        isDead = true;

        if (itemPrefab != null)
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        // ▼ 剣に当たったらダメージ
        if (other.CompareTag("Sword"))
        {
            TakeDamage(1);
        }


        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(1);
            
            return;
        }
    }
}
