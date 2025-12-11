using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitBossEnemy : MonoBehaviour
{
   
    [System.Serializable]
    public class MiniUnit
    {
        public Transform transform;
        public float moveSpeed = 4f;
        public bool gathering;
        public Transform gatherTarget;

        public MiniUnit(Transform tf)
        {
            transform = tf;
        }

        // 集合中の移動
        public void Update()
        {
            if (gathering && gatherTarget != null)
            {
                Vector2 dir = ((Vector2)gatherTarget.position - (Vector2)transform.position).normalized;
                transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
            }
        }

        public void StartGathering(Transform target)
        {
            gathering = true;
            gatherTarget = target;
        }

        public void StopGathering()
        {
            gathering = false;
            gatherTarget = null;
        }
    }


    // =============================
    // ▼ ボス設定
    // =============================
    [Header("Boss Settings")]
    public int maxHP = 20;
    private int currentHP;

    // =============================
    // ▼ ミニ敵設定
    // =============================
    [Header("Mini Enemy Settings")]
    public GameObject miniEnemyPrefab;
    public int initialMiniCount = 6;
    public float gatherDuration = 2.5f;
    public float attackInterval = 10f;

    private List<MiniUnit> minis = new List<MiniUnit>();

    private float attackTimer = 0f;
    private bool isDead = false;

    private Transform player;

    // =============================
    // ▼ 全体攻撃
    // =============================
    [Header("Attack Settings")]
    public float waveRange = 7f;
    public int waveDamage = 2;


    // =============================
    // ▼ Start
    // =============================
    void Start()
    {
        currentHP = maxHP;

        player = GameObject.FindWithTag("Player")?.transform;

        // ミニ敵生成
        for (int i = 0; i < initialMiniCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 2f;
            GameObject obj = Instantiate(miniEnemyPrefab, (Vector2)transform.position + offset, Quaternion.identity);

            // SplitEnemy を使わず、自作の MiniUnit を登録
            minis.Add(new MiniUnit(obj.transform));
        }
    }


    // =============================
    // ▼ Update
    // =============================
    void Update()
    {
        if (isDead) return;

        // ミニ敵更新
        foreach (var mini in minis)
            mini.Update();

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            StartCoroutine(GatherAndAttack());
            attackTimer = 0f;
        }
    }


    // =============================
    // ▼ ダメージ
    // =============================
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;

        if (currentHP <= 0)
            Die();
    }


    void Die()
    {
        isDead = true;

        // ミニ敵削除
        foreach (var mini in minis)
        {
            if (mini.transform != null)
                Destroy(mini.transform.gameObject);
        }

        Destroy(gameObject);
    }


    // =============================
    // ▼ 集合 → 全体攻撃 → 散開
    // =============================
    IEnumerator GatherAndAttack()
    {
        // 1) 集合
        foreach (var mini in minis)
            mini.StartGathering(transform);

        yield return new WaitForSeconds(gatherDuration);

        // 2) 全体攻撃
        PerformWaveAttack();

        // 3) 散開
        foreach (var mini in minis)
            mini.StopGathering();
    }


    // =============================
    // ▼ 全体攻撃
    // =============================
    void PerformWaveAttack()
    {
        if (player == null) return;

        float dist = Vector2.Distance(player.position, transform.position);
        if (dist <= waveRange)
        {
            player.GetComponent<Player>()?.TakeDamage(waveDamage);
        }

        Debug.Log("🔥 SplitBoss：全体攻撃発動！");
    }
}
