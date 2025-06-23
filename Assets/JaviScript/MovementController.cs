using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] float _speed;
    Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 direction)
    {
        Vector2 movement = _speed * Time.deltaTime * direction.normalized;
        _rb.linearVelocity = new Vector2(movement.x, movement.y);
    }
    
    public void Stop()
    {
        _rb.linearVelocity = new Vector2(0, 0);
    }
}
