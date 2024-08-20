using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Dreamteck.Splines;
using MoreMountains.Feedbacks;
using System;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;

public class MainMenu : MonoBehaviour
{
    private VisualElement title;
    private VisualElement buttonsContainer;
    private VisualElement pointer;
    private Button playButton;
    private Button quitButton;
    
    private MMLoadScene sceneLoader;
    private Coroutine _coroutine;

    private PlayerControls playerControls;

    public float titleEnterDelay;
    public float pointerDelay;
    public enum PointerPos
    {
        Play,
        Quit
    }
    public PointerPos currentPointerPos;
    

    private void Awake()
    {
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

        var root = GetComponent<UIDocument>().rootVisualElement;

        title = root.Q<VisualElement>("title");
        buttonsContainer = root.Q<VisualElement>("buttonsContainer");
        pointer = root.Q<VisualElement>("pointer");
        playButton = root.Q<Button>("playButton");

        playButton.RegisterCallback<ClickEvent>(OnPlayButtonClicked);

        sceneLoader = GetComponent<MMLoadScene>();
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
        buttonsContainer.style.visibility = Visibility.Hidden;

        currentPointerPos = PointerPos.Play;
        playButton.AddToClassList("button-mainMenu-selected");

        StartCoroutine(TitleEnter());

//      BGPanel.style.display = DisplayStyle.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator TitleEnter()
    {
        WaitForSeconds waitTitle = new WaitForSeconds(titleEnterDelay);
        WaitForSeconds waitPointer = new WaitForSeconds(pointerDelay);

        yield return waitTitle;        
        title.AddToClassList("title-finalPos");
        
        yield return waitPointer;
        buttonsContainer.style.visibility = Visibility.Visible;
    }

    private void OnPlayButtonClicked(ClickEvent evt)
    {
        sceneLoader.LoadScene();
    }

#region INPUT CALLBACKS

    private void MoveUp_Started(InputAction.CallbackContext context)
    {
        if (pointer.ClassListContains("pointer-quitPos"))
        {
            pointer.RemoveFromClassList("pointer-quitPos");
            playButton.AddToClassList("button-mainMenu-selected");

            /*if (quitButton.ClassListContains("button-mainMenu-selected"))
            {
                quitButton.RemoveFromClassList("button-mainMenu-selected");
            }*/
        }
        else
        {

        }
    }

    private void MoveUp_Performed(InputAction.CallbackContext context)
    {
        if (currentPointerPos == PointerPos.Quit)
        {
            currentPointerPos = PointerPos.Play;
        }
        else
        {

        }
    }

    private void MoveUp_Canceled(InputAction.CallbackContext context)
    {

    }

    private void MoveDown_Started(InputAction.CallbackContext context)
    {
        if (!pointer.ClassListContains("pointer-quitPos"))
        {
            pointer.AddToClassList("pointer-quitPos");
            
            //quitButton.AddToClassList("button-mainMenu-selected");

            if (playButton.ClassListContains("button-mainMenu-selected"))
            {
                playButton.RemoveFromClassList("button-mainMenu-selected");
            }
        }
        else
        {
            
        }
    }

    private void MoveDown_Performed(InputAction.CallbackContext context)
    {
        if (currentPointerPos == PointerPos.Play)
        {
            currentPointerPos = PointerPos.Quit;
        }
        else
        {
            
        }
    }

    private void MoveDown_Canceled(InputAction.CallbackContext context)
    {
        
    }

    private void Confirm_Started(InputAction.CallbackContext context)
    {
        if (currentPointerPos == PointerPos.Play)
        {
            playButton.AddToClassList("button-mainMenu-active");
            sceneLoader.LoadScene();
        }

        if (currentPointerPos == PointerPos.Quit)
        {
            //quitButton.AddToClassList("button-mainMenu-active");
            Application.Quit();
        }
    }

    private void Confirm_Performed(InputAction.CallbackContext context)
    {
        
    }

    private void Confirm_Canceled(InputAction.CallbackContext context)
    {
        if (playButton.ClassListContains("button-mainMenu-active"))
        {
            playButton.RemoveFromClassList("button-mainMenu-active");
        }
    }
    
#endregion
}
