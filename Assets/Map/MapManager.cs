using UnityEngine;
using static OpenDoor;

public class MapManager : MonoBehaviour
{
    public enum RoomConditionType
    {
        Battle,
        Gimmick
    }
    public static MapManager Instance;

    public int currentMapID;

    [Header("現在の条件数")]
    public int currentCount = 0;

    [Header("現在の条件タイプ")]
    public RoomConditionType currentConditionType;

    private void Awake()
    {
        Instance = this;
    }

    // ===== 条件開始 =====
    public void StartCondition(RoomConditionType type, int startCount = 0)
    {
        currentConditionType = type;
        currentCount = startCount;
    }

    // ===== カウント加算 =====
    public void AddCount(int value = 1)
    {
        currentCount += value;
    }

    // ===== 敵撃破 =====
    public void EnemyDefeated()
    {
        if (currentConditionType != RoomConditionType.Battle) return;
        currentCount--;
    }

    public bool IsConditionCleared()
    {
        return currentCount <= 0;
    }
}
