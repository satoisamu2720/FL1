using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    public static Status Instance { get; private set; }

    [Header("プレイヤーステータス")]
    public float PlayerHP = 5;
    public float PlayerMP = 10;
    public float PlayerSpeed = 1.0f;
    public float PlayerDashSpeed = 1.0f;
    public float PlayerDashCoolTime = 2.0f;
    public float MaxMP = 10;
    public float MaxHP = 10;

    [Header("MPバーUI")]
    public Image mpFillImage;


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
    }
    
    // Update is called once per frame
    void Update()
    {
        
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

    void UpdateMPUI()
    {
        if (mpFillImage != null)
        {
            mpFillImage.fillAmount = (float)PlayerMP / MaxMP;
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
