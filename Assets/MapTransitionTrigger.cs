using UnityEngine;

public class MapTransitionTrigger : MonoBehaviour
{
    public Vector3 cameraTargetPosition;  // 次のマップ中心
    public Vector3 playerTargetPosition;  // プレイヤーの新しい位置
    public float transitionDelay = 0.5f;  // スクロール演出の間

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Transition(other.transform));
        }
    }

    private System.Collections.IEnumerator Transition(Transform player)
    {
        isTransitioning = true;

        // プレイヤーの操作を一時停止（例：PlayerMovementスクリプト無効化）
        var move = player.GetComponent<Player>();
        if (move != null) move.enabled = false;

        // カメラを移動
        Camera.main.GetComponent<RoomCamera>().MoveTo(cameraTargetPosition);

        // 少し待ってからプレイヤー移動
        yield return new WaitForSeconds(transitionDelay);
        player.position = playerTargetPosition;

        // プレイヤー操作を戻す
        if (move != null) move.enabled = true;

        isTransitioning = false;
    }
}
