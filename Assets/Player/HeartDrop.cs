using UnityEngine;

public class HeartDrop : MonoBehaviour
{
    public int healAmount = 4; // �񕜗�

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Status.Instance.RecoverHP(healAmount);
            Debug.Log($"�n�[�g���E�����I�i+{healAmount}�j");
            

            Destroy(gameObject);
        }
    }
}
