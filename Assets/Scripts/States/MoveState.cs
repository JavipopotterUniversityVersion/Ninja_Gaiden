using UnityEngine;

public class MoveState : IState
{
    [SerializeField] MovementController _movementController;
    [SerializeField] SpriteRenderer _spriteRenderer;

    public override void Enter() => _brain.PlayAnimation(StateNames.PLAYER_WALK);

    public override void StateUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(horizontal, vertical);

        if (direction != Vector2.zero)
        {
            _movementController.Move(direction);
            _spriteRenderer.flipX = direction.x < 0;
        }
        else _brain.ChangeState("IdleState");

        if (Input.GetKeyDown(KeyCode.Space)) _brain.ChangeState("JumpState");
    }
}
