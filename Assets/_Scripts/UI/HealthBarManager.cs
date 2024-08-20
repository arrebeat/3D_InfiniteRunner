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

public class HealthBarManager : MonoBehaviour
{
    
    public Image[] images;
    public PlayerControllerCowboy player { get; private set; }

    private void Awake()
    {
        GameObject containerHealth = GameObject.Find("Container Health");
        images = containerHealth.GetComponentsInChildren<Image>(true);

        GameObject playerObject = GameObject.Find("Cowboy");
        player = playerObject.GetComponent<PlayerControllerCowboy>();
    }

    void Start()
    {
        //HealthCounterCheck();
    }

    void Update()
    {
        
    }

    public void HealthCounterCheck()
    {
        int maxHealth = player.maxHealth;
        
        for (int i = 1; i < images.Length; i++)
        {
            images[i].enabled = i <= player.currentHealth;
        }
    }
}
