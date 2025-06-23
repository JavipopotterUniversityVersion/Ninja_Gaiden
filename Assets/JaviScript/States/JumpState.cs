using System.Collections;
using UnityEngine;

public class JumpState : IState
{
    public override void Enter()
    {
        StartCoroutine(JumpCoroutine());
    }

    IEnumerator JumpCoroutine()
    {
        _brain.GetAnimator().Play("Salto1");
        yield return new WaitForSeconds(0.7f);
        _brain.ChangeState("IdleState");
    }
}
