using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Dreamteck.Splines;
using MoreMountains.Feedbacks;
using TMPro;

public class BarController : MonoBehaviour
{

    public PlayerController player { get; private set; }

    private PlayerControls playerControls;

    public Image fill;

    [Range(0, 1)]
    public float normalizedTestSpeed;

    private void Awake()
    {
        GameObject playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<PlayerController>();
        playerControls = new PlayerControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FillUpdate();
    }

    private void FillUpdate()
    {
        normalizedTestSpeed = Mathf.Lerp(0, 1, player.testSpeed/player.runMaxSpeed);
        fill.fillAmount = normalizedTestSpeed;
    }
}
