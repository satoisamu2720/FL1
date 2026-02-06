using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    public bool IsReflected { get; private set; }
    public ReflectMage shooter;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // ”½Ëˆ—
    public void Reflect(Vector2 newDirection)
    {
        IsReflected = !IsReflected; // ”½Ëó‘Ô‚ğƒgƒOƒ‹
        rb.linearVelocity = newDirection.normalized * rb.linearVelocity.magnitude;
    }
}
