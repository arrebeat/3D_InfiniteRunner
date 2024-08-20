using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.Feedbacks;
using System;
using System.Net;
using MoreMountains.FeedbacksForThirdParty;

public class Projectile : MonoBehaviour
{
    
    public Transform target;
    [SerializeField] float duration;
    [SerializeField] int damage;
    [SerializeField] float hitStopDurationHit;
    [SerializeField] float hitStopDurationParry;
    
    private SpriteRenderer[] renderers;
    private MMF_Player feedbacks;
    private MMF_Player feedbacksManager;
    private PlayerControllerCowboy player;

    private void Awake()
    {
        GameObject playerObject = GameObject.Find("Cowboy");
        player = playerObject.GetComponent<PlayerControllerCowboy>();

        GameObject targetObject = GameObject.Find("Target");
        target = targetObject.GetComponent<Transform>();
        
        Vector2 targetDirection = target.position - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, targetDirection);
        
        feedbacks = GetComponent<MMF_Player>();
        feedbacks.DurationMultiplier = duration;
        MMF_Position positionFeedback = feedbacks.GetFeedbackOfType<MMF_Position>();
        positionFeedback.DestinationPositionTransform = target;

        
    }
    
    void Start()
    {
        MMF_Position positionFeedback = feedbacks.GetFeedbackOfType<MMF_Position>();
        positionFeedback.Play(transform.position, 1);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);

        if (other.gameObject.CompareTag("Player"))
        {
            player.TakeDamage(damage);
            ScreenShake(0);
            //HitStop(hitStopDurationHit);
            Die();
        }

        if (other.gameObject.CompareTag("Target"))
        {
            Die();
        }

        if (other.gameObject.CompareTag("Parry"))
        {
            ScreenShake(1);
            //HitStop(hitStopDurationParry);
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void HitStop(float duration)
    {
        GameObject feedbacksManagerObject = GameObject.Find("Feedbacks Manager");
        feedbacksManager = feedbacksManagerObject.GetComponent<MMF_Player>();
        MMF_FreezeFrame freezeFrame = feedbacksManager.GetFeedbackOfType<MMF_FreezeFrame>();
        
        freezeFrame.FreezeFrameDuration = duration;
        freezeFrame.Play(transform.position, 1);
    }

    private void ScreenShake(int feedbackIndex)
    {
        GameObject feedbacksManagerObject = GameObject.Find("Feedbacks Manager");
        feedbacksManager = feedbacksManagerObject.GetComponent<MMF_Player>();
        
        //MMF_CinemachineImpulse screenShakeHit = feedbacksManager.GetFeedbackOfType<MMF_CinemachineImpulse>();
        List<MMF_PositionShake> screenShakes = feedbacksManager.GetFeedbacksOfType<MMF_PositionShake>();

        screenShakes[feedbackIndex].Play(transform.position, 1);
    }
}
