using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    public ReflectMage shooter;
    public float speed = 10f;

    Rigidbody2D rb;

    public bool IsReflected { get; private set; } = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Reflect(Vector2 reflectDir)
    {
        if (IsReflected) return;

        IsReflected = true;

        rb.linearVelocity = reflectDir.normalized * speed;

        int layer = LayerMask.NameToLayer("PlayerBullet");
        if (layer != -1)
            gameObject.layer = layer;
        else
            Debug.LogWarning("PlayerBullet layer not found!");

        gameObject.tag = "PlayerBullet";
        GetComponent<SpriteRenderer>().color = Color.cyan;
    }

}
