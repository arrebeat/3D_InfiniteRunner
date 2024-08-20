using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Dreamteck.Splines;
using MoreMountains.Feedbacks;

public class SwitchController : MonoBehaviour
{
    public PlayerController player { get; private set; }
    public MMF_Player feedbacks { get; private set; }
    public MMF_Player target;

    public bool IsHorizontal;
    public bool IsVertical;
    public bool IsPlayerActivated;
    public bool IsTeleportActivated;
    public bool IsPressed { get; private set; }

    private void Awake()
    {
        GameObject playerObject = GameObject.Find("Teletustra");
        player = playerObject.GetComponent<PlayerController>();
        feedbacks = GetComponent<MMF_Player>();
    }


    void Update()
    {
        // Teleport Activated
        if (IsTeleportActivated && player != null && player.HasTeleported)
        {
            Activate();
        }
    }

    private void Activate()
    {
        target.PlayFeedbacks();
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.CompareTag("Player") && !IsPressed)
        {
            ContactPoint2D contact = other.contacts[0];
            
            // Floor Switch
            if (IsHorizontal && contact.normal.y < -0.5f)
            {
                feedbacks.PlayFeedbacks();
                IsPressed = true;
                feedbacks.CanPlay = false;
            
                if (target != null)
                {
                    Activate();
                }
            }

            // Wall Switch
            if (IsVertical && player.IsRunning)
            {
                if (contact.normal.x > 0.5f)
                {
                    MMF_Position pressRight = feedbacks.GetFeedbackOfType<MMF_Position>(searchedLabel: "Press Right");
                    pressRight.Play(transform.position, 1);
                    IsPressed = true;
                    feedbacks.CanPlay = false;
            
                    if (target != null)
                    {
                        Activate();
                    }
                }

                if (contact.normal.x < -0.5f)
                {
                    MMF_Position pressLeft = feedbacks.GetFeedbackOfType<MMF_Position>(searchedLabel: "Press Left");
                    pressLeft.Play(transform.position, 1);
                    IsPressed = true;
                    feedbacks.CanPlay = false;
            
                    if (target != null)
                    {
                        Activate();
                    }
                }
            }

            // Player Activated
            if (IsPlayerActivated && contact.normal.y < -0.5f && !IsPressed)
            {
                feedbacks.PlayFeedbacks();
                IsPressed = true;
                //feedbacks.CanPlay = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && IsPressed)
        {
            if (IsPlayerActivated && IsPressed)
            {
                IsPressed = false;
            }
        }
    } 
}