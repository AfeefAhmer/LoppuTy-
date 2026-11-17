using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHealth = 100;
    public int CurrentHealth = 100;

    public event Action OnHealthChange;

    private void Start()
    {
        CurrentHealth = MaxHealth;
        OnHealthChange?.Invoke();
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }

        OnHealthChange?.Invoke();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;

        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        OnHealthChange?.Invoke();
    }

    void Die()
    {
        Debug.Log("Player died!");
        Destroy(gameObject);
    }
}