using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBase : MonoBehaviour
{
    private PlayerController player;
    public int MaxHealth;
    public int CurrentHealth;


    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        CurrentHealth = MaxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        if (player.HasDied) return;

        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            player.Die();
        }
    }

    protected virtual void DetectDamage()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Damage")) 
        {
            //Debug.Log("DAMAGE DETECTED");
        }
    }
}
