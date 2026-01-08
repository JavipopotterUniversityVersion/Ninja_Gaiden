using UnityEngine;
using System;

/// <summary>
/// Component for items that can be picked up from the ground
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [SerializeField] private MonoBehaviour itemBehaviour;
    private IItem item;
    
    private void Start()
    {
        if (itemBehaviour != null)
        {
            item = itemBehaviour as IItem;
            if (item == null)
            {
                Debug.LogError($"ItemPickup on {gameObject.name}: itemBehaviour does not implement IItem interface!");
            }
        }
        else
        {
            Debug.LogWarning($"ItemPickup on {gameObject.name}: No item behaviour assigned!");
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ItemInventory inventory = collision.GetComponent<ItemInventory>();
            if (inventory != null && item != null && itemBehaviour != null)
            {
                // Validate item type before adding
                Type itemType = itemBehaviour.GetType();
                
                // Check if the component type is valid and implements IItem
                if (!typeof(IItem).IsAssignableFrom(itemType))
                {
                    Debug.LogError($"Cannot add item: {itemType.Name} does not implement IItem!");
                    return;
                }
                
                // Check if component is a MonoBehaviour
                if (!typeof(MonoBehaviour).IsAssignableFrom(itemType))
                {
                    Debug.LogError($"Cannot add item: {itemType.Name} is not a MonoBehaviour!");
                    return;
                }
                
                // Add the item component to the player
                MonoBehaviour newItemComponent = collision.gameObject.AddComponent(itemType) as MonoBehaviour;
                
                // Set the item in inventory if successfully added
                if (newItemComponent != null && newItemComponent is IItem newItem)
                {
                    inventory.SetItem(newItem);
                    Debug.Log($"Picked up {newItem.GetItemName()}!");
                }
                else
                {
                    Debug.LogError($"Failed to add item component {itemType.Name} to player!");
                    // Clean up failed component
                    if (newItemComponent != null)
                    {
                        Destroy(newItemComponent);
                    }
                    return;
                }
                
                // Destroy the pickup object
                Destroy(gameObject);
            }
            else if (inventory == null)
            {
                Debug.LogWarning($"Player does not have ItemInventory component!");
            }
        }
    }
}
