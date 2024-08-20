using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class ItemBase_Key : ItemBase
{
    void Start()
    {
        StartFeedbacks();
    }

    private void StartFeedbacks()
    {
        MMF_Scale pulse = feedbacks.GetFeedbackOfType<MMF_Scale>();
        pulse.Play(transform.position, 1);

    }

    protected override void Collected()
    {
        MMF_ParticlesInstantiation collected = feedbacks.GetFeedbackOfType<MMF_ParticlesInstantiation>();
        spriteRenderer.enabled = false;
        collected.Play(transform.position, 1);

        ItemManager.instance.CollectKey(); 

        Destroy(gameObject);
    }

    protected override void Consumed()
    {
        MMF_ParticlesInstantiation collected = feedbacks.GetFeedbackOfType<MMF_ParticlesInstantiation>();
        //spriteRenderer.enabled = false;
        collected.Play(transform.position, 1);

        //Destroy(gameObject);
    }
}
