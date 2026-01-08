using UnityEngine;

/// <summary>
/// Manages the player's item inventory
/// </summary>
public class ItemInventory : MonoBehaviour
{
    [SerializeField] private MonoBehaviour currentItemBehaviour;
    private IItem currentItem;
    
    private void Start()
    {
        if (currentItemBehaviour != null)
        {
            currentItem = currentItemBehaviour as IItem;
        }
    }
    
    /// <summary>
    /// Set the current item in the inventory
    /// </summary>
    public void SetItem(IItem item)
    {
        currentItem = item;
    }
    
    /// <summary>
    /// Use the current item
    /// </summary>
    public void UseItem()
    {
        if (currentItem != null && currentItem.CanUse())
        {
            currentItem.Use(gameObject);
        }
    }
    
    /// <summary>
    /// Get the current item
    /// </summary>
    public IItem GetCurrentItem()
    {
        return currentItem;
    }
    
    /// <summary>
    /// Clear the current item
    /// </summary>
    public void ClearItem()
    {
        currentItem = null;
    }
}
