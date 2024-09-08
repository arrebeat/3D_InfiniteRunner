using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.Feedbacks;

public class ItemBase : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public MMF_Player feedbacks;

    public string tagPlayer = "Player";
    public string tagBat = "Bat";

    public float timeToDestroy;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        feedbacks = GetComponentInChildren<MMF_Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag(tagPlayer))
        {
            Collected();
        }

        else if (other.transform.CompareTag(tagBat))
        {
            Consumed();
        }
    }

    protected virtual void Collected()
    {
        //Debug.Log("Collected");
        MMF_ParticlesInstantiation collected = feedbacks.GetFeedbackOfType<MMF_ParticlesInstantiation>();
        MMF_AudioSource collectedSfx = feedbacks.GetFeedbackOfType<MMF_AudioSource>();
        collected.Play(transform.position, 1);
        collectedSfx.Play(transform.position, 1);

        spriteRenderer.enabled = false;
        
        ItemManager.instance.CollectOnion();

        Destroy(gameObject, timeToDestroy);
    }

    protected virtual void Consumed()
    {
        //Debug.Log("Consumed");
        MMF_ParticlesInstantiation collected = feedbacks.GetFeedbackOfType<MMF_ParticlesInstantiation>();
        MMF_AudioSource collectedSfx = feedbacks.GetFeedbackOfType<MMF_AudioSource>();
        collected.Play(transform.position, 1);
        collectedSfx.Play(transform.position, 1);

        spriteRenderer.enabled = false;
        
        ItemManager.instance.ConsumeOnion();

        Destroy(gameObject, timeToDestroy);
    }
}
