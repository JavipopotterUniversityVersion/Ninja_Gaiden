using UnityEngine;

public class aSaltoNuevo : MonoBehaviour
{
    public float jumpVelocity = 10f;
    public float gravity = -20f;
    public float maxFallSpeed = -25f;
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
        HandleInput();
        ApplyGravity();
        MoveCharacter();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
            isGrounded = false;
            verticalVelocity = jumpVelocity;

            float horizontalInput = Input.GetAxisRaw("Horizontal");
            horizontalVelocity = horizontalInput != 0 ? Mathf.Sign(horizontalInput) * arcSpeed : 0f;
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
            if (verticalVelocity < maxFallSpeed)
                verticalVelocity = maxFallSpeed;
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
