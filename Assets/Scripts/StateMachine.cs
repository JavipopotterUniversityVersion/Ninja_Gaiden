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
    public void PlayAnimation(string animationName) => _animator.Play(animationName);
}

public static class StateNames
{
    public const string PLAYER_IDLE = "PLAYER_IDLE";
    public const string PLAYER_WALK = "PLAYER_WALK";
    public const string PLAYER_RUN = "PLAYER_RUN";
    public static string PLAYER_JUMP
    {
        get
        {
            int index = Random.Range(0, 1);
            return $"PLAYER_JUMP_{index}";
        }
    }
}