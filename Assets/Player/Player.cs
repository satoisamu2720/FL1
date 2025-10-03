using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    // リギドボディ2D
    public Rigidbody2D rb;
    // 移動用変数
    private Vector2 movement;  
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPause == false)
        {
            MovePlayer();
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        }
    }

    private void MovePlayer()
    {
        rb.MovePosition(rb.position + movement * Status.Instance.PlayerSpeed * Time.deltaTime);
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
