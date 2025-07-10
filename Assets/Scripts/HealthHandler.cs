using UnityEngine;
using UnityEngine.Events;

public class HealthHandler : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    public int CurrentHealth
    {
        get => currentHealth;
        private set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            onHealthChanged.Invoke(currentHealth);
        }
    }

    [SerializeField] UnityEvent<int> onHealthChanged;
    [SerializeField] UnityEvent onGetDamage;
    [SerializeField] UnityEvent onDeath;

    private void Start()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        onGetDamage.Invoke();
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        currentHealth = 0;
        onDeath.Invoke();
    }
}
