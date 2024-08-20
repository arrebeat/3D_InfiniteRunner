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
using UnityEngine.SceneManagement;
using MoreMountains.Tools;

public class PauseManager : MonoBehaviour
{
    public bool isPause;
    public GameObject containerEndObject;
    public EndMenu endMenu;

    private MMTimeManager timeManager;
    
    private PlayerControls playerControls;
    public InputActionAsset playerControlsAsset; 

    private InputActionMap actionMapCowboy;
    
    private void Awake()
    {
        containerEndObject = GameObject.Find("Container End");
        endMenu = containerEndObject.GetComponent<EndMenu>();

        GameObject timeManagerObject = GameObject.Find("Time Manager");
        timeManager = timeManagerObject.GetComponent<MMTimeManager>();

        playerControls = new PlayerControls();
        actionMapCowboy = playerControlsAsset.FindActionMap("Cowboy");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        containerEndObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseHandler()
    {
        endMenu.pointerPosIndex = 0;
        endMenu.PointerPosHandler();
        
        if (!isPause)
        {
            isPause = true;
            timeManager.NormalTimeScale = 0;
            actionMapCowboy.Disable();
            containerEndObject.SetActive(true);
        }    
        else
        {
            isPause = false;
            timeManager.NormalTimeScale = 1;
            actionMapCowboy.Enable();
            containerEndObject.SetActive(false);
        }
    }

    public void EnableActionMap()
    {

    }
}
