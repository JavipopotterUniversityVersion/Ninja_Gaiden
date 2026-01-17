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

        if (horizontal != 0 || vertical != 0) _brain.ChangeState(StateNames.PLAYER_WALK);
    }
}
