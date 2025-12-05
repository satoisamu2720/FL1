using UnityEngine;

//public class PlayerPush : MonoBehaviour
//{
//    public float checkDistance = 0.6f; // ブロックに触れてる距離判定

//    void Update()
//    {
//        Vector2 dir = Vector2.zero;

//        if (Input.GetKey(KeyCode.UpArrow))
//        { dir = Vector2.up; }

//        if (Input.GetKey(KeyCode.DownArrow)) 
//        { dir = Vector2.down; }
//        if (Input.GetKey(KeyCode.LeftArrow))
//        { dir = Vector2.left; }
//        if (Input.GetKey(KeyCode.RightArrow)) 
//            {dir = Vector2.right;

//        if (dir != Vector2.zero)
//        {
//            // 目の前にブロックがあるか
//            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, checkDistance, LayerMask.GetMask("Block"));
//            if (hit.collider != null)
//            {
//                PushBlock block = hit.collider.GetComponent<PushBlock>();
//                block.TryPush(dir);
//            }
//        }
//    }
//}