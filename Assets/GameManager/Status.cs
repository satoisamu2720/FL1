using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    public static Status Instance { get; private set; }

    [Header("プレイヤーステータス")]
    public int PlayerHp = 5;
    public int PlayerMP = 10;
    public float PlayerSpeed = 1.0f;
    public float PlayerDashSpeed = 1.0f;
    public float PlayerDashCoolTime = 2.0f;
    public int MaxMP = 10;

    [Header("UI")]
    public Slider mpSlider;


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
    // 魔力消費
    public bool UseMP(int amount)
    {
        if (PlayerMP >= amount)
        {
            PlayerMP -= amount;
            UpdateMPUI();
            return true;
        }
        return false; // 足りない
    }

    // 魔力回復
    public void RecoverMP(int amount)
    {
        PlayerMP = Mathf.Min(PlayerMP + amount, MaxMP);
        UpdateMPUI();
    }

    private void UpdateMPUI()
    {
        Debug.Log($"MP更新: {PlayerMP}/{MaxMP}");
        if (mpSlider != null)
        {
            mpSlider.value = (float)PlayerMP / MaxMP;
            Debug.Log($"スライダー値: {mpSlider.value}");
        }
        else
        {
            Debug.LogWarning("MPBarのSliderが未設定です！");
        }
    }

    void Awake()
    {
        Instance = this;
        PlayerMP = MaxMP;
        UpdateMPUI();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
