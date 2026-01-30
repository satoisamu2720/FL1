using UnityEngine;

public class MapTransitionTrigger : MonoBehaviour
{
    [Header("次マップのカメラ位置を中心座標")]
    public Vector3 cameraTargetPosition;

    [Header("プレイヤーの移動先のオブジェクト")]
    public GameObject playerTargetObject;
    [Header("プレイヤーの移動先で押し出す距離")]
    public float playerPush = 1.0f;

    public float transitionDelay = 0.5f;

    private bool isTransitioning = false;

    public int currentMapID = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            StartCoroutine(Transition(other.transform));
        }
    }

    private System.Collections.IEnumerator Transition(Transform player)
    {
        isTransitioning = true;

        var move = player.GetComponent<Player>();
        if (move != null)
        {
            move.enabled = false;
        }

        // カメラ移動
        Camera.main.GetComponent<CameraController>().MoveTo(playerTargetObject.transform.position);

        yield return new WaitForSeconds(transitionDelay);

        // ワープ前の位置を保持
        Vector3 beforePos = player.position;

        // ワープ先に移動
        Vector3 targetPos = playerTargetObject.transform.position;
        player.position = targetPos;

        // 押し出す方向を自動判定
        Vector2 direction = (targetPos - beforePos).normalized;

        // 正規化の結果が (0,0) の場合（同じ場所等）対策
        if (direction == Vector2.zero)
        {
            // プレイヤーとトリガーの位置関係から決める
            Vector2 diff = (player.position - transform.position);
            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                direction = new Vector2(Mathf.Sign(diff.x), 0f);
            }
            else
            {
                direction = new Vector2(0f, Mathf.Sign(diff.y));
            }
        }

        // 押し出す量
        player.position += (Vector3)direction * playerPush;

        MapManager.Instance.currentMapID = currentMapID;

        yield return new WaitForSeconds(transitionDelay);

        if (move != null)
        {
            move.enabled = true;
        }

        isTransitioning = false;
    }
}
