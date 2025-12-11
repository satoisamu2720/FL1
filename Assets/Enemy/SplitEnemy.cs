using UnityEngine;

public class SplitEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Gather State")]
    private bool isGathering = false;
    private Transform gatherTarget;

    private Rigidbody2D rb;
    private Transform player;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }

        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<CircleCollider2D>();
        }
    }

    void Start()
    {
        // ★ プレイヤー取得 ★
        player = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (isGathering)
        {
            MoveTowardGatherPoint();
        }
        else
        {
            ChasePlayer();
        }
    }

    // -------------------------
    // ★ プレイヤー追従 ★
    // -------------------------
    void ChasePlayer()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    // -------------------------
    // 集合状態
    // -------------------------
    void MoveTowardGatherPoint()
    {
        if (gatherTarget == null) return;

        Vector2 dir = (gatherTarget.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    // -------------------------
    // ボスの集合命令
    // -------------------------
    public void StartGathering(Transform target)
    {
        isGathering = true;
        gatherTarget = target;
    }

    public void StopGathering()
    {
        isGathering = false;
        gatherTarget = null;
        rb.linearVelocity = Vector2.zero;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
