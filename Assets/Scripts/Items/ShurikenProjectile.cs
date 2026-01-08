using UnityEngine;

/// <summary>
/// Projectile that moves in a straight line and damages enemies
/// </summary>
public class ShurikenProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;
    
    private Vector2 direction;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }
        
        Destroy(gameObject, lifetime);
    }
    
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        
        // Set velocity once
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
        
        // Rotate shuriken to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if hit an enemy
        if (collision.CompareTag("Enemy"))
        {
            HealthHandler health = collision.GetComponent<HealthHandler>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            
            Destroy(gameObject);
        }
        // Check if hit a wall or obstacle
        else if (collision.CompareTag("Wall") || collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
