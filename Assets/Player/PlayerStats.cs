using UnityEngine;


public class PlayerStats : MonoBehaviour
{
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateExpToNextLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //経験値の取得
    public void AddExperience(int amount)
    {
        currentExp += amount;
        Debug.Log("Exp + " + amount + " Current Exp: " + currentExp);

        //レベルアップ判定
        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }

    
    }

    //レベルアップ処理
    void LevelUp()
    {
        level++;
        Debug.Log("Level Up! New Level: " + level);
        UpdateExpToNextLevel();
    }

    //次のレベルまでの経験値設定
    void UpdateExpToNextLevel()
    {
        expToNextLevel = 100;
    }
}
