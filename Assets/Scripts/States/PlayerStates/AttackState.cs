using System.Collections;
using UnityEngine;

public class AttackState : IState
{
    public override void Enter() {
        _brain.PlayAnimation(StateNames.PLAYER_ATTACK);
        StartCoroutine(AttackRoutine());
    }

    public override void ReceiveInput(string inputName) { }
    
    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        _brain.ChangeState(StateNames.PLAYER_IDLE);
    }
}
