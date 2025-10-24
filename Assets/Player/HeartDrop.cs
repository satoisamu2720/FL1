using UnityEngine;

public class HeartDrop : MonoBehaviour
{
    public float healAmount = 1f; // 回復量（1=ハート1個分）

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<Player>();
            if (health != null)
            {
                health.Recover(healAmount);
                Debug.Log($"ハートを拾った！（+{healAmount}）");
            }

            Destroy(gameObject);
        }
    }
}
