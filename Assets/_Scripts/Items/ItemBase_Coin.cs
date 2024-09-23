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
        //meshRenderer.enabled = false;
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
        MMF_ParticlesInstantiation collected = feedbacks.GetFeedbackOfType<MMF_ParticlesInstantiation>();
        meshRenderer.enabled = false;
        collected.Play(transform.position, 1);

        /* MMF_ScaleShake scaleShakePlayer = player.feedbacks.GetFeedbackOfType<MMF_ScaleShake>();
        scaleShakePlayer.Play(transform.position, 1); */    

        //ItemManager.instance.CollectKey(); 

        Destroy(gameObject, 0.5f);
    }
}
