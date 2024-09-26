using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Dreamteck.Splines;
using MoreMountains.Feedbacks;
using System;
using UnityEngine.UI;
using TMPro;
//using TMPro.Examples;

public class ProgressManager : MonoBehaviour
{
    public TextMeshPro waveText { get; private set; }
    public SpawnerManager spawnerManager { get; private set; }
    public int currentWave;

    private void Awake()
    {
        GameObject containerWaveObject = GameObject.FindGameObjectWithTag("ContainerWave");
        waveText = containerWaveObject.GetComponentInChildren<TextMeshPro>();
        
        GameObject spawnerManagerObject = GameObject.Find("Spawner Manager");
        spawnerManager = spawnerManagerObject.GetComponent<SpawnerManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        waveText.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WaveTextUpdate()
    {
        currentWave = spawnerManager.currentSequenceIndex + 1;
        string formattedWaveText = $"Wave {currentWave}";
        
        waveText.text = formattedWaveText;
    }
}
