using UnityEngine;

/// <summary>
/// State for using items - triggers item use and returns to idle
/// </summary>
public class UseItemState : IState
{
    [SerializeField] private ItemInventory itemInventory;
    
    public override void Enter()
    {
        if (itemInventory != null)
        {
            itemInventory.UseItem();
        }
        
        // Immediately return to idle after using item
        _brain.ChangeState(StateNames.PLAYER_IDLE);
    }
    
    public override void ReceiveInput(string inputName) { }
}
