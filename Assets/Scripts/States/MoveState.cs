using UnityEngine;

public class MoveState : IState
{
    [SerializeField] MovementController _movementController;

    public override void StateUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(horizontal, vertical);
        
        if (direction != Vector2.zero) _movementController.Move(direction);
        else _brain.ChangeState("IdleState");

        if (Input.GetKeyDown(KeyCode.Space)) _brain.ChangeState("JumpState");
    }
}
