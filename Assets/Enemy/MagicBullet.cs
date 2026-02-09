using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    public bool IsReflected { get; set; } // 反射状態
    public ReflectMage shooter;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer sr;

    [Header("反射設定")]
    public int remainingReflections;          // 残り反射回数
    public float speedIncreasePerReflect = 2f; // 反射するたび速度アップ

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // 発射時に呼ぶ
    public void Initialize(Vector2 direction, int reflections, float initialSpeed)
    {
        remainingReflections = reflections;
        rb.linearVelocity = direction.normalized * initialSpeed;
        sr.color = Color.red; // 敵弾は赤固定
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        bool reflectedThisTime = false;

        // プレイヤーで反射
        if (collision.collider.CompareTag("Player") && remainingReflections > 0)
        {
            Vector2 reflectDir = ((Vector2)transform.position - (Vector2)collision.transform.position).normalized;
            rb.linearVelocity = reflectDir * (rb.linearVelocity.magnitude + speedIncreasePerReflect);
            IsReflected = true;
            remainingReflections--;
            reflectedThisTime = true;

            sr.color = Color.cyan; // プレイヤー反射は水色
        }

        // 壁で反射
        if (collision.collider.CompareTag("Wall") && remainingReflections > 0)
        {
            Vector2 normal = collision.contacts[0].normal;
            rb.linearVelocity = Vector2.Reflect(rb.linearVelocity, normal).normalized * (rb.linearVelocity.magnitude + speedIncreasePerReflect);
            IsReflected = true;
            remainingReflections--;
            reflectedThisTime = true;

            // 壁反射は残り回数で色を変える
            UpdateColor();
        }

        if (remainingReflections <= 0)
        {
            Destroy(gameObject, 5f);
        }
    }

    public void Reflect(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * (rb.linearVelocity.magnitude + speedIncreasePerReflect);
        IsReflected = true;
        remainingReflections--;
        sr.color = Color.cyan; // 敵で反射した場合も水色
    }

    void UpdateColor()
    {
        if (remainingReflections == 3) sr.color = Color.red;
        else if (remainingReflections == 2) sr.color = Color.yellow;
        else if (remainingReflections == 1) sr.color = Color.gray;
    }
}
