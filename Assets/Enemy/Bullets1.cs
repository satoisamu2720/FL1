using UnityEngine;

public class Bullets : MonoBehaviour
{
    public float speed = 8f;
    private Vector2 moveDir;

    public void SetDirection(Vector2 dir)
    {
        moveDir = dir.normalized;
    }

    void Update()
    {
        transform.position += (Vector3)moveDir * speed * Time.deltaTime;
    }
    public interface IPlayerDamageable
    {
        void TakeDamage(int damage);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IPlayerDamageable player = other.GetComponent<IPlayerDamageable>();

        if (player != null)
        {
            player.TakeDamage(1);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }


        if (other.CompareTag("Player"))
        {
            if (player != null)
            {
                player.TakeDamage(1);
            }

            Destroy(gameObject);
            return;
        }
    }
}
