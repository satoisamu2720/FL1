using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    [Header("Damage / Speed")]
    public int damage = 1;
    public float bulletSpeed = 10f;

    [Header("Reflection Settings")]
    public int maxEnemyReflects = 5;

    [HideInInspector] public ReflectMage shooter;     // ★ この弾を撃った敵
    private int enemyReflectCount = 0;
    private bool reflectedByPlayer = false;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ★ 自分を撃った敵には当たらない
        var enemyHit = other.GetComponent<ReflectMage>();
        if (enemyHit != null && enemyHit == shooter)
            return;

        // ★ プレイヤー反射（最初の一回だけ）
        if (other.CompareTag("Player") && !reflectedByPlayer)
        {
            reflectedByPlayer = true;
            ReflectTowardClosestEnemy();

            gameObject.tag = "PlayerBullet";
            gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
            GetComponent<SpriteRenderer>().color = Color.cyan;

            return;
        }

        // ★ 敵に当たった場合
        if (enemyHit != null)
        {
            // --- 敵がランダムで反射する ---
            if (Random.value > 0.5f && enemyReflectCount < maxEnemyReflects)
            {
                enemyReflectCount++;
                ReflectTowardPlayer();

                gameObject.tag = "MagicBullet";
                gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
                GetComponent<SpriteRenderer>().color = Color.red;

                return;  // 反射しただけでダメージなし
            }

            // --- 反射しない場合 → ダメージ与える ---
            enemyHit.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // ★ 壁に当たったら破壊
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    // ================================================================
    // 最も近い敵へ飛ばす（プレイヤー反射時）
    // ================================================================
    private void ReflectTowardClosestEnemy()
    {
        ReflectMage[] enemies = FindObjectsOfType<ReflectMage>();
        if (enemies.Length == 0) return;

        ReflectMage nearest = null;
        float minDist = float.MaxValue;

        foreach (var e in enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (e != shooter && dist < minDist)
            {
                minDist = dist;
                nearest = e;
            }
        }

        if (nearest == null) return;

        Vector2 dir = (nearest.transform.position - transform.position).normalized;
        rb.linearVelocity = dir * bulletSpeed;
    }

    // ================================================================
    // 敵が反射 → プレイヤーへ飛ばす
    // ================================================================
    private void ReflectTowardPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector2 dir = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = dir * bulletSpeed;
    }
}
