using UnityEngine;

public class HeartDrop : MonoBehaviour
{
    public float healAmount = 1f; // �񕜗ʁi1=�n�[�g1���j

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<Player>();
            if (health != null)
            {
                health.Recover(healAmount);
                Debug.Log($"�n�[�g���E�����I�i+{healAmount}�j");
            }

            Destroy(gameObject);
        }
    }
}
