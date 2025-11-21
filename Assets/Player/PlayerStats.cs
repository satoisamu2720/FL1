using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    // 強化ステータス
    public int maxHP = 10;
    public int attack = 1;

    private void Awake()
    {
        Instance = this;
    }

    public void AddExperience(int amount)
    {
        currentExp += amount;
        Debug.Log($"EXP + {amount} (Total {currentExp})");

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        Debug.Log($"LEVEL UP! → {level}");

        // ★ ステータス強化
        maxHP += 2;
        attack += 1;

        Debug.Log($"Status: HP={maxHP} / ATK={attack}");

        // ★ 5レベルごとにアイテム付与
        if (level % 5 == 0)
        {
            int rewardItemID = 0;

            if (level == 5) rewardItemID = 0; //剣
            if (level == 10) rewardItemID = 1;  // 弓
            if (level == 15) rewardItemID = 2;  // 爆弾

            Inventory.Instance.AddItem(rewardItemID);

            Debug.Log("レベル報酬アイテムを付与！");
        }

        UpdateExpToNextLevel();
    }

    void UpdateExpToNextLevel()
    {
        expToNextLevel = 100 + (level - 1) * 25;
    }
}
