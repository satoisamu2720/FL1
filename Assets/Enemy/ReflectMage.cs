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

    [Header("SE")]
    public AudioClip damageSE;
    public AudioClip deathSE;
    public AudioClip shootSE;
    AudioSource audioSource;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalColor = spriteRenderer.color;

        currentHP = maxHP;
        shootTimer = shootInterval;

        audioSource = GetComponent<AudioSource>();


    }

    void Update()
    {
        if (player == null) return;

        // 向き変更
        Vector3 dir = player.position - transform.position;
        //transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);

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

        if (shootSE != null)
        {
            audioSource.PlayOneShot(shootSE);
        }


        GameObject bulletObj =
            Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        MagicBullet bullet = bulletObj.GetComponent<MagicBullet>();
        bullet.shooter = this;

        Vector2 direction =
            (player.position - transform.position).normalized;

        Rigidbody2D rb = bulletObj.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * bulletSpeed;

        bulletObj.tag = "MagicBullet";
        bulletObj.layer = LayerMask.NameToLayer("EnemyBullet");

        bulletObj.GetComponent<SpriteRenderer>().color = Color.red;
    }

    // ▼ 反射された弾のみダメージ
    void OnTriggerEnter2D(Collider2D other)
    {
        MagicBullet bullet = other.GetComponent<MagicBullet>();
        if (bullet == null) return;

        // 未反射弾は無効
        if (!bullet.IsReflected) return;

        // ▼ ダメージを受ける
        TakeDamage(1);

        // ▼ 弾は消す
        Destroy(bullet.gameObject);
    }



    void TakeDamage(int dmg)
    {

        if (damageSE != null)
            audioSource.PlayOneShot(damageSE);


        currentHP -= dmg;
        StartCoroutine(HitFlash());

        if (currentHP <= 0)
        {
            OnDeath();
        }
    }
    void OnDeath()
    {

        if (deathSE != null)
            AudioSource.PlayClipAtPoint(deathSE, transform.position);


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