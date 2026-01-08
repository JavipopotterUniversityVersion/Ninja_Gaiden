using UnityEngine;
using System.Collections;

/// <summary>
/// Item that allows the player to possess and control an enemy
/// When possessed, the player controls the enemy with movement only
/// If the enemy dies, the player returns to their original body
/// </summary>
public class BodyPossessionItem : MonoBehaviour, IItem
{
    private bool isPossessing = false;
    private GameObject originalBody;
    private GameObject possessedEnemy;
    private StateMachine originalStateMachine;
    private StateMachine possessedStateMachine;
    private float possessionRange = 3f;
    
    public string GetItemName()
    {
        return "Body Possession";
    }
    
    public bool CanUse()
    {
        return !isPossessing;
    }
    
    public void Use(GameObject user)
    {
        if (!CanUse()) return;
        
        // Find nearest enemy
        GameObject nearestEnemy = FindNearestEnemy(user.transform.position);
        
        if (nearestEnemy != null)
        {
            StartCoroutine(PossessEnemy(user, nearestEnemy));
        }
        else
        {
            Debug.Log("No enemy nearby to possess!");
        }
    }
    
    private GameObject FindNearestEnemy(Vector3 position)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float minDistance = possessionRange;
        
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < minDistance)
            {
                // Check if enemy is alive
                HealthHandler health = enemy.GetComponent<HealthHandler>();
                if (health != null && health.CurrentHealth > 0)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }
        }
        
        return nearest;
    }
    
    private IEnumerator PossessEnemy(GameObject player, GameObject enemy)
    {
        isPossessing = true;
        originalBody = player;
        possessedEnemy = enemy;
        
        // Store original state machine
        originalStateMachine = player.GetComponent<StateMachine>();
        
        // Disable player control
        if (originalStateMachine != null)
        {
            originalStateMachine.enabled = false;
        }
        
        // Make player body inactive but not destroyed
        player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        player.GetComponent<MovementController>().enabled = false;
        
        // Set up enemy for player control
        possessedStateMachine = enemy.GetComponent<StateMachine>();
        if (possessedStateMachine == null)
        {
            // If enemy doesn't have StateMachine, add movement control
            MovementController enemyMovement = enemy.GetComponent<MovementController>();
            if (enemyMovement == null)
            {
                enemyMovement = enemy.AddComponent<MovementController>();
            }
        }
        
        // Subscribe to enemy death
        HealthHandler enemyHealth = enemy.GetComponent<HealthHandler>();
        if (enemyHealth != null)
        {
            enemyHealth.onDeath.AddListener(OnPossessedEnemyDeath);
        }
        
        // Transfer control to enemy
        ItemInventory inventory = enemy.GetComponent<ItemInventory>();
        if (inventory == null)
        {
            inventory = enemy.AddComponent<ItemInventory>();
        }
        
        // Wait for possession to complete
        yield return null;
        
        Debug.Log($"Possessing {enemy.name}!");
    }
    
    private void OnPossessedEnemyDeath()
    {
        if (isPossessing && originalBody != null)
        {
            ReturnToOriginalBody();
        }
    }
    
    private void ReturnToOriginalBody()
    {
        Debug.Log("Returning to original body!");
        
        // Re-enable original player control
        if (originalStateMachine != null)
        {
            originalStateMachine.enabled = true;
        }
        
        MovementController originalMovement = originalBody.GetComponent<MovementController>();
        if (originalMovement != null)
        {
            originalMovement.enabled = true;
        }
        
        // Clean up
        isPossessing = false;
        possessedEnemy = null;
    }
    
    private void Update()
    {
        // Handle possessed enemy movement
        if (isPossessing && possessedEnemy != null)
        {
            HandlePossessedMovement();
        }
    }
    
    private void HandlePossessedMovement()
    {
        MovementController movement = possessedEnemy.GetComponent<MovementController>();
        if (movement == null) return;
        
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Add joystick support if available
        Joystick joystick = FindFirstObjectByType<Joystick>();
        if (joystick != null)
        {
            horizontal += joystick.Horizontal;
            vertical += joystick.Vertical;
        }
        
        Vector2 direction = new Vector2(horizontal, vertical);
        
        if (direction.sqrMagnitude > 0.01f)
        {
            movement.Move(direction);
            
            // Flip sprite based on direction
            SpriteRenderer sr = possessedEnemy.GetComponent<SpriteRenderer>();
            if (sr != null && direction.x != 0)
            {
                sr.flipX = direction.x < 0;
            }
        }
        else
        {
            movement.Stop();
        }
    }
}
