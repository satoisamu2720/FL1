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
        // 最初は非表示
        chestObject.SetActive(false);
    }

    void Update()
    {
        if (spawned) return;

        // マップIDが違うなら無視
        if (MapManager.Instance.currentMapID != mapID) return;

        // 戦闘条件がクリアされたら
        if (MapManager.Instance.currentConditionType == MapManager.RoomConditionType.Battle
            && MapManager.Instance.IsConditionCleared())
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
