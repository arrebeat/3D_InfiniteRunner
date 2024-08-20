using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    
    public int damage;
    public float timeToDestroy;

    private Coroutine _coroutine;

    void Awake()
    {
        
    }

    void Start()
    {
        SelfDestroy();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("ENEMY DETECTED");

            var health = other.gameObject.GetComponent<HealthBaseEnemy>();
            
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            //Destroy(this.gameObject);
        }    
    }

    public void SelfDestroy()
    {
        Destroy(this.gameObject, timeToDestroy);
    } 
}
