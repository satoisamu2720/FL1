using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;

    private Vector3 targetPosition;

    void Start()
    {
        if (player != null)
            targetPosition = new Vector3(player.position.x, player.position.y, -10f);
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // 他のスクリプトから呼び出してカメラを動かす
    public void MoveTo(Vector3 newPosition)
    {
        targetPosition = new Vector3(newPosition.x, newPosition.y, -10f);
    }
}
