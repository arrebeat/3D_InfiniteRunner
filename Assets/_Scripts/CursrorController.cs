using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Dreamteck.Splines;

public class CursrorController : MonoBehaviour
{
    
    public Texture2D cursor;
    public Texture2D cursorClicked;

    public Vector3 screenPosition;
    public Vector3 worldPosition;

    public SplineComputer Spline;
    public GameObject teleOrigin;
    public GameObject teleTarget;
    public PlayerController player;
    public BatController bat;
       
    public float cursorDistance;
    public float TeleportDistance;

    private PlayerControls playerControls;

    public Vector3 _previousCursorPosition;
    public Vector3 _currentCursorPosition;
    public bool _isMovingRight;
    public float toleranceRight;
    public bool _isMovingLeft;
    public float toleranceLeft;


    private void Awake()
    {
        //teleTarget = Spline.GetComponentInChildren<Node>();
        playerControls = new PlayerControls();

        playerControls.InGame.Teleport.started += LeftClick_started;
        playerControls.InGame.Teleport.performed += LeftClick_performed;
        playerControls.InGame.Teleport.canceled += LeftClick_canceled;
        
        ChangeCursor(cursor);
        Cursor.lockState = CursorLockMode.None;
    }
    
    private void OnEnable() 
    {
        playerControls.Enable();   
    }

    private void OnDisable() 
    {
        playerControls.Disable();
    }

    private void Start()
    {
        _previousCursorPosition = worldPosition;
    }

//void OnGUI()
    //{
        //Vector3 point = new Vector3();
        //Event   currentEvent = Event.current;
        //Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        //mousePos.x = currentEvent.mousePosition.x;
        //mousePos.y = Camera.main.pixelHeight - currentEvent.mousePosition.y + 50;

        //point = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));

        //GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        //GUILayout.Label("Screen pixels: " + Camera.main.pixelWidth + ":" + Camera.main.pixelHeight);
        //GUILayout.Label("Mouse position: " + mousePos);
        //GUILayout.Label("World position: " + point.ToString("F3"));
        //GUILayout.EndArea();
    //}

    private void LeftClick_started(InputAction.CallbackContext context)
    {
        ChangeCursor(cursorClicked);
    }

    private void LeftClick_performed(InputAction.CallbackContext context)
    {
        
    }

    private void LeftClick_canceled(InputAction.CallbackContext context)
    {
        ChangeCursor(cursor);
    }

    private void ChangeCursor(Texture2D cursorType)
    {
        Vector2 hotspot = new Vector2(cursorType.width / 2, cursorType.height / 2);
        Cursor.SetCursor(cursorType, hotspot, CursorMode.Auto);
    }
    
    // Update is called once per frame
    void Update()
    {
        screenPosition = Mouse.current.position.ReadValue();
        screenPosition.z = Camera.main.nearClipPlane;

        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPosition.z = Camera.main.nearClipPlane;

        teleTarget.transform.position = worldPosition;

        //Set Max Teleport Distance
        Vector3 TeleportDirection = teleOrigin.transform.position - teleTarget.transform.position;

        cursorDistance = TeleportDirection.magnitude;
        TeleportDistance = Spline.CalculateLength(0, 1);

        if (cursorDistance > player.soPlayerData.maxTeleportDistance)
        {
            teleTarget.transform.position = teleOrigin.transform.position - TeleportDirection.normalized * player.soPlayerData.maxTeleportDistance;
        }

        CheckMoveDirection();       
    }

    public void CheckMoveDirection()
    {
        _currentCursorPosition = worldPosition;

        if (_currentCursorPosition != _previousCursorPosition)
        {
            bat.isMoving = true;

            if (_currentCursorPosition.x > _previousCursorPosition.x + toleranceRight)
            {
                bat.isFacingRight = true;
                _isMovingRight = true;
                _isMovingLeft = false;

                Vector3 scale = bat.transform.localScale;
                scale.x = -1;
                bat.transform.localScale = scale;
            }
            
            if (_currentCursorPosition.x < _previousCursorPosition.x - toleranceLeft)
            {
                bat.isFacingRight = false;
                _isMovingRight = false;
                _isMovingLeft = true;

                Vector3 scale = bat.transform.localScale;
                scale.x = 1;
                bat.transform.localScale = scale;
            }
        }
        else if (_currentCursorPosition == _previousCursorPosition)
        {
            bat.isMoving = false;
            _isMovingRight = false;
            _isMovingLeft = false;
        }

        _previousCursorPosition = _currentCursorPosition;
    }
}
