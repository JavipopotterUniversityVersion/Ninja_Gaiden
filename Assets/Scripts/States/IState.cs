using Unity.VisualScripting;
using UnityEngine;

public class IState : MonoBehaviour
{
    protected StateMachine _brain;
    public void SetBrain(StateMachine brain) => _brain = brain;
    public virtual void Init() { }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void StateUpdate() { }
}
