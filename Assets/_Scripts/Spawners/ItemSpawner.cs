using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.Feedbacks;
using System;
using UnityEngine.ParticleSystemJobs;

public class ItemSpawner : MonoBehaviour, ISpawn
{
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private bool active;
    [SerializeField] public bool canSpawn;
    [SerializeField] public Item[] spawnSlot = new Item[1];

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn)
        {
            
        }

        if (active)
        {
            SpawnSlotCheck();
        }
    }

    public void Spawn()
    {

        GameObject itemToSpawn = itemPrefabs[0];
        
        if (spawnSlot[0] == null)
        {
            Instantiate(itemToSpawn, transform.position, Quaternion.identity);
            //spawnSlot[0].animator.SetInteger("BountyMultiplier", spawnSlot[0].bountyMultiplier);
            //spawnSlot[0].MultiplierTextUpdate();        
        }
        else
        {
            if (spawnSlot[0].bountyMultiplier < 6)
            {
                spawnSlot[0].bountyMultiplier += 1;
            }
            
            spawnSlot[0].timer = spawnSlot[0].lifetime;
            spawnSlot[0].animator.SetInteger("BountyMultiplier", spawnSlot[0].bountyMultiplier);
            spawnSlot[0].MultiplierTextUpdate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            spawnSlot[0] = other.GetComponent<Item>();
            canSpawn = false;
            
            /*
            if (spawnSlot.Count > 5)
            {
                canSpawn = false;
            }
            */
        }
    }

    private void SpawnSlotCheck()
    {
        if (spawnSlot[0] != null)
        {
            canSpawn = false;
            Item itemToRemove = spawnSlot[0];

            if (itemToRemove == null)
            {
                spawnSlot[0] = null;
            }

            /*
            if (spawnSlot.Count > 5)
            {
                canSpawn = false;
            }
            */
        }
        else
        {
            canSpawn = true;
        }
    }

    
}
