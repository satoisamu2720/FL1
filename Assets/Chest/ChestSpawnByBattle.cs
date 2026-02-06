using UnityEngine;

public class ChestSpawnByBattle : MonoBehaviour
{
    [Header("対象マップID")]
    public int mapID = 2;

    [Header("宝箱")]
    public GameObject chestObject;

    private bool spawned = false;

    void Start()
    {
        chestObject.SetActive(false);
    }

    void Update()
    {
        if (spawned) return;

        // マップIDが違うなら無視
        if (MapManager.Instance.currentMapID != mapID) return;

        // 敵が全滅したら宝箱出現
        if (MapManager.Instance.enemyRemaining <= 0)
        {
            SpawnChest();
        }
    }

    void SpawnChest()
    {
        spawned = true;
        chestObject.SetActive(true);
        Debug.Log("宝箱が出現しました！");
    }
}
