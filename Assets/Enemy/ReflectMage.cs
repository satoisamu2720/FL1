using UnityEngine;

public class ReflectMage : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 3;
    private int currentHP;

    [Header("Attack Settings")]
    public GameObject bulletPrefab;
    public float shootInterval = 3f;
    public float bulletSpeed = 10f;
    private float shootTimer;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Color normalColor;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalColor = spriteRenderer.color;
        currentHP = maxHP;
        shootTimer = shootInterval;
    }

    void Update()
    {
        if (player == null) return;

        // 向き変更
        Vector3 dir = player.position - transform.position;
        transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);

        // 発射タイマー
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootInterval;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || player == null) return;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * bulletSpeed;
        bullet.tag = "MagicBullet"; // 敵の弾として生成
        bullet.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        StartCoroutine(HitFlash());

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    System.Collections.IEnumerator HitFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = normalColor;
    }
}
