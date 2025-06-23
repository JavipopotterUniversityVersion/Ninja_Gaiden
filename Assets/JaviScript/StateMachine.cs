using UnityEngine;
using System.Linq;

public class StateMachine : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] SerializableDictionary<string, IState> _states;
    IState _currentState;

    private void Awake()
    {
        _currentState = _states[_states.Keys.First()];
        foreach (var state in _states.Values)
        {
            state.SetBrain(this);
            state.Init();
        }
    }

    private void Update()
    {
        if (_currentState != null) _currentState.StateUpdate();
        else Debug.LogWarning("Current state is null. Please set a valid state.");
    }

    public void ChangeState(string stateName)
    {
        _currentState.Exit();
        _currentState = _states[stateName];
        _currentState.Enter();
    }

    public Animator GetAnimator() => _animator;
}
