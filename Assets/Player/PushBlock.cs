using UnityEngine;
using System.Collections;

public class PushBlock : MonoBehaviour
{
    public float moveTime = 0.15f;    // 1マス動く時間
    private bool isMoving = false;

    public LayerMask obstacleLayer;   // 壁, 他のブロック, Tilemap

    // プレイヤーから呼ばれる
    public void TryPush(Vector2 dir)
    {
        if (isMoving) return;

        // 1マス先
        Vector2 targetPos = (Vector2)transform.position + dir;

        // 移動先に何かある？
        if (Physics2D.OverlapBox(targetPos, Vector2.one * 0.8f, 0, obstacleLayer))
        {
            return; // 移動不可
        }

        StartCoroutine(MoveBlock(targetPos));
    }

    private IEnumerator MoveBlock(Vector2 target)
    {
        isMoving = true;
        Vector2 start = transform.position;
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector2.Lerp(start, target, elapsed / moveTime);
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.9f);
    }
}
