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
    private bool hitWall = false;
    private Vector2 slideDirection;
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
        if (distance >= minDistanceFromPlayer) return;

        Vector2 moveDir;

        if (hitWall)
        {
            // 壁に当たってる間は横移動
            moveDir = slideDirection;
        }
        else
        {
            // 通常はプレイヤーから逃げる
            moveDir = (transform.position - player.position).normalized;
        }

        transform.position += (Vector3)moveDir * speed * Time.deltaTime;
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
        MapManager.Instance.OpenDoorNum++;
        Destroy(gameObject);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        // ▼ 剣に当たったらダメージ
        if (other.CompareTag("Sword"))
        {
            TakeDamage(1);
        }

        if(other.CompareTag("Arrow"))
        {
            TakeDamage(1);
        }

        //Player p = other.GetComponent<Player>();
        //if (player != null)
        //{
        //    p.TakeDamage(1);
        //    if (p != null)
        //        p.TakeDamage(1);
        //}

        if (other.CompareTag("Wall"))
        {
            hitWall = true;

            //// プレイヤーから逃げる方向
            //Vector2 awayDir = (transform.position - player.position).normalized;

            //// 左右どちらかに90度回転
            //if (Random.value < 0.5f)
            //    slideDirection = new Vector2(-awayDir.y, awayDir.x); // 左
            //else
            //    slideDirection = new Vector2(awayDir.y, -awayDir.x); // 右
        }


    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            hitWall = false;
        }
    }
}
