using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DivisionEnemy") || other.CompareTag("MiniEnemy"))
        {
            DivisionEnemy enemy = other.GetComponent<DivisionEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }
        }
    }
}
