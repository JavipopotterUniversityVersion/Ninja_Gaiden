# Items System Documentation - Ninja Gaiden

## Overview

This document describes the item system implemented for Ninja Gaiden, including the two special items: **Body Possession** and **Shuriken**.

---

## System Architecture

### Core Components

#### 1. **IItem Interface** (`IItem.cs`)
Defines the contract for all items in the game:
- `Use(GameObject user)` - Execute the item's effect
- `GetItemName()` - Returns the item's name
- `CanUse()` - Checks if the item can currently be used

#### 2. **ItemInventory** (`ItemInventory.cs`)
Manages the player's current item:
- Attached to the player GameObject
- Stores one item at a time
- Provides methods to set, use, and clear items

#### 3. **ItemPickup** (`ItemPickup.cs`)
Component for items that can be picked up from the ground:
- Place on item GameObjects in the scene
- Automatically transfers item to player on collision
- Destroys pickup object after collection

#### 4. **UseItemState** (`UseItemState.cs`)
State machine state for using items:
- Triggers item use through ItemInventory
- Immediately returns to idle state

---

## Items

### 1. Body Possession Item

**Script:** `BodyPossessionItem.cs`

**Description:** Allows the player to possess and control a nearby enemy. When possessed, the player abandons their original body and can control the enemy with movement only. If the possessed enemy dies, the player returns to their original body.

**Features:**
- Finds nearest enemy within range (3 units)
- Transfers control to the enemy
- Player body remains in the world (inactive)
- Only movement controls work on possessed enemy
- Automatic return to player body on enemy death
- Cannot use while already possessing

**Usage:**
1. Get near an enemy (within 3 units)
2. Press the USE_ITEM button
3. Control the enemy with movement keys
4. If enemy dies, you return to your original body

### 2. Shuriken Item

**Script:** `ShurikenItem.cs` + `ShurikenProjectile.cs`

**Description:** Throws a shuriken projectile in a straight line, either left or right depending on which way the player is facing. The projectile can damage enemies.

**Features:**
- Throws in the direction player is facing
- Straight-line projectile motion
- Deals damage to enemies on hit
- Cooldown system (0.5 seconds default)
- Destroys on collision with enemies or walls
- Auto-generates visual if no prefab assigned

**Configuration:**
- **Speed:** 10 units/second (default)
- **Damage:** 10 HP (default)
- **Lifetime:** 5 seconds
- **Cooldown:** 0.5 seconds

---

## Setup Instructions

### Player Setup

1. **Add ItemInventory to Player:**
   ```
   - Select Player GameObject
   - Add Component → ItemInventory
   ```

2. **Configure Input System:**
   Add "USE_ITEM" input to the InputBuffer:
   ```
   - Select InputBuffer GameObject
   - Add new entry to Input Buffer dictionary
   - Key: "USE_ITEM"
   - Duration: 0.2f (recommended)
   ```

3. **Add UseItemState to Player:**
   ```
   - Select Player GameObject
   - Find StateMachine component
   - Add new state to States dictionary
   - Key: "PLAYER_USE_ITEM"
   - Value: Add Component → UseItemState
   - Assign ItemInventory reference in UseItemState
   ```

4. **Configure Input Binding:**
   Create a button or key binding that calls:
   ```csharp
   InputBuffer.Instance.ActivateInput("USE_ITEM");
   ```

### Item Pickup Setup

#### Creating a Body Possession Pickup:

1. Create a new GameObject (e.g., "BodyPossessionPickup")
2. Add a 2D Collider (mark as Trigger)
3. Add Component → ItemPickup
4. Add Component → BodyPossessionItem
5. In ItemPickup, assign BodyPossessionItem to the "Item Behaviour" field
6. Add a sprite or visual representation

#### Creating a Shuriken Pickup:

1. Create a new GameObject (e.g., "ShurikenPickup")
2. Add a 2D Collider (mark as Trigger)
3. Add Component → ItemPickup
4. Add Component → ShurikenItem
5. In ItemPickup, assign ShurikenItem to the "Item Behaviour" field
6. (Optional) Create and assign a shuriken prefab in ShurikenItem
7. Add a sprite or visual representation

### Enemy Setup for Possession

For enemies to be possessable, ensure they have:
- Tag: "Enemy"
- HealthHandler component
- MovementController component (or it will be added automatically)

---

## Input Configuration

### Required Input Mappings

The system requires the "USE_ITEM" input to be configured:

**Option 1: Unity Input Manager**
Add to Project Settings → Input:
```
Name: USE_ITEM
Positive Button: e (or your preferred key)
```

**Option 2: Mobile Button**
For mobile controls, create a button that calls:
```csharp
InputBuffer.Instance.ActivateInput("USE_ITEM");
```

### Example Mobile Button Setup:
1. Create UI Button
2. Add EnhancedTouchScreenButtonInput component
3. Set Input Name: "USE_ITEM"
4. Configure button appearance

---

## State Machine Integration

The system integrates with the existing state machine:

**States that can transition to USE_ITEM:**
- PLAYER_IDLE
- PLAYER_WALK

**Flow:**
```
[Any State] → USE_ITEM input → PLAYER_USE_ITEM → Item Used → PLAYER_IDLE
```

---

## Code Examples

### Giving Player an Item Directly

```csharp
// Get player
GameObject player = GameObject.FindGameObjectWithTag("Player");
ItemInventory inventory = player.GetComponent<ItemInventory>();

// Add Body Possession
BodyPossessionItem possession = player.AddComponent<BodyPossessionItem>();
inventory.SetItem(possession);

// Or add Shuriken
ShurikenItem shuriken = player.AddComponent<ShurikenItem>();
inventory.SetItem(shuriken);
```

### Creating Custom Items

```csharp
using UnityEngine;

public class MyCustomItem : MonoBehaviour, IItem
{
    public string GetItemName() => "My Item";
    
    public bool CanUse() => true;
    
    public void Use(GameObject user)
    {
        // Your item logic here
        Debug.Log("Item used!");
    }
}
```

---

## Technical Details

### Body Possession Implementation

**Possession Process:**
1. Find nearest enemy within range
2. Store reference to player's original body
3. Disable player's StateMachine and MovementController
4. Add movement control to enemy
5. Subscribe to enemy's death event
6. Transfer input handling to possessed enemy

**Return Process:**
1. Triggered by enemy death event
2. Re-enable player's StateMachine and MovementController
3. Clear possession references
4. Player regains control

### Shuriken Projectile

**Movement:**
- Uses Rigidbody2D with zero gravity
- Constant velocity in throw direction
- Direction determined by player's facing (SpriteRenderer.flipX or transform.scale)

**Collision:**
- TriggerEnter2D detection
- Checks for "Enemy" tag
- Applies damage through HealthHandler
- Self-destructs on impact

---

## Troubleshooting

### Item Not Working

**Problem:** Pressing USE_ITEM does nothing
- **Solution:** Check that:
  1. ItemInventory is on the player
  2. Player has an item set (not null)
  3. USE_ITEM input is configured in InputBuffer
  4. UseItemState is in StateMachine's states dictionary

### Body Possession Issues

**Problem:** Can't possess enemy
- **Solution:** Verify:
  1. Enemy has "Enemy" tag
  2. Enemy has HealthHandler with health > 0
  3. Enemy is within 3 units of player
  4. Not already possessing another enemy

**Problem:** Can't return to original body
- **Solution:** 
  1. Check that HealthHandler's onDeath event is properly set up
  2. Ensure original body GameObject wasn't destroyed

### Shuriken Issues

**Problem:** Shuriken not appearing
- **Solution:**
  1. Check that shuriken prefab is assigned (or leave null for auto-generation)
  2. Verify player has SpriteRenderer or proper scale for direction detection

**Problem:** Shuriken not damaging enemies
- **Solution:**
  1. Ensure enemies have HealthHandler component
  2. Check that enemies have "Enemy" tag
  3. Verify ShurikenProjectile has a Collider2D (Trigger)

---

## Performance Considerations

- **Body Possession:** Minimal overhead, only active when possessing
- **Shuriken:** Projectiles auto-destroy after 5 seconds
- **Item Pickups:** Destroyed immediately on collection

---

## Future Enhancements

Possible improvements:
1. Multiple item slots
2. Item durability/uses
3. Item combinations
4. More item types
5. Item UI display
6. Item drop on death
7. Item trading system

---

## Files Reference

### Scripts
- `/Assets/Scripts/Items/IItem.cs` - Item interface
- `/Assets/Scripts/Items/ItemInventory.cs` - Player inventory
- `/Assets/Scripts/Items/ItemPickup.cs` - Pickup component
- `/Assets/Scripts/Items/BodyPossessionItem.cs` - Body possession item
- `/Assets/Scripts/Items/ShurikenItem.cs` - Shuriken throw item
- `/Assets/Scripts/Items/ShurikenProjectile.cs` - Shuriken projectile
- `/Assets/Scripts/States/UseItemState.cs` - Use item state

### Modified Files
- `/Assets/Scripts/StateMachine.cs` - Added PLAYER_USE_ITEM constant
- `/Assets/Scripts/States/IdleState.cs` - Added USE_ITEM input handling
- `/Assets/Scripts/States/MoveState.cs` - Added USE_ITEM input handling

---

## License

This code is part of the Ninja_Gaiden project and follows the same license.

---

## Credits

**Implemented by:** GitHub Copilot Agent  
**For:** JavipopotterUniversityVersion  
**Project:** Ninja Gaiden  
**Date:** January 2026
