
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHealth = 100;
    public int CurrentHealth = 120;

    public event Action OnHealthChange;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    private void Start()
    {
        OnHealthChange?.Invoke();
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(0, CurrentHealth);

        OnHealthChange?.Invoke();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);

        OnHealthChange?.Invoke();
    }

    void Die()
    {
        Debug.Log("Player died!");
        Destroy(gameObject);
    }
}
