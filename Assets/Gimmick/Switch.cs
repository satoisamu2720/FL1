using UnityEngine;

public class Switch : MonoBehaviour
{

    public static Switch Instance;

    [Header("スイッチがオンのときに動かすオブジェクト")]
    public GameObject targetObject;

    [Header("スイッチ管理")]
    public bool oneTime = true;

    private bool isActivated = false;


    private void Awake()
    {
        Instance = this;

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated && oneTime) 
        { 
            return; 
        }

        // 攻撃オブジェクトタグ判定
        if (other.CompareTag("Sword") ||
            other.CompareTag("Arrow") ||
            other.CompareTag("Bomb"))
        {
            ActivateSwitch();
        }
    }

    void ActivateSwitch()
    {
        isActivated = true;

        Debug.Log("作動");

        if (targetObject != null)
        {
            // ここで仕掛けのスクリプトを呼ぶ
            var mechanism = targetObject.GetComponent<IMechanism>();
            if (mechanism != null)
            {
                mechanism.Activate();
                MapManager.Instance.AddCount(1);
            }
        }
    }

    public interface IMechanism
    {
        void Activate();

    }

    
}
