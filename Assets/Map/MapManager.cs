using UnityEngine;

public class MapManager : MonoBehaviour
{
    public enum RoomConditionType
    {
        Battle,
        Gimmick,
        Boss,
        BossGimmick,
    }

    public static MapManager Instance;

    public int currentMapID;

    [Header("敵条件")]
    public int enemyRemaining = 0;

    [Header("ボタン条件")]
    public int ButtonCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    // ===== 戦闘開始 =====
    public void StartBattle(int enemyCount)
    {
        enemyRemaining = enemyCount;
    }

    // ===== 敵撃破 =====
    public void EnemyDefeated()
    {
        enemyRemaining--;
    }

    // ===== ボタン押下 =====
    public void PressButton()
    {
        Debug.Log($"{ButtonCount}個目");
        ButtonCount++;
    }

    // ===== 条件判定 =====
    public bool CanOpen(RoomConditionType type, int requiredCount)
    {
        switch (type)
        {
            case RoomConditionType.Battle:
                return enemyRemaining <= 0;

            case RoomConditionType.Gimmick:
                return ButtonCount >= requiredCount;

            case RoomConditionType.Boss:
                return enemyRemaining <= 0;

            case RoomConditionType.BossGimmick:
                return enemyRemaining <= 0 && ButtonCount >= requiredCount;
        }
        return false;
    }
}
