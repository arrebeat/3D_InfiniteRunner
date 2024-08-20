using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.Feedbacks;
using System;
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private bool active;
    [SerializeField] public bool canSpawn;
    [SerializeField] private float spawnRate;
    [SerializeField] public List<Enemy> spawnSlot = new List<Enemy>();
    
    private Coroutine _coroutine; 

    void Start()
    {
        if (canSpawn)
        {
            //StartSpawnerCoroutine();
        }
    }

    void Update()
    {
        if (canSpawn)
        {
            //StartSpawnerCoroutine();
        }
        else
        {
            //StopSpawnerCoroutine();
        }

        if (active)
        {
            //SpawnSlotCheck();
        }
    }

    private void StartSpawnerCoroutine()
    {
        if (_coroutine == null)
        {
            _coroutine = StartCoroutine(Spawner());
        }
    }

    private void StopSpawnerCoroutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator Spawner()
    {
        WaitForSeconds wait = new WaitForSeconds(UnityEngine.Random.Range(1, spawnRate));

        while (canSpawn)
        {
            yield return wait;

            int rand = UnityEngine.Random.Range(0, enemyPrefabs.Length);
            GameObject enemyToSpawn = enemyPrefabs[rand];

            Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
            canSpawn = false;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        //Debug.Log(other.gameObject.name);

        if (other.gameObject.CompareTag("Enemy"))
        {
            spawnSlot.Add(other.GetComponent<Enemy>());
            canSpawn = false;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        //Debug.Log(other.gameObject.name);

        if (other.gameObject.CompareTag("Enemy"))
        {
            //spawnSlot[0] = null;
            //canSpawn = true;
            
        }
    }
    
    public void SpawnSlotCheck()
    {
        if (spawnSlot.Count > 0)
        {
            canSpawn = false;

            Enemy enemyToRemove = spawnSlot[0];

            if (enemyToRemove == null || enemyToRemove.hp <= 0)
            {
                spawnSlot.RemoveAt(0);
            }
        }
        else
        {
            canSpawn = true;
        }
    }

    public void Spawn(int enemyIndex)
    {
        if (enemyIndex < 0)
        {
            int rand = UnityEngine.Random.Range(0, enemyPrefabs.Length);
            GameObject enemyToSpawn = enemyPrefabs[rand];

            Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
        }
        else
        {
            GameObject enemyToSpawn = enemyPrefabs[enemyIndex];

            Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
            canSpawn = false;
        }        
    }

    public void SpawnRandom()
    {
        int rand = UnityEngine.Random.Range(0, enemyPrefabs.Length);
        GameObject enemyToSpawn = enemyPrefabs[rand];

        Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
        canSpawn = false;
    }

    public void SpawnOutlaw()
    {
        GameObject enemyToSpawn = enemyPrefabs[0];

        Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
        canSpawn = false;
    }

    public void SpawnSamurai()
    {
        GameObject enemyToSpawn = enemyPrefabs[1];

        Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
        canSpawn = false;
    }

    public void SpawnBarret()
    {
        GameObject enemyToSpawn = enemyPrefabs[2];

        Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
        canSpawn = false;
    }

    public void SpawnShorty()
    {
        GameObject enemyToSpawn = enemyPrefabs[3];

        Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
        canSpawn = false;
    }
}
