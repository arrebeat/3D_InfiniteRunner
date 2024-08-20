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

public class EndMenu : MonoBehaviour
{
    public GameObject pointerObject { get; private set;}

    public TextMeshProUGUI[] menuOptionTexts;
    public TextMeshProUGUI restartText { get; private set; }
    public TextMeshProUGUI quitText { get; private set; }

    private PlayerControls playerControls;
    public MMF_Player feedbacks { get; private set;}

    public enum PointerPos
    {
        Restart,
        Quit
    }
    public PointerPos currentPointerPos;
    
    [Range(0, 1)]
    public int pointerPosIndex;

    public MMSceneRestarter sceneManager;
    
    private void Awake()
    {
        pointerObject = GameObject.Find("pointer");
        
        playerControls = new PlayerControls();

        playerControls.Interface.Up.started += MoveUp_Started;
        playerControls.Interface.Up.performed += MoveUp_Performed;
        playerControls.Interface.Up.canceled += MoveUp_Canceled;

        playerControls.Interface.Down.started += MoveDown_Started;
        playerControls.Interface.Down.performed += MoveDown_Performed;
        playerControls.Interface.Down.canceled += MoveDown_Canceled;

        playerControls.Interface.Button1.started += Confirm_Started;
        playerControls.Interface.Button1.performed += Confirm_Performed;
        playerControls.Interface.Button1.canceled += Confirm_Canceled;

        menuOptionTexts = GetComponentsInChildren<TextMeshProUGUI>();
        restartText = menuOptionTexts[0];
        quitText = menuOptionTexts[1];

        GameObject canvas = GameObject.Find("Canvas");
        feedbacks = canvas.GetComponentInChildren<MMF_Player>();

        GameObject sceneManagerObject = GameObject.Find("Scene Manager");
        sceneManager = GetComponentInChildren<MMSceneRestarter>();
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
        pointerPosIndex = 0;
        PointerPosHandler();
    }

    void Update()
    {
        pointerPosIndex = Mathf.Clamp(pointerPosIndex, 0, 1);

        
    }

    #region INPUT CALLBACKS

    private void MoveUp_Started(InputAction.CallbackContext context)
    {
        pointerPosIndex -= 1;
    }

    private void MoveUp_Performed(InputAction.CallbackContext context)
    {
        PointerPosHandler();
    }

    private void MoveUp_Canceled(InputAction.CallbackContext context)
    {

    }

    private void MoveDown_Started(InputAction.CallbackContext context)
    {
        pointerPosIndex += 1;
    }

    private void MoveDown_Performed(InputAction.CallbackContext context)
    {
        PointerPosHandler();
    }

    private void MoveDown_Canceled(InputAction.CallbackContext context)
    {
        
    }

    private void Confirm_Started(InputAction.CallbackContext context)
    {
        
    }

    private void Confirm_Performed(InputAction.CallbackContext context)
    {
        if (currentPointerPos == PointerPos.Restart)
        {
            RestartScene();    
        }
        if (currentPointerPos == PointerPos.Quit)
        {
            Application.Quit();    
        }
    }

    private void Confirm_Canceled(InputAction.CallbackContext context)
    {
        
    }
    
#endregion

    public void PointerPosHandler()
    {
        List<MMF_DestinationTransform> feedbacksDestination = feedbacks.GetFeedbacksOfType<MMF_DestinationTransform>();
        List<MMF_Scale> feedbacksScale = feedbacks.GetFeedbacksOfType<MMF_Scale>();

        if (pointerPosIndex == 0)
        {
            currentPointerPos = PointerPos.Restart;
            feedbacksDestination[0].Play(transform.position, 1);
            feedbacksScale[0].Play(transform.position, 1);
        }
        
        if (pointerPosIndex == 1)
        {
            currentPointerPos = PointerPos.Quit;
            feedbacksDestination[1].Play(transform.position, 1);
            feedbacksScale[1].Play(transform.position, 1);
        }
    }

    public void RestartScene()
    {
        // Get the current active scene's name
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Reload the current scene by loading it again
        SceneManager.LoadScene(currentSceneName);
    }

}
