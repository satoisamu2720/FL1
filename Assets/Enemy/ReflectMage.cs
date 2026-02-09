using UnityEngine;

public class ReflectMage : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 3;
    int currentHP;

    [Header("Attack Settings")]
    public GameObject bulletPrefab;
    public float shootInterval = 3f;
    public float bulletSpeed = 10f;
    float shootTimer;

    Transform player;
    SpriteRenderer spriteRenderer;
    Color normalColor;

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

        // 発射
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootInterval;
        }
    }

    void Shoot()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        MagicBullet bullet = bulletObj.GetComponent<MagicBullet>();
        bullet.shooter = this;

        Vector2 direction = (player.position - transform.position).normalized;

        // 反射回数ランダム 1～3
        int reflections = Random.Range(1, 4);

        Rigidbody2D rb = bulletObj.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * bulletSpeed;

        bullet.Initialize(direction, reflections, bulletSpeed);

        bulletObj.tag = "MagicBullet";
        bulletObj.layer = LayerMask.NameToLayer("EnemyBullet");

        // 敵弾は赤固定
        bullet.sr.color = Color.red;
    }


    // 弾を反射する＆反射された弾だけダメージを受ける
    private void OnTriggerEnter2D(Collider2D other)
    {
        MagicBullet bullet = other.GetComponent<MagicBullet>();
        if (bullet == null) return;

        // 弾がまだ反射されていなくて残り反射回数がある場合 → 跳ね返す
        if (!bullet.IsReflected && bullet.remainingReflections > 0)
        {
            Vector2 reflectDir = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
            bullet.Reflect(reflectDir);
        }
        // すでに反射された弾 → ダメージ
        else if (bullet.IsReflected)
        {
            TakeDamage(1);
            Destroy(bullet.gameObject);
        }
    }

    void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        StartCoroutine(HitFlash());

        if (currentHP <= 0)
        {
            OnDeath();
        }
    }

    void OnDeath()
    {
        MapManager.Instance.EnemyDefeated();
        Destroy(gameObject);
    }

    System.Collections.IEnumerator HitFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = normalColor;
    }
}
