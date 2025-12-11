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
    public int expAmount = 100;
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
        GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        MagicBullet bullet = bulletObj.GetComponent<MagicBullet>();
        bullet.shooter = this;   // ★ 誰が撃ったかセット

        Vector2 direction = (player.position - transform.position).normalized;
        Rigidbody2D rb = bulletObj.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * bulletSpeed;

        bulletObj.tag = "MagicBullet";
        bulletObj.layer = LayerMask.NameToLayer("EnemyBullet");

        bulletObj.GetComponent<SpriteRenderer>().color = Color.red;
    }


    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        StartCoroutine(HitFlash());

        //経験値を渡す
        PlayerStats.Instance.AddExperience(expAmount);


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
