using UnityEngine;
using static UnityEditor.Progress;

public class TriggerSpawn : MonoBehaviour
{
    public EnemySpawn enemySpawner; // Inspector で指定
    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
            if (!hasTriggered && other.CompareTag("Player"))
            {
                enemySpawner.SpawnEnemiesManually();
                Debug.Log($"スポーンした");
                hasTriggered = true;
            }

    }
}
