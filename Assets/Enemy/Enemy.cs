using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 3;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject); 
    }
}
