using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifeTime = 3f;
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("ìGÇ…ñΩíÜÅI");
            Destroy(gameObject);
        }
    }
}
