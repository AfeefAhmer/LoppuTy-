
using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public int MaxHealth = 50;
    public int CurrentHealth = 50;

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

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}
