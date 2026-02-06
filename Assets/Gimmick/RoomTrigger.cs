using UnityEngine;
using static MapManager;

public class RoomTrigger : MonoBehaviour
{
    public OpenDoor[] doors;
    public EnemySpawn enemySpawn;

    private bool activated = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;

        //扉を閉める
        foreach (var door in doors)
        {
            door.CloseByBattle();
        }

        //敵をスポーン
        int enemyCount = enemySpawn.SpawnEnemiesManually();

        //戦闘開始
        MapManager.Instance.StartBattle(enemyCount);
    }
}
