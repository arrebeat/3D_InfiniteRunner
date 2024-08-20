using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBaseEnemy : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;
    public SpriteRenderer[] renderers;

    private PlayerController player;
    private bool _isDead;

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        CurrentHealth = MaxHealth;
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
    }
    
    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        _isDead = true;

        //GameObject enemyObject = GetComponentInParent<GameObject>();

        Destroy(this.gameObject);
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
