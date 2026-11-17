using UnityEngine;

public class TargetDummy : MonoBehaviour, IDamageable
{
    public float health = 100f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log(name + " took " + amount + " damage. Health: " + health);

        if (health <= 0f) Destroy(gameObject);
    }
}