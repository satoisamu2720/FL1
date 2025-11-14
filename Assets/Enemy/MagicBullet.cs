using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    public int damage = 1;

    private bool reflectedByPlayer = false;
    private int enemyReflectCount = 0;
    private int maxEnemyReflects = 5;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーに当たったら必ず反射
        if (other.CompareTag("Player") && !reflectedByPlayer)
        {
            reflectedByPlayer = true;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = -rb.linearVelocity; // 反射
            GetComponent<SpriteRenderer>().color = Color.cyan; // 青
            gameObject.tag = "PlayerBullet";
        }

        // 敵に当たった場合
        if (other.GetComponent<ReflectMage>() != null)
        {
            ReflectMage enemy = other.GetComponent<ReflectMage>();

            // ランダムで反射
            if (Random.value > 0.5f && enemyReflectCount < maxEnemyReflects)
            {
                enemyReflectCount++;
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                rb.linearVelocity = -rb.linearVelocity;
                GetComponent<SpriteRenderer>().color = Color.red; // 赤
                gameObject.tag = "MagicBullet";
                return; // 反射しただけでダメージは与えない
            }

            // ダメージ処理
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }

        // 壁に当たったら消える
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
