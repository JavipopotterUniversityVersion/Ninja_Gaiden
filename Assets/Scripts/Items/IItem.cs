using UnityEngine;

/// <summary>
/// Interface for all items in the game
/// </summary>
public interface IItem
{
    /// <summary>
    /// Use the item
    /// </summary>
    /// <param name="user">The GameObject using the item (typically the player)</param>
    void Use(GameObject user);
    
    /// <summary>
    /// Get the name of the item
    /// </summary>
    string GetItemName();
    
    /// <summary>
    /// Check if the item can be used
    /// </summary>
    bool CanUse();
}
