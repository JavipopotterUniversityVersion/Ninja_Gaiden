using UnityEngine;

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
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ItemInventory inventory = collision.GetComponent<ItemInventory>();
            if (inventory != null && item != null && itemBehaviour != null)
            {
                // Add the item component to the player and transfer it properly
                System.Type itemType = itemBehaviour.GetType();
                MonoBehaviour newItemComponent = collision.gameObject.AddComponent(itemType) as MonoBehaviour;
                
                // Copy any serialized fields if needed
                if (newItemComponent is IItem newItem)
                {
                    inventory.SetItem(newItem);
                }
                
                // Destroy the pickup object
                Destroy(gameObject);
            }
        }
    }
}
