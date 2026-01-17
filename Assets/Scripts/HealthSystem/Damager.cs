using UnityEngine;

public class Damager : MonoBehaviour
{
    [SerializeField] int damageAmount = 10;
    [SerializeField] LayerMask damageableLayer;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & damageableLayer) != 0)
        {
            if(other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damageAmount);
            }
        }
    }
}
