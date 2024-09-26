using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ItemBase_Coin : ItemBase
{
    public PlayerController_Ball player;
    public MeshRenderer meshRenderer;
    public ParticleSystem particleSystem_Aura;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        feedbacks = GetComponentInChildren<MMF_Player>();
        particleSystem_Aura = GetComponentInChildren<ParticleSystem>();
        player = GetComponent<PlayerController_Ball>();
    }

    void Start()
    {
        meshRenderer.enabled = false;
        particleSystem_Aura.Stop();
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
        meshRenderer.enabled = false;
        
        List<MMF_ParticlesInstantiation> feedbacksCollected = feedbacks.GetFeedbacksOfType<MMF_ParticlesInstantiation>();
        feedbacksCollected[0].Play(transform.position, 1);
        feedbacksCollected[1].Play(transform.position, 1);
          

        //ItemManager.instance.CollectKey(); 

        Destroy(gameObject, 1f);
    }
}
