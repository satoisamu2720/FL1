using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Movement Settings")]
    public float moveSpeed = 5f; // �� �ړ��X�s�[�h�ݒ�

    private Rigidbody2D rb;
    private Vector2 movement;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ���͎擾�iWASD / �����L�[�j
        movement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
