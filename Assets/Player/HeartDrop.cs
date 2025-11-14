using UnityEngine;

public class HeartDrop : MonoBehaviour
{
    public int healAmount = 4; // 回復量

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Status.Instance.RecoverHP(healAmount);
            Debug.Log($"ハートを拾った！（+{healAmount}）");
            

            Destroy(gameObject);
        }
    }
}
