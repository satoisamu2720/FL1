using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;      // �X�N���[�����x
    private Vector3 targetPos;
    private bool isMoving = false;

    private void Start()
    {
        if (player != null)
            targetPosition = new Vector3(player.position.x, player.position.y, -10f);
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // �J�����ړ�������������I��
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                transform.position = targetPos;
                isMoving = false;
            }
        }
    }

    // �J�������w������Ɉړ�
    public void MoveToNextRoom(Vector2 direction, float roomWidth, float roomHeight)
    {
        if (isMoving) return;

        targetPos = transform.position + new Vector3(direction.x * roomWidth, direction.y * roomHeight, 0);
        isMoving = true;
    }

    // �J�����ړ����͑��X�N���v�g�����Ԋm�F�ł���悤��
    public bool IsMoving()
    {
        return isMoving;
    }
}
