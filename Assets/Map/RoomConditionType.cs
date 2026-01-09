using UnityEngine;
using static MapManager;

public class RoomTrigger : MonoBehaviour
{
    public RoomConditionType conditionType;

    [Header("戦闘用")]
    public OpenDoor[] doors;
    public GameObject[] enemies;

    [Header("ギミック用")]
    public int gimmickTargetCount = 1;

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;

        if (conditionType == RoomConditionType.Battle)
        {
            foreach (var door in doors)
                door.Close();

            foreach (var enemy in enemies)
                enemy.SetActive(true);

            MapManager.Instance.StartCondition(
                RoomConditionType.Battle,
                enemies.Length
            );
        }

        if (conditionType == RoomConditionType.Gimmick)
        {
            MapManager.Instance.StartCondition(
                RoomConditionType.Gimmick,
                0
            );
        }
    }
}
