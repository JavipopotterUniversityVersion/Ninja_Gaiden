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
        InputBuffer.Instance.OnInputActivated.AddListener(ReceiveInput);
        foreach (var state in _states.Values)
        {
            state.SetBrain(this);
            state.Init();
        }
    }

    public void ReceiveMessage(string message) => ReceiveInput(message);
    private void ReceiveInput(string inputName)
    {
        if (_currentState != null) _currentState.ReceiveInput(inputName);
        else Debug.LogWarning("Current state is null. Please set a valid state.");
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
    public const string PLAYER_JUMP = "PLAYER_JUMP";
    public const string PLAYER_RUN = "PLAYER_RUN";
    public const string PLAYER_ATTACK = "PLAYER_ATTACK";
    public const string PLAYER_GET_DAMAGE = "PLAYER_GET_DAMAGE";
}