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

public class BadgeController : MonoBehaviour
{
    public Animator[] animators;
    
    public enum BountyRating
    {
        Gold,
        Silver,
        Bronze,
        Copper
    }
    public BountyRating finalBountyRating;
    
    public enum LivesRating
    {
        Gold,
        Silver,
        Bronze,
        Bone
    }
    public LivesRating finalLivesRating;

    public enum WavesRating
    {
        Gold,
        Silver,
        Bronze
    }
    public WavesRating finalWavesRating;

    public enum ComboRating
    {
        Gold,
        Silver,
        Bronze,
        NoCombo
    }
    public ComboRating finalComboRating;

    public MMF_Player feedbacks { get; private set; }
    
    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
        feedbacks = GetComponentInChildren<MMF_Player>();
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
        var badgeShake = feedbacks.GetFeedbackOfType<MMF_PositionShake>();
        badgeShake.Play(transform.position, 1);
    }
}
