using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class BatController : MonoBehaviour
{
    public bool isMoving;
    public bool isFacingRight;

    public MMF_Player feedbacks { get; private set; }
    public Transform shootPoint; 


    void Awake()
    {
        feedbacks = GetComponentInChildren<MMF_Player>();
    }

    void Start()
    {
        isFacingRight = false;
    }

    void Update()
    {
        
    }

    public void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }

    public void Shoot()
    {
        MMF_ParticlesInstantiation shootLeft = feedbacks.GetFeedbackOfType<MMF_ParticlesInstantiation>(searchedLabel: "shootLeft");
        MMF_ParticlesInstantiation shootRight = feedbacks.GetFeedbackOfType<MMF_ParticlesInstantiation>(searchedLabel: "shootRight");
    
        if (isFacingRight)
        {
            shootRight.Play(shootPoint.position, 1);
        }
        else
        {
            shootLeft.Play(shootPoint.position, 1);
        }

    } 
}
