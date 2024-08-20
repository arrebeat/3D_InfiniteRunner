using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int Damage;

    void Awake()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var health = other.gameObject.GetComponent<HealthBase>();

        if (health != null) 
        {
            health.TakeDamage(Damage);
        }
    }
}
