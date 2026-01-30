using UnityEngine;

public class Switch : MonoBehaviour
{
    public static Switch Instance;

    [Header("スイッチがオンのときに動かすオブジェクト")]
    public GameObject targetObject;

    [Header("スイッチ管理")]
    public bool oneTime = true;

    [Header("見た目")]
    public Sprite offSprite;
    public Sprite onSprite;

    private bool isActivated = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 初期状態はOFF
        SetVisual(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated && oneTime) return;

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
        SetVisual(true);

        Debug.Log("作動");

        if (targetObject != null)
        {
            var mechanism = targetObject.GetComponent<IMechanism>();
            if (mechanism != null)
            {
                mechanism.Activate();
                MapManager.Instance.AddCount(1);
            }
        }
    }

    void SetVisual(bool on)
    {
        if (spriteRenderer == null) return;
        spriteRenderer.sprite = on ? onSprite : offSprite;
    }

    public interface IMechanism
    {
        void Activate();
    }
}
