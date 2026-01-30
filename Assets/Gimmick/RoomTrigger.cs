using UnityEngine;
using static MapManager;

public class RoomTrigger : MonoBehaviour
{
    public OpenDoor[] doors;
    public RoomConditionType conditionType;
    public int requiredCount;
    public GameObject[] enemies;

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;

        //Ç‹Ç∏ïKÇ∏ï¬ÇﬂÇÈ
        foreach (var door in doors)
        {
            door.Close();
        }

        // èåèäJén
        if (conditionType == RoomConditionType.Battle)
        {
            MapManager.Instance.StartCondition(
                RoomConditionType.Battle,
                enemies.Length
            );
        }
        else
        {
            MapManager.Instance.StartCondition(
                RoomConditionType.Gimmick,
                0
            );
        }
    }
}

