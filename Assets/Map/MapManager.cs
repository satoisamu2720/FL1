using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public int currentMapID;

    [Header("現在マップの条件達成数")]
    public int OpenDoorNum = 0;

    private void Awake()
    {
        Instance = this;
    }

    // 敵やギミックから呼ぶ
    public void AddOpenDoorNum(int value = 1)
    {
        OpenDoorNum += value;
        Debug.Log($"条件達成 +{value} / 現在 {OpenDoorNum}");
    }

    public void ResetMapCount()
    {
        OpenDoorNum = 0;
    }

    public void MapTransition(int toMapID)
    {
        currentMapID = toMapID;
        ResetMapCount();
    }
}
