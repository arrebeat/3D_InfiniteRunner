using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Events;
using Dreamteck.Splines;
using MoreMountains.Feedbacks;
using System;
using Unity.VisualScripting;
using UnityEngine.VFX;
using MoreMountains.Tools;

public class PlayerControllerCowboy : MonoBehaviour, IDamageable, IInvincible
{
    
    //public Rigidbody2D rb { get; private set; } 
    public Animator animator { get; private set; }
    public GunRaycast[] guns { get; private set; }
    private GunRaycast LGun;
    private GunRaycast RGun;

    private BarManager ammoBarManager;
    public int currentAmmo;


    public MMF_Player feedbacksManager;
    public MMF_Player Feedbacks { get; private set; }
    public SpriteRenderer[] Renderers { get; private set; }
    public Material[] materials;

    public VisualEffect[] visualEffects { get; private set; }

    private PlayerControls playerControls;


    [SerializeField] bool isHitboxEnabled = true;
    
    public int maxHealth;
    public int currentHealth;
    public bool isWin = false;
    public bool isDead = false;
    public float initialSpeed;
    public float highSpeed;
    public HealthBarManager healthBarManager { get; private set; }

    public bool leftPressed;
    public bool rightPressed;

    [Range(-3, 3)]
    public int stanceIndex;
    public bool canShoot;
    public float unloadTime;
    public bool isGunFightHigh;
    public Enemy[] targets = new Enemy[2];
    private Enemy LTarget;
    private Enemy RTarget;

    public BoxCollider[] parryColliders;
    private BoxCollider parryColliderLeft;
    private BoxCollider parryColliderCenter;
    private BoxCollider parryColliderRight;

    [SerializeField] public bool isGunSpinning;
    [SerializeField] float reloadTime;
    [SerializeField] public bool isParrying;
    [SerializeField] float parryTime;
    [SerializeField] private Color gunInitialColor = Color.white;
    [SerializeField] private Color gunParryColor;

    public GameObject[] enemySpawners = new GameObject[3];
    private EnemySpawner enemySpawnerLeft;
    private EnemySpawner enemySpawnerCenter;
    private EnemySpawner enemySpawnerRight; 
    
    public GameObject[] itemSpawners = new GameObject[2];
    private ItemSpawner LeftItemSpawner;
    private ItemSpawner RightItemSpawner;

    private Coroutine _reloadCoroutine;

    private MMTimeManager timeManager;
    private PauseManager pauseManager;
    private ScoreManager scoreManager;
    private ComboManager comboManager;
    public GameObject fullSpiritFillObjectL { get; private set; }
    public GameObject fullSpiritFillObjectR { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        GameObject feedbacksManagerObject = GameObject.Find("Feedbacks Manager");
        feedbacksManager = feedbacksManagerObject.GetComponent<MMF_Player>();
        Feedbacks = GetComponent<MMF_Player>();
        Renderers = GetComponentsInChildren<SpriteRenderer>(true);
        visualEffects = GetComponentsInChildren<VisualEffect>();
        playerControls = new PlayerControls();

        GameObject canvas = GameObject.Find("Canvas");
        healthBarManager = canvas.GetComponent<HealthBarManager>();

        GameObject pauseManagerObject = GameObject.Find("Pause Manager");
        pauseManager = pauseManagerObject.GetComponent<PauseManager>();

        guns = GetComponentsInChildren<GunRaycast>();
        LGun = guns[0].GetComponent<GunRaycast>();
        RGun = guns[1].GetComponent<GunRaycast>();

        ammoBarManager = canvas.GetComponentInChildren<BarManager>();
        fullSpiritFillObjectL = GameObject.Find("FillHighL");
        fullSpiritFillObjectR = GameObject.Find("FillHighR");

        parryColliders = GetComponentsInChildren<BoxCollider>();
        parryColliderLeft = parryColliders[0];
        parryColliderCenter = parryColliders[1];
        parryColliderRight = parryColliders[2];

        enemySpawners[0] = GameObject.Find("Enemy Spawner Left");
        enemySpawners[1] = GameObject.Find("Enemy Spawner Center");
        enemySpawners[2] = GameObject.Find("Enemy Spawner Right");
            enemySpawnerLeft = enemySpawners[0].GetComponent<EnemySpawner>();
            enemySpawnerCenter = enemySpawners[1].GetComponent<EnemySpawner>();
            enemySpawnerRight = enemySpawners[2].GetComponent<EnemySpawner>();

        GameObject timeManagerObject = GameObject.Find("Time Manager");
        timeManager = timeManagerObject.GetComponent<MMTimeManager>();
        
        GameObject scoreManagerObject = GameObject.Find("Canvas");
        scoreManager = scoreManagerObject.GetComponent<ScoreManager>();

        GameObject comboManagerObject = GameObject.Find("Combo Manager");
        comboManager = comboManagerObject.GetComponent<ComboManager>();

        itemSpawners[0] = GameObject.Find("Item Spawner Left");
        itemSpawners[1] = GameObject.Find("Item Spawner Right");
            LeftItemSpawner = itemSpawners[0].GetComponent<ItemSpawner>();
            RightItemSpawner = itemSpawners[1].GetComponent<ItemSpawner>();

        playerControls.Cowboy.MoveLeft.started += MoveLeft_Started;
        playerControls.Cowboy.MoveLeft.performed += MoveLeft_Performed;
        playerControls.Cowboy.MoveLeft.canceled += MoveLeft_Canceled;

        playerControls.Cowboy.MoveRight.started += MoveRight_Started;
        playerControls.Cowboy.MoveRight.performed += MoveRight_Performed;
        playerControls.Cowboy.MoveRight.canceled += MoveRight_Canceled;

        playerControls.Cowboy.Shoot.started += Shoot_Started;
        playerControls.Cowboy.Shoot.performed += Shoot_Performed;
        playerControls.Cowboy.Shoot.canceled += Shoot_Canceled;

        playerControls.Cowboy.Reload.started += Reload_Started;
        playerControls.Cowboy.Reload.performed += Reload_Performed;
        playerControls.Cowboy.Reload.canceled += Reload_Canceled;

        playerControls.Cowboy.Pause.started += Pause_Started;
        playerControls.Cowboy.Pause.performed += Pause_Performed;
        playerControls.Cowboy.Pause.canceled += Pause_Canceled;
    }

    private void OnEnable()
    {
        playerControls.Enable();    
    }

    private void OnDisable()
    {
        playerControls.Disable();    
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBarManager.HealthCounterCheck();
        stanceIndex = 0;
        canShoot = true;
        animator.SetFloat("moveSpeed", initialSpeed);
        fullSpiritFillObjectL.SetActive(false);
        fullSpiritFillObjectR.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        stanceIndex = Mathf.Clamp(stanceIndex, -3, 3);
        
        currentAmmo = ammoBarManager.barCurrentValue;
        if (currentAmmo <= 0 || Mathf.Abs(stanceIndex) == 3)
        {
            canShoot = false;
        }
        if (currentAmmo > 0 && Mathf.Abs(stanceIndex) > 3)
        {
            canShoot = true;
        }

        if (!isGunSpinning && _reloadCoroutine != null)
        {
            StopCoroutine(_reloadCoroutine);
            _reloadCoroutine = null;
        }

        TargetCheck();
    }

    private void Move()
    {
        if (leftPressed)
        {
            animator.SetTrigger("leftPress");
        }

        if (rightPressed)
        {
            animator.SetTrigger("rightPress");
        }
    }

    private void MoveSpeed()
    {

    }

    private void StanceChecks()
    {
        //Taking cover
        while (MathF.Abs(stanceIndex) == 3)
        {
            canShoot = false;
            Invincible();
        }
    }

    private void TargetCheck()
    {
        if (LGun.gunTarget[0] != null)
        {
            targets[0] = LGun.gunTarget[0];
        }
        else
        {
            targets[0] = null;
        }

        if (RGun.gunTarget[0] != null)
        {
            targets[1] = RGun.gunTarget[0];
        }
        else
        {
            targets[1] = null;
        }
    }

     private void Shoot()
     {
        if (Mathf.Abs(stanceIndex) < 3)
        {
            List<MMF_ScaleShake> feedbacksGunScale = feedbacksManager.GetFeedbacksOfType<MMF_ScaleShake>();
            List<MMF_Particles> feedbacksGunMuzzleFlash = feedbacksManager.GetFeedbacksOfType<MMF_Particles>();
                
                feedbacksGunScale[0].Duration = 0.05f;
                feedbacksGunScale[1].Duration = 0.05f;
                feedbacksGunScale[0].Play(transform.position, 1);
                feedbacksGunScale[1].Play(transform.position, 1);
                feedbacksGunMuzzleFlash[0].Play(transform.position, 1);
                feedbacksGunMuzzleFlash[1].Play(transform.position, 1);
            
            ammoBarManager.TapToFill(-1, unloadTime);
        }

        if (targets[0] != null)
        {
            LTarget = targets[0].GetComponent<Enemy>();
            LTarget.TakeDamage(1);
        }
        
        if (targets[1] != null)
        {
            RTarget = targets[1].GetComponent<Enemy>();
            RTarget.TakeDamage(1);
        }
     }

     public IEnumerator Reload()
     {
        WaitForSeconds wait = new WaitForSeconds(reloadTime);
        //WaitForSeconds waitDeflect = new WaitForSeconds(spinTime);

        if (Mathf.Abs(stanceIndex) < 3)
        {
            FeedbacksGunReload();
            Renderers[4].enabled = false;
            Renderers[5].enabled = false;
            isGunSpinning = true;
        }

        if (currentAmmo < ammoBarManager.barMaxParameter)
        {
            if (isGunFightHigh)
            {
                ammoBarManager.TapToFill(6, reloadTime);
            }
            else
            {
                ammoBarManager.TapToFill(1, reloadTime);
            }

            canShoot = false;
            yield return wait;
            canShoot = true;
            Renderers[4].enabled = true;
            Renderers[5].enabled = true;
            
            //yield return waitDeflect;
            isGunSpinning = false;
        }
        else
        {
            yield return wait;
            Renderers[4].enabled = true;
            Renderers[5].enabled = true;
            
            //yield return waitDeflect;
            isGunSpinning = false;
        }

        yield return null;
     }

    private void FeedbacksGunReload()
    {
        GameObject feedbacksManagerObject = GameObject.Find("Feedbacks Manager");
        feedbacksManager = feedbacksManagerObject.GetComponent<MMF_Player>();

        List<MMF_Scale> feedbacksReloadScale = feedbacksManager.GetFeedbacksOfType<MMF_Scale>("gunReloadScale");
        List<MMF_Particles> feedbacksReloadSpin = feedbacksManager.GetFeedbacksOfType<MMF_Particles>("gunReloadSpin");

            //feedbacksReloadScale[0].Play(transform.position, 1);
            //feedbacksReloadScale[1].Play(transform.position, 1);
            feedbacksReloadSpin[0].Play(transform.position, 1);
            feedbacksReloadSpin[1].Play(transform.position, 1);
    }

    public void TakeDamage(int Damage)
    {
        currentHealth -= Damage;
        healthBarManager.HealthCounterCheck();

        comboManager.SpiritDeplete();
        scoreManager.combo = 0;
        scoreManager.UpdateComboText();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Death();
        }
    }

    public void Win()
    {
        Debug.Log("WIN!!!");
        isWin = true;

        //StopAllCoroutines();
        StartCoroutine(WinHandler());
    }

    public IEnumerator WinHandler()
    {
        WaitForSeconds wait = new WaitForSeconds(scoreManager.waitScore);

        //timeManager.NormalTimeScale = 0.5f;

        yield return wait;

        //timeManager.NormalTimeScale = 1f;
        OnDisable();
        scoreManager.ScoreTally();

        yield return null;
    }

    public void Death()
    {
        isDead = true;

        StopAllCoroutines();
        OnDisable();

        foreach (var renderer in Renderers)
        {
            renderer.enabled = false;
        }

        scoreManager.ScoreTally();
    }

    public void Pickup()
    {                
        if (stanceIndex == -3 && LeftItemSpawner.spawnSlot[0] != null)
        {
            Item itemToCollect = LeftItemSpawner.spawnSlot[0];
            itemToCollect.TakeDamage(1);
            comboManager.PickupScored();
        }

        if (stanceIndex == 3 && RightItemSpawner.spawnSlot[0] != null)
        {
            Item itemToCollect = RightItemSpawner.spawnSlot[0];
            itemToCollect.TakeDamage(1);
            comboManager.PickupScored();
        }
    }

    private IEnumerator Parry(BoxCollider parryCollider)
    {
        WaitForSeconds wait = new WaitForSeconds(parryTime);

        parryCollider.enabled = true;
        isParrying = true;
        Invincible();
        comboManager.ParryScored();

        if (currentAmmo < ammoBarManager.barMaxParameter)
        {
            if (isGunSpinning)
            {
                canShoot = true;
            }
            else
            {
                ammoBarManager.TapToFill(1, 0.01f);
                canShoot = true;
            }
        }

        yield return wait;
        
        parryCollider.enabled = false;
        isParrying = false;
        Invincible();
    }

    private void FeedbacksGunParry(int gun)
    {
        GameObject feedbacksManagerObject = GameObject.Find("Feedbacks Manager");
        feedbacksManager = feedbacksManagerObject.GetComponent<MMF_Player>();

        List<MMF_SpriteRenderer> feedbacksParryColor = feedbacksManager.GetFeedbacksOfType<MMF_SpriteRenderer>("gunParryColor");
        List<MMF_ScaleShake> feedbacksParryScale = feedbacksManager.GetFeedbacksOfType<MMF_ScaleShake>("gunParryScale");
        List<MMF_ParticlesInstantiation> feedbacksParryFlash = feedbacksManager.GetFeedbacksOfType<MMF_ParticlesInstantiation>("gunParryFlash");

            feedbacksParryColor[gun].Play(transform.position, 1);
            feedbacksParryScale[gun].Duration = 0.15f;
            feedbacksParryScale[gun].Play(transform.position, 1);
            feedbacksParryFlash[gun].Play(transform.position, 1);
    }

    private void Deflect(int gun, int enemySpawner)
    {        
        comboManager.DeflectScored();

        List<MMF_ScaleShake> feedbacksGunScale = feedbacksManager.GetFeedbacksOfType<MMF_ScaleShake>();
            List<MMF_Particles> feedbacksGunDeflect = feedbacksManager.GetFeedbacksOfType<MMF_Particles>("gunDeflect");
                
                //feedbacksGunScale[0].Duration = 0.05f;
                //feedbacksGunScale[1].Duration = 0.05f;
                //feedbacksGunScale[0].Play(transform.position, 1);
                //feedbacksGunScale[1].Play(transform.position, 1);
                feedbacksGunDeflect[gun].Play(transform.position, 1);

        EnemySpawner spawnerToUse = enemySpawners[enemySpawner].GetComponent<EnemySpawner>();
        
        if (spawnerToUse.spawnSlot[0] != null)
        {
            Enemy enemyToDamage = spawnerToUse.spawnSlot[0];
            enemyToDamage.TakeDamage(3);
        }
        else
        {
            return;
        }
    }

    public void Invincible()
    {
        CapsuleCollider hitbox = GetComponentInChildren<CapsuleCollider>();
        isHitboxEnabled = !isHitboxEnabled;
        hitbox.enabled = isHitboxEnabled;
    }

    public void GunFightHigh()
    {
        isGunFightHigh = true;
        animator.SetFloat("moveSpeed", highSpeed);

        Renderers[4].material = materials[1];
        Renderers[5].material = materials[1];
        
        fullSpiritFillObjectL.SetActive(true);
        fullSpiritFillObjectR.SetActive(true); 
    }
    

#region INPUT CALLBACKS

    private void MoveLeft_Started(InputAction.CallbackContext context)
    {
        leftPressed = true;
        animator.SetTrigger("leftPress");
        animator.SetBool("left", true);

        if (stanceIndex == 0)
        {
            
        }

        if (stanceIndex == -2)
            {
                canShoot = false;
                Invincible();
            }
    }

    private void MoveLeft_Performed(InputAction.CallbackContext context)
    {
        if (stanceIndex != -3)
        {
            stanceIndex -= 1;
            animator.SetInteger("stanceIndex", stanceIndex);

            if (stanceIndex == 1)
            {
                Collider[] hitColliders = Physics.OverlapBox(parryColliderCenter.transform.position + parryColliderCenter.center, parryColliderCenter.size/2, parryColliderCenter.transform.rotation, LayerMask.GetMask("Projectile"));

                foreach (Collider hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Projectile"))
                    {
                        animator.Play("Front_Right_Idle", 0, 0);
                        StartCoroutine(Parry(parryColliderCenter));
                        FeedbacksGunParry(0);
                        
                        if (isGunSpinning)
                        {
                            Deflect(0, 1);
                        }
                    }
                }
            }

            if (stanceIndex == 2)
            {
                Renderers[4].enabled = true;
                Renderers[5].enabled = true;
                canShoot = true;
                Invincible();
            }

            if (stanceIndex == -1)
            {
                Collider[] hitColliders = Physics.OverlapBox(parryColliderLeft.transform.position + parryColliderLeft.center, parryColliderLeft.size/2, parryColliderLeft.transform.rotation, LayerMask.GetMask("Projectile"));

                foreach (Collider hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Projectile"))
                    {
                        animator.Play("Front_Left_Idle", 0, 0);
                        StartCoroutine(Parry(parryColliderLeft));
                        FeedbacksGunParry(0);
                        
                        if (isGunSpinning)
                        {
                            Deflect(0, 0);
                        }
                    }
                }
                
                
            }
        }
    }

    private void MoveLeft_Canceled(InputAction.CallbackContext context)
    {
        leftPressed = false;
        animator.ResetTrigger("leftPress");
        animator.SetBool("left", false);
    }

    private void MoveRight_Started(InputAction.CallbackContext context)
    {
        rightPressed = true;
        animator.SetTrigger("rightPress");
        animator.SetBool("right", true);

        if (stanceIndex == 2)
            {
                canShoot = false;
                Invincible();
            }
    }

    private void MoveRight_Performed(InputAction.CallbackContext context)
    {
        if (stanceIndex != 3)
        {
            stanceIndex += 1;
            animator.SetInteger("stanceIndex", stanceIndex);

            if (stanceIndex == 1)
            {
                Collider[] hitColliders = Physics.OverlapBox(parryColliderRight.transform.position + parryColliderRight.center, parryColliderRight.size/2, parryColliderRight.transform.rotation, LayerMask.GetMask("Projectile"));

                foreach (Collider hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Projectile"))
                    {
                        animator.Play("Front_Right_Idle", 0, 0);
                        StartCoroutine(Parry(parryColliderRight));
                        FeedbacksGunParry(1);
                        
                        if (isGunSpinning)
                        {
                            Deflect(1, 2);
                        }
                    }
                };
            }

            if (stanceIndex == -2)
            {
                Renderers[4].enabled = true;
                Renderers[5].enabled = true;
                canShoot = true;
                Invincible();
            }

            if (stanceIndex == -1)
            {
                Collider[] hitColliders = Physics.OverlapBox(parryColliderCenter.transform.position + parryColliderCenter.center, parryColliderCenter.size/2, parryColliderCenter.transform.rotation, LayerMask.GetMask("Projectile"));

                foreach (Collider hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Projectile"))
                    {
                        animator.Play("Front_Left_Idle", 0, 0);
                        StartCoroutine(Parry(parryColliderCenter));
                        FeedbacksGunParry(1);
                        
                        if (isGunSpinning)
                        {
                            Deflect(1, 1);
                        }
                    }
                }
            }
        }
    }

    private void MoveRight_Canceled(InputAction.CallbackContext context)
    {
        rightPressed = false;
        animator.ResetTrigger("rightPress");
        animator.SetBool("right", false);
    }

    private void Shoot_Started(InputAction.CallbackContext context)
    {
        animator.SetTrigger("shoot");
        animator.SetBool("bshoot", true);
        Renderers[4].enabled = true;
        Renderers[5].enabled = true;
    }

    private void Shoot_Performed(InputAction.CallbackContext context)
    {
        if (currentAmmo > 0 && canShoot)
        {
            Shoot();
        }
        
        Pickup();
    }

    private void Shoot_Canceled(InputAction.CallbackContext context)
    {
        animator.ResetTrigger("shoot");
        animator.SetBool("bshoot", false);
    }

    private void Reload_Started(InputAction.CallbackContext context)
    {
        //if (_reloadCoroutine == null)
        {
            _reloadCoroutine = StartCoroutine(Reload());
        }
    }

    private void Reload_Performed(InputAction.CallbackContext context)
    {
        /*if (_reloadCoroutine == StartCoroutine(Reload()))
        {
            StopCoroutine(_reloadCoroutine);
            _reloadCoroutine = null;
            _reloadCoroutine = StartCoroutine(Reload());
        }*/
    }

    private void Reload_Canceled(InputAction.CallbackContext context)
    {
        
    }

    private void Pause_Started(InputAction.CallbackContext context)
    {
        pauseManager.PauseHandler();

        //playerControls.Cowboy.Disable();
    }

    private void Pause_Performed(InputAction.CallbackContext context)
    {
        
    }

    private void Pause_Canceled(InputAction.CallbackContext context)
    {
        
    }
    #endregion


}