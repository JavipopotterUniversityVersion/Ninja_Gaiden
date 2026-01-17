using UnityEngine;

public class DamageFilterByState : MonoBehaviour, IDamageable
{
    HealthHandler _healthHandler;
    [SerializeField]StateMachine _stateMachine;

    private void Awake()
    {
        _healthHandler = GetComponent<HealthHandler>();
    }

    public void TakeDamage(int damage)
    {
        if (_stateMachine.CurrentStateName == StateNames.PLAYER_GET_DAMAGE ||
            _stateMachine.CurrentStateName == StateNames.PLAYER_JUMP ||
            _stateMachine.CurrentStateName == StateNames.PLAYER_ATTACK ||
            _stateMachine.CurrentStateName == StateNames.PLAYER_DIE)
        {
            Debug.Log("Cannot take damage in current state: " + _stateMachine.CurrentStateName);
            return;
        }

        _healthHandler.TakeDamage(damage);
        _stateMachine.ChangeState(StateNames.PLAYER_GET_DAMAGE);
    }
}
