using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains.Feedbacks;
using System;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour, IDamageable, ITargeted, IShoot, IInvincible
{   
    public StateMachine stateMachine { get; private set; }
    public SpriteRenderer[] renderers { get; private set; }
    public GameObject playerObject { get; private set; }
    public PlayerControllerCowboy player { get; private set; }
    public GunRaycast[] playerGuns { get; private set; }
    private GunRaycast LGun;
    private GunRaycast RGun;
    private bool isTargeted;
    public Animator animator { get; private set; }
    public MMF_Player Feedbacks { get; private set; }
    public MMSequencer Sequencer { get; private set; }
    public MMSequence[] sequences;

    public enum SpawnZone
    {
        Left,
        Center,
        Right    
    }
    public SpawnZone currentSpawnZone;

    public GameObject[] itemSpawners = new GameObject[2];
    private ItemSpawner LeftItemSpawner;
    private ItemSpawner RightItemSpawner;

    private ScoreManager scoreManager;
    public ComboManager comboManager;

    [SerializeField] public bool isHitboxEnabled;

    private float damageMultiplier;
    public float hp;
    public float bounty;
    public float speed;
    public bool isInvincible;
    public bool isArmor;
    public bool isHit;
    public bool isDead = false;
    public int itemDropChance;

    [SerializeField] private GameObject[] projectilePrefabs;
    [SerializeField] public GameObject shootPosition;
    [SerializeField] public bool canShoot;
    [SerializeField] public float fireRate;
    [SerializeField] public float shootCooldown;
    [SerializeField] private Color initialColor = Color.white;
    [SerializeField] public Color targetedColor = Color.red;
    [SerializeField] GameObject[] itemPrefabs;
    
    public UnityEvent OnEnter;
    public UnityEvent OnIdle;
    public UnityEvent OnAttackStartup;
    public UnityEvent OnAttackActive;
    public UnityEvent OnAttackRecovery;
    public UnityEvent OnHit;
    public UnityEvent OnDeath;
    
    private Coroutine _coroutine;
    
   private void Awake()
   {
        BoxCollider collider = GetComponent<BoxCollider>();
        stateMachine = GetComponentInChildren<StateMachine>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.SetFloat("speedFactor", speed);
        Feedbacks = GetComponentInChildren<MMF_Player>();
        Sequencer = GetComponentInChildren<MMSequencer>();
        
        playerObject = GameObject.Find("Cowboy");
        player = playerObject.GetComponent<PlayerControllerCowboy>();
        
        playerGuns = playerObject.GetComponentsInChildren<GunRaycast>();
        LGun = playerGuns[0].GetComponent<GunRaycast>();
        RGun = playerGuns[1].GetComponent<GunRaycast>();

        //GameObject[] ItemSpawnerObjects = GameObject.FindGameObjectsWithTag("ItemSpawner");
        itemSpawners[0] = GameObject.Find("Item Spawner Left");
        itemSpawners[1] = GameObject.Find("Item Spawner Right");

        LeftItemSpawner = itemSpawners[0].GetComponent<ItemSpawner>();
        RightItemSpawner = itemSpawners[1].GetComponent<ItemSpawner>();

        GameObject scoreManagerObject = GameObject.Find("Canvas");
        scoreManager = scoreManagerObject.GetComponent<ScoreManager>();

        GameObject comboManagerObject = GameObject.Find("Combo Manager");
        comboManager = comboManagerObject.GetComponent<ComboManager>();
   }

    void Start()
    {
        if (canShoot)
        {
            StartSateReadyCoroutine();
        }

        OnEnter?.Invoke();                
    }

    void Update()
    {
        if (canShoot)
        {
            StartSateReadyCoroutine();
        }
        else
        {
            StopStateReadyCoroutine();
        }

        TargetCheck();

        if (player.isDead)
        {
            canShoot = false;

            Sequencer.StopSequence();
            Sequencer.Sequence = sequences[0];
            Sequencer.PlaySequence();
        }

        if (animator.GetBool("isHit"))
        {
            isHit = true;
        }
        else
        {
            isHit = false;
        }
    }

    private void TargetCheck()
    {
        if (LGun.gunTarget[0] == this || RGun.gunTarget[0] == this)
        {
            isTargeted = true;
        }
        else
        {
            isTargeted = false;
        }
        
        if (isTargeted)
        {
            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.material.color = targetedColor;
            }
        }
        else
        {
            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.material.color = initialColor;
            }
        }
    }

    public void Targeted(RaycastHit raycastHit)
    {
        
    }
    
    public IEnumerator Death()
    {
        WaitForSeconds waitDeath = new WaitForSeconds(0.4f);
        
        isDead = true;

        if (comboManager.spiritBarL.BarProgress >= 0.85f)
        {
            ItemDrop();
        }

        Sequencer.StopSequence();
        comboManager.KillScored();
        scoreManager.ChangeScoreGradually(bounty);

        //yield return waitDeath;
        //Destroy(gameObject);

        yield return null;
    }

    public void DeadHit()
    {
        comboManager.DeadHitScored();
    }

    public void Invincible()
    {
        isInvincible = !isInvincible;
        animator.SetBool("isInvincible", isInvincible);
    }

    public void Armor()
    {
        isArmor = !isArmor;
        animator.SetBool("isArmor", isArmor);
    }

    protected virtual void DamageMultiplier(int DamageToMultiply)
    {
        if (isInvincible)
        {
            damageMultiplier = 0;
        }
        if (!isInvincible)
        {
            damageMultiplier = 1;
        }
    }

    public void TakeDamage(int damage)
    {
        MMF_PositionShake positionShake = Feedbacks.GetFeedbackOfType<MMF_PositionShake>();
        MMF_SpriteRenderer colorFeedback = Feedbacks.GetFeedbackOfType<MMF_SpriteRenderer>();
        MMF_Flicker flickerFeedback = Feedbacks.GetFeedbackOfType<MMF_Flicker>();

        List<MMF_Particles> particlesFeedback = Feedbacks.GetFeedbacksOfType<MMF_Particles>();

        if (isInvincible)
        {
            if (currentSpawnZone == SpawnZone.Center)
            {
                particlesFeedback[6].Play(transform.position, 1);
            }
            if (currentSpawnZone == SpawnZone.Left)
            {
                particlesFeedback[7].Play(transform.position, 1);
            }
            if (currentSpawnZone == SpawnZone.Right)
            {
                particlesFeedback[8].Play(transform.position, 1);
            }
        }            


        if (!isInvincible)
        {     
            animator.SetBool("isHit", true);
            /*MMF_PositionShake positionShake = Feedbacks.GetFeedbackOfType<MMF_PositionShake>();
            MMF_SpriteRenderer colorFeedback = Feedbacks.GetFeedbackOfType<MMF_SpriteRenderer>();
            MMF_Flicker flickerFeedback = Feedbacks.GetFeedbackOfType<MMF_Flicker>();

            List<MMF_Particles> particlesFeedback = Feedbacks.GetFeedbacksOfType<MMF_Particles>();
            */
            positionShake.Play(transform.position, 1);
            colorFeedback.Play(transform.position, 1);

            if (!isArmor)
            {
                Sequencer.StopSequence();
            }


            if (hp > 0)
            {
                hp -= damage;
                {
                    if (currentSpawnZone == SpawnZone.Center)
                    {
                        particlesFeedback[0].Play(transform.position, 1);
                    }
                    if (currentSpawnZone == SpawnZone.Left)
                    {
                        particlesFeedback[1].Play(transform.position, 1);
                    }
                    if (currentSpawnZone == SpawnZone.Right)
                    {
                        particlesFeedback[2].Play(transform.position, 1);
                    }
                
                }
            }
                
            if (hp <= 0)
            {
                if (!isDead)
                {
                    StopAllCoroutines();
                    StartCoroutine(Death());
                    flickerFeedback.Play(transform.position, 1);
                }
                else
                {                                 
                    if (currentSpawnZone == SpawnZone.Center)
                    {
                        particlesFeedback[3].Play(transform.position, 1);
                    }
                    if (currentSpawnZone == SpawnZone.Left)
                    {
                        particlesFeedback[4].Play(transform.position, 1);
                    }
                    if (currentSpawnZone == SpawnZone.Right)
                    {
                        particlesFeedback[5].Play(transform.position, 1);
                    }
                }
                
            }        
        }

        
    }

    public bool ProbabilityCheck(int itemProbability)
    {
        float rand = UnityEngine.Random.Range(0, 101);
        if (rand <= itemProbability)
            return true;
        else
            return false;
    }

    public void ItemDrop()
    {
        if (currentSpawnZone == SpawnZone.Left)
        {
            LeftItemSpawner.Spawn();
        }

        if (currentSpawnZone == SpawnZone.Right)
        {
            RightItemSpawner.Spawn();
        }

        if (currentSpawnZone == SpawnZone.Center)
        {
            int rand = UnityEngine.Random.Range(0, itemSpawners.Length);
            ItemSpawner itemSpawnerToSelect = itemSpawners[rand].GetComponent<ItemSpawner>();

            itemSpawnerToSelect.Spawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemySpawnerL"))
        {
            currentSpawnZone = SpawnZone.Left;
        }

        if (other.gameObject.CompareTag("EnemySpawnerC"))
        {
            currentSpawnZone = SpawnZone.Center;
        }

        if (other.gameObject.CompareTag("EnemySpawnerR"))
        {
            currentSpawnZone = SpawnZone.Right;
        }
    }

    public void Shoot()
    {
        int rand = UnityEngine.Random.Range(0, projectilePrefabs.Length);
        GameObject projectileToShoot = projectilePrefabs[rand];

        Instantiate(projectileToShoot, shootPosition.transform.position, Quaternion.identity);
    }

    private void StartSateReadyCoroutine()
    {
        if (_coroutine == null)
        {
            _coroutine = StartCoroutine(StateReady());
        }
    }

    private void StopStateReadyCoroutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator StateReady()
    {
        WaitForSeconds wait = new WaitForSeconds(fireRate);
        WaitForSeconds randWait = new WaitForSeconds(UnityEngine.Random.Range(0, shootCooldown));

        while (canShoot)
        {
            yield return wait;
            Shoot();
            yield return randWait;
        }
    }

    public void StartSequence()
    {
        Sequencer.PlaySequence();
    }    
}
