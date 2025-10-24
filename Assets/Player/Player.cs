using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    // ���M�h�{�f�B2D
    public Rigidbody2D rb;
    // �ړ��p�ϐ�
    private Vector2 movement;
    // �}�b�v�ړ����̃t���O
    private bool canMove = true;
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

    public void EnableMovement(bool enable)
    {
        canMove = enable;
        if (!enable)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

}
