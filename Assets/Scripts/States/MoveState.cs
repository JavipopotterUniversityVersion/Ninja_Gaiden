using UnityEngine;

public class MoveState : IState
{
    [SerializeField] MovementController _movementController;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Joystick joystick;

    public override void Enter() => _brain.PlayAnimation(StateNames.PLAYER_WALK);

    public override void ReceiveInput(string inputName)
    {
        switch (inputName)
        {
            case "JUMP":
                _brain.ChangeState(StateNames.PLAYER_JUMP);
                break;
            case "ATTACK":
                _brain.ChangeState(StateNames.PLAYER_ATTACK);
                break;
        }
    }

    public override void StateUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal") + joystick.Horizontal;
        float vertical = Input.GetAxisRaw("Vertical") + joystick.Vertical;

        Vector2 direction = new Vector2(horizontal, vertical);

        if (direction != Vector2.zero)
        {
            _movementController.Move(direction);
            _spriteRenderer.flipX = direction.x < 0;
        }
        else _brain.ChangeState(StateNames.PLAYER_IDLE);
    }
}
