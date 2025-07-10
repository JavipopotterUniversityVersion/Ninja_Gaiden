using System.Collections;
using UnityEngine;

public class GetDamageState : IState
{
    public override void Enter() {
        _brain.PlayAnimation(StateNames.PLAYER_GET_DAMAGE);
        StartCoroutine(GetDamageRoutine());
    }

    public override void ReceiveInput(string inputName) { }
    IEnumerator GetDamageRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        _brain.ChangeState(StateNames.PLAYER_IDLE);
    }
}
