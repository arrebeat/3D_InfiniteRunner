using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ItemBase_Coin : ItemBase
{
    public MeshRenderer meshRenderer { get; private set; }

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        feedbacks = GetComponentInChildren<MMF_Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other + "COLLECTED");
        if (other.transform.CompareTag(tagPlayer))
        {
            Collected();
        }
    }

    protected override void Collected()
    {
        MMF_ParticlesInstantiation collected = feedbacks.GetFeedbackOfType<MMF_ParticlesInstantiation>();
        meshRenderer.enabled = false;
        collected.Play(transform.position, 1);

        //ItemManager.instance.CollectKey(); 

        Destroy(gameObject, 0.5f);
    }
}
