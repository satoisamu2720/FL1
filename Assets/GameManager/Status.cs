using UnityEngine;

public class Status : MonoBehaviour
{
    public static Status Instance { get; private set; }

    [Header("プレイヤーステータス")]
    public int PlayerHp = 5;
    public float PlayerSpeed = 1.0f;
    public float PlayerDashSpeed = 1.0f;
    public float PlayerDashCoolTime = 2.0f;

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

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
