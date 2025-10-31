using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;      // スクロール速度
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

            // カメラ移動が完了したら終了
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                transform.position = targetPos;
                isMoving = false;
            }
        }
    }

    // カメラを指定方向に移動
    public void MoveToNextRoom(Vector2 direction, float roomWidth, float roomHeight)
    {
        if (isMoving) return;

        targetPos = transform.position + new Vector3(direction.x * roomWidth, direction.y * roomHeight, 0);
        isMoving = true;
    }

    // カメラ移動中は他スクリプトから状態確認できるように
    public bool IsMoving()
    {
        return isMoving;
    }
}
