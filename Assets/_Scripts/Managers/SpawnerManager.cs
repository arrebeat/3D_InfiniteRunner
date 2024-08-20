using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains.Feedbacks;
using System;
using Unity.VisualScripting;
using MoreMountains.Tools;

public class SpawnerManager : MonoBehaviour
{

    public GameObject[] enemySpawners = new GameObject[3];
    private EnemySpawner EnemySpawnerLeft;
    private EnemySpawner EnemySpawnerCenter;
    private EnemySpawner EnemySpawnerRight;

    private PlayerControllerCowboy player;

    private ScoreManager scoreManager;
    public ComboManager comboManager;
    public ProgressManager progressManager;
    public MMSequencer mainSequencer;
    public MMSequencer SpawnerSequencer;

    public float startDelay;

    public List<MMSequence> Sequences = new List<MMSequence>();
    public int currentSequenceIndex;
    public MMSequence currentSequence;
    public int nextSequenceIndex;
    public MMSequence nextSequence;

    [Space(5)]
    public bool spawnerLeftReady;
    public bool spawnerCenterReady;
    public bool spawnerRightReady;
    [Space(5)]
    public bool spawnersReady;

    public Coroutine _coroutine;

    private void Awake()
    {
        enemySpawners[0] = GameObject.Find("Enemy Spawner Left");
        enemySpawners[1] = GameObject.Find("Enemy Spawner Center");
        enemySpawners[2] = GameObject.Find("Enemy Spawner Right");                

        GameObject playerObject = GameObject.Find("Cowboy");
        player = playerObject.GetComponent<PlayerControllerCowboy>();

        EnemySpawnerLeft = enemySpawners[0].GetComponent<EnemySpawner>();
        EnemySpawnerCenter = enemySpawners[1].GetComponent<EnemySpawner>();
        EnemySpawnerRight = enemySpawners[2].GetComponent<EnemySpawner>();

        GameObject mainSequencerObject = GameObject.Find("Main Sequencer");
        mainSequencer = mainSequencerObject.GetComponent<MMSequencer>();
        SpawnerSequencer = GetComponentInChildren<MMSequencer>();
        SpawnerSequencer.Sequence = Sequences[0];

        GameObject scoreManagerObject = GameObject.Find("Canvas");
        scoreManager = scoreManagerObject.GetComponent<ScoreManager>();

        GameObject comboManagerObject = GameObject.Find("Combo Manager");
        comboManager = comboManagerObject.GetComponent<ComboManager>();

        progressManager = scoreManagerObject.GetComponent<ProgressManager>();
    } 

    void Start()
    {
        StartCoroutine(PlayMainSequence());
    }

    void Update()
    {
        
    }

    public IEnumerator PlayMainSequence()
    {
        WaitForSeconds waitToStart = new WaitForSeconds(startDelay);

        yield return waitToStart;
        mainSequencer.PlaySequence();
    }

    private void StartSequenceHandlerCoroutine()
    {
        if (_coroutine == null)
        {
            _coroutine = StartCoroutine(SequenceHandler());
        }
    }

    private void StopSequenceHandlerCoroutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public IEnumerator SequenceHandler()
    {
        if (currentSequence == null)
        {
            //Debug.Log("FIRST SEQ");

            currentSequenceIndex = 0;
            currentSequence = Sequences[currentSequenceIndex];
            SpawnerSequencer.Sequence = currentSequence;
            
            progressManager.WaveTextUpdate();

            nextSequenceIndex = currentSequenceIndex + 1;
            nextSequence = Sequences[currentSequenceIndex + 1];
            
        }
        else
        {   
            if (currentSequenceIndex < Sequences.Count)
            {
                //Debug.Log("DEMAIS SEQS");
                
                nextSequenceIndex = currentSequenceIndex + 1;
                currentSequenceIndex = nextSequenceIndex;

                if (currentSequenceIndex < Sequences.Count)
                {
                    progressManager.WaveTextUpdate();
                }

                if (currentSequenceIndex == Sequences.Count - 1)
                {
                    //Debug.Log("LASTZEN SEQZEN");
                }

                if (currentSequenceIndex == Sequences.Count)
                {
                    //Debug.Log("PARÔ SEQS");
                    
                    player.Win();
                    mainSequencer.StopSequence();
                }

                currentSequence = Sequences[currentSequenceIndex];
                SpawnerSequencer.Sequence = currentSequence;
                
                nextSequenceIndex = currentSequenceIndex + 1;
                nextSequence = Sequences[currentSequenceIndex + 1];
            }
        }
        
        yield return null;
    }

    public void SpawnHandler()
    {
        if (!player.isWin)
        {
            EnemySpawnerLeft.SpawnSlotCheck();
            EnemySpawnerCenter.SpawnSlotCheck();
            EnemySpawnerRight.SpawnSlotCheck();

            spawnerLeftReady = EnemySpawnerLeft.canSpawn;
            spawnerCenterReady = EnemySpawnerCenter.canSpawn;
            spawnerRightReady = EnemySpawnerRight.canSpawn;

            if (spawnerLeftReady && spawnerCenterReady && spawnerRightReady)
            {
                spawnersReady = true;
            }
            else
            {
                spawnersReady = false;
            }

            if (spawnersReady && _coroutine == null)
            {
                StartSequenceHandlerCoroutine();

                if (currentSequenceIndex < Sequences.Count)
                {
                    SpawnerSequencer.PlaySequence();
                }
            }
            else
            {
                StopSequenceHandlerCoroutine();
            }
        }
        else
        {
            mainSequencer.StopSequence();
            StopAllCoroutines();
        }    
    }
}
