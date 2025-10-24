using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public Vector3 cameraNewPosition; // ���̕����̒��S���W

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Camera.main.GetComponent<RoomCamera>().MoveToRoom(cameraNewPosition);
        }
    }
}
