using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Header("壊れるときのエフェクト")]
    public GameObject breakEffect;

    [Header("ドロップするハートプレハブ")]
    public GameObject heartPrefab;

    [Header("ハートのドロップ確率")]
    [Range(0f, 1f)]
    public float heartDropChance = 0.25f; // 25%でドロップ

    private bool isBroken = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 剣が当たったら壊す
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

        Destroy(gameObject); // 壺や草を消す
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
