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

public class BadgeShaker : MonoBehaviour
{
    public MMF_Player feedbacks;
    
    private void Awake()
    {
    
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BadgeShake()
    {
        feedbacks.GetFeedbackOfType<MMF_PositionShake>().Play(transform.position, 1);
    }
}
