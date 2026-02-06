using System.Collections;
using UnityEngine;

public class TriggerSpawn : MonoBehaviour
{
    public EnemySpawn enemySpawner;
    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Physics2D.IgnoreCollision(
            other.GetComponent<Collider2D>(),
            GetComponent<Collider2D>(),
            true
        );

        StartCoroutine(SpawnDelay());
    }

    IEnumerator SpawnDelay()
    {
        // ▼ Trigger を一時的に無効化
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // ワープ直後のゴチャつき回避
        yield return new WaitForSeconds(0.3f);

        // ▼ 敵スポーン
        enemySpawner.SpawnEnemiesManually();

        // ▼ Trigger を再有効化（必要なら）
        if (col != null)
            col.enabled = true;
    }
   

}
