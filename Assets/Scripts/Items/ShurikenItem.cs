using UnityEngine;

/// <summary>
/// Item that throws a shuriken projectile in the direction the player is facing
/// </summary>
public class ShurikenItem : MonoBehaviour, IItem
{
    [SerializeField] private GameObject shurikenPrefab;
    [SerializeField] private float cooldown = 0.5f;
    private float lastUseTime = -999f;
    private static Sprite cachedCircleSprite;
    
    public string GetItemName()
    {
        return "Shuriken";
    }
    
    public bool CanUse()
    {
        return Time.time >= lastUseTime + cooldown;
    }
    
    public void Use(GameObject user)
    {
        if (!CanUse()) return;
        
        lastUseTime = Time.time;
        
        // Determine throw direction based on player facing
        Vector2 throwDirection = GetThrowDirection(user);
        
        // Get spawn position (slightly in front of player)
        Vector3 spawnPosition = user.transform.position + new Vector3(throwDirection.x * 0.5f, 0, 0);
        
        // Create shuriken
        GameObject shuriken = CreateShuriken(spawnPosition);
        
        if (shuriken != null)
        {
            ShurikenProjectile projectile = shuriken.GetComponent<ShurikenProjectile>();
            if (projectile != null)
            {
                projectile.SetDirection(throwDirection);
            }
        }
    }
    
    private Vector2 GetThrowDirection(GameObject user)
    {
        // Check sprite renderer flip
        SpriteRenderer sr = user.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            return sr.flipX ? Vector2.left : Vector2.right;
        }
        
        // Check transform scale as fallback
        return user.transform.localScale.x < 0 ? Vector2.left : Vector2.right;
    }
    
    private GameObject CreateShuriken(Vector3 position)
    {
        if (shurikenPrefab != null)
        {
            return Instantiate(shurikenPrefab, position, Quaternion.identity);
        }
        else
        {
            // Create a basic shuriken if no prefab is assigned
            GameObject shuriken = new GameObject("Shuriken");
            shuriken.transform.position = position;
            
            // Add visual (simple circle for now)
            SpriteRenderer sr = shuriken.AddComponent<SpriteRenderer>();
            sr.sprite = GetCachedCircleSprite();
            sr.color = Color.gray;
            
            // Add collider
            CircleCollider2D collider = shuriken.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.1f;
            
            // Add projectile component
            shuriken.AddComponent<ShurikenProjectile>();
            
            return shuriken;
        }
    }
    
    private Sprite GetCachedCircleSprite()
    {
        // Use cached sprite to avoid creating multiple textures
        if (cachedCircleSprite == null)
        {
            cachedCircleSprite = CreateCircleSprite();
        }
        return cachedCircleSprite;
    }
    
    private Sprite CreateCircleSprite()
    {
        // Create a simple circle texture for the shuriken
        Texture2D texture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(16, 16));
                pixels[y * 32 + x] = distance < 12 ? Color.white : Color.clear;
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }
}
