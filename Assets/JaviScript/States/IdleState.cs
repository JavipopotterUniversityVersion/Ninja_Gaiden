using UnityEngine;

public class IdleState : IState
{
    public override void StateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space)) _brain.ChangeState("JumpState");

         float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0) _brain.ChangeState("MoveState");
    }
}
