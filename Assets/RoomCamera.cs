using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;

    private Vector3 targetPos;
    private bool isMoving = false;

    void Start()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                transform.position = targetPos;
                isMoving = false;
                player.GetComponent<Player>().EnableMovement(true);
            }
        }
    }

    public void MoveToRoom(Vector3 newPos)
    {
        targetPos = new Vector3(newPos.x, newPos.y, transform.position.z);
        isMoving = true;
        player.GetComponent<Player>().EnableMovement(false);
    }
}
