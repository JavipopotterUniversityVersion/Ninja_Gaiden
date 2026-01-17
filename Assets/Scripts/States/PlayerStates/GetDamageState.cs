using System.Collections;
using UnityEngine;

public class GetDamageState : IState
{
    [SerializeField] string _stateAnimation = StateNames.PLAYER_GET_DAMAGE;
    [SerializeField] string _nextState = StateNames.PLAYER_IDLE;
    public override void Enter()
    {
        _brain.PlayAnimation(_stateAnimation);
        StartCoroutine(GetDamageRoutine());
    }

    public override void ReceiveInput(string inputName) { }
    IEnumerator GetDamageRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        _brain.ChangeState(_nextState);
    }
}
