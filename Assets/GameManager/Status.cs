using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    public static Status Instance { get; private set; }

    [Header("プレイヤーステータス")]
    public int PlayerHP = 5;
    public float PlayerMP = 10;
    public float PlayerSpeed = 1.0f;
    public float PlayerDashSpeed = 1.0f;
    public float PlayerDashCoolTime = 2.0f;
    public int MaxHearts = 3;   // 最大ハート数（1ハート＝4HP）
    public int HeartUnit = 4;   // 1ハートあたりのHP単位
    public int MaxHP => MaxHearts * HeartUnit;
    public float MaxMP = 10;


    [Header("UI")]
    public Image mpFillImage;
    public Image heartPrefab;          // ハートUIプレハブ
    public Transform heartParent;     
    public Sprite[] heartSprites;      // [0~4] 各段階のスプライト
    public float heartSpacing = 40f; // ハートの間隔（ピクセル単位）

    private List<Image> hearts = new List<Image>();

    [System.Serializable]
    public class UserData
    {
        //プレイヤー
        public Vector3 position;
        public int playerLevel;
        public int health;
        public float speed;
        public float dashSpeed;
        public float dashCoolTime;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateMPUI();
        UpdateHearts();

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        PlayerHP = Mathf.Max(0, PlayerHP - amount);
        UpdateHearts();
        Debug.Log($"ダメージ: {amount} 現在HP: {PlayerHP}/{MaxHP}");
    }

    public void RecoverHP(int amount)
    {
        PlayerHP = Mathf.Min(MaxHP, PlayerHP + amount);
        UpdateHearts();
        Debug.Log($"回復: {amount} 現在HP: {PlayerHP}/{MaxHP}");
    }

    public void RecoverQuarterHeart()
    {
        RecoverHP(1); // 1/4ハート回復
    }

    public void RecoverFullHeart()
    {
        RecoverHP(HeartUnit); // 4/4 = 1ハート回復
    }
    public bool UseMP(int amount)
    {
        if (PlayerMP >= amount)
        {
            PlayerMP = Mathf.Clamp(PlayerMP - amount, 0, MaxMP);
            UpdateMPUI();
            return true;
        }
        return false;
    }
    public void RecoverMP(int amount)
    {
        PlayerMP = Mathf.Clamp(PlayerMP + amount, 0, MaxMP);
        UpdateMPUI();
    }

    void UpdateHearts()
    {
        if (heartPrefab == null || heartParent == null)
        {
            Debug.LogWarning("heartPrefab または heartParent が設定されていません！");
            return;
        }

        // 足りなければハートを生成
        while (hearts.Count < MaxHearts)
        {
            Image newHeart = Instantiate(heartPrefab, heartParent);
            hearts.Add(newHeart);
        }

        // 並べて配置
       
        for (int i = 0; i < hearts.Count; i++)
        {
            int heartValue = Mathf.Clamp(PlayerHP - i * HeartUnit, 0, HeartUnit);
            hearts[i].sprite = heartSprites[heartValue];

            // 位置をずらして配置
            hearts[i].rectTransform.anchoredPosition = new Vector2(i * heartSpacing, 0);
        }
    }



    void UpdateMPUI()
    {
        if (mpFillImage != null)
        {
            mpFillImage.fillAmount = PlayerMP / MaxMP;
        }
    }
    void Awake()
    {
        Instance = this;
        PlayerMP = MaxMP;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
