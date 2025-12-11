using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        ISwordDamageable target = other.GetComponent<ISwordDamageable>();

        if (target != null)
        {
            target.TakeDamage(1);
        }
    }
}
