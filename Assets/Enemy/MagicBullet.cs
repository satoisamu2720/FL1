using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    public int damage = 1;
    public float bulletSpeed = 10f;

    private bool reflectedByPlayer = false;
    private int enemyReflectCount = 0;
    private int maxEnemyReflects = 5;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ★ プレイヤーに当たったら反射して、敵へ向かう
        if (other.CompareTag("Player") && !reflectedByPlayer)
        {
            reflectedByPlayer = true;

            // 近い敵の方向へ向ける
            ReflectTowardClosestEnemy();

            GetComponent<SpriteRenderer>().color = Color.cyan;
            gameObject.tag = "PlayerBullet";
            return;
        }

        // ★ 敵に当たった場合
        if (other.GetComponent<ReflectMage>() != null)
        {
            ReflectMage enemy = other.GetComponent<ReflectMage>();

            // ランダムで反射
            if (Random.value > 0.5f && enemyReflectCount < maxEnemyReflects)
            {
                enemyReflectCount++;

                // 敵 → プレイヤー方向へ飛ぶ
                ReflectTowardPlayer();

                GetComponent<SpriteRenderer>().color = Color.red;
                gameObject.tag = "MagicBullet";

                return; // 反射しただけでダメージなし
            }

            // ダメージ
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }

        // 壁
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    // ★ プレイヤーが弾を反射したとき、敵へ向ける
    private void ReflectTowardClosestEnemy()
    {
        ReflectMage[] enemies = FindObjectsOfType<ReflectMage>();
        if (enemies.Length == 0) return;

        // 最も近い敵を探す
        ReflectMage nearest = null;
        float minDist = float.MaxValue;

        foreach (var e in enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = e;
            }
        }

        if (nearest != null)
        {
            Vector2 dir = (nearest.transform.position - transform.position).normalized;
            rb.linearVelocity = dir * bulletSpeed;
        }
    }

    // ★ 敵が弾を反射したとき、プレイヤー方向へ飛ぶ
    private void ReflectTowardPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector2 dir = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = dir * bulletSpeed;
    }
}
