using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 3f;
    public float moveSpeed = 3f;

    private Vector3 targetPosition;
    private bool isManualMoving = false;

    void Update()
    {
        if (!isManualMoving)
        {
            //プレイヤーを追従
            targetPosition = new Vector3(player.position.x, player.position.y, -10f);
        }

        // カメラ移動（プレイヤー追従または手動スクロール）
        transform.position = Vector3.Lerp(transform.position, targetPosition,
            (isManualMoving ? moveSpeed : followSpeed) * Time.deltaTime);
    }

    // マップを切り替える時だけ呼ぶ
    public void MoveTo(Vector3 newPosition)
    {
        isManualMoving = true;
        targetPosition = new Vector3(newPosition.x, newPosition.y, -10f);

        // 数秒後に追従モードに戻す（1秒後など）
        StartCoroutine(ReturnToFollow(1f));
    }

    private System.Collections.IEnumerator ReturnToFollow(float delay)
    {
        yield return new WaitForSeconds(delay);
        isManualMoving = false;
    }
}