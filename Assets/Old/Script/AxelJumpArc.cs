
using UnityEngine;

public class AxelJumpArc : MonoBehaviour
{
    public float jumpVelocity = 10f;
    public float gravity = -20f;
    public float minFallSpeed = -25f;
    public float arcSpeed = 3f;

    private float verticalVelocity = 0f;
    private float horizontalVelocity = 0f;
    private bool isJumping = false;
    private bool isGrounded = true;
    private Vector2 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        ApplyGravity();
        MoveCharacter();
    }

    public void Jump()
    {
        isJumping = true;
        isGrounded = false;
        verticalVelocity = jumpVelocity;

        horizontalVelocity = arcSpeed;
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
            if (verticalVelocity < minFallSpeed)
                verticalVelocity = minFallSpeed;
        }
    }

    void MoveCharacter()
    {
        if (!isGrounded)
        {
            Vector3 move = new Vector3(horizontalVelocity, verticalVelocity, 0) * Time.deltaTime;
            transform.position += move;

            if (transform.position.y <= startPosition.y)
            {
                transform.position = new Vector3(transform.position.x, startPosition.y, transform.position.z);
                isGrounded = true;
                isJumping = false;
                verticalVelocity = 0f;
                horizontalVelocity = 0f;
            }
        }
    }
}
