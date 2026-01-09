using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifeTime = 3f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ▼ 魔法弾に当たったら「矢の方向にそのまま反射」
        MagicBullet bullet = other.GetComponent<MagicBullet>();
        if (bullet != null)
        {
            // ★ プレイヤーが撃った方向そのもの
            Vector2 reflectDir = rb.linearVelocity.normalized;

            bullet.Reflect(reflectDir);

            Destroy(gameObject);
            return;
        }

        // ▼ 通常の衝突
        if (other.CompareTag("Enemy") || other.CompareTag("Switch") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
