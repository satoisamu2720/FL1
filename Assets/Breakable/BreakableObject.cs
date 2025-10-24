using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Header("����Ƃ��̃G�t�F�N�g")]
    public GameObject breakEffect;

    [Header("�h���b�v����n�[�g�v���n�u")]
    public GameObject heartPrefab;

    [Header("�n�[�g�̃h���b�v�m��")]
    [Range(0f, 1f)]
    public float heartDropChance = 0.25f; // 25%�Ńh���b�v

    private bool isBroken = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���������������
        if (isBroken) return;

        if (other.CompareTag("Sword"))
        {
            Break();
        }
    }

    void Break()
    {
        isBroken = true;

        if (breakEffect != null)
            Instantiate(breakEffect, transform.position, Quaternion.identity);

        TryDropHeart();

        Destroy(gameObject); // ��⑐������
    }

    void TryDropHeart()
    {
        float rand = Random.value; 
        if (rand < heartDropChance && heartPrefab != null)
        {
            Instantiate(heartPrefab, transform.position, Quaternion.identity);
        }
    }
}
