using UnityEngine;

public class IdleState : IState
{
    [SerializeField] MovementController _movementController;
    [SerializeField] Joystick joystick;

    public override void Enter()
    {
        _movementController.Stop();
        _brain.PlayAnimation(StateNames.PLAYER_IDLE);

    }

    public override void StateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space)) _brain.ChangeState("JumpState");

        float horizontal = Input.GetAxisRaw("Horizontal") + joystick.Horizontal;
        float vertical = Input.GetAxisRaw("Vertical") + joystick.Vertical;

        if (horizontal != 0 || vertical != 0) _brain.ChangeState("MoveState");
    }
}
