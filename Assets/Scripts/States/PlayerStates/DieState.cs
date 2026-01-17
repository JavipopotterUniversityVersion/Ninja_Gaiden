using System.Collections;
using UnityEngine;

public class DieState : IState
{
    public override void Enter() {
        _brain.PlayAnimation(StateNames.PLAYER_GET_DAMAGE);
        StartCoroutine(GetDamageRoutine());
    }

    IEnumerator GetDamageRoutine()
    {
        yield return new WaitForSeconds(0.5f);
    }
}
