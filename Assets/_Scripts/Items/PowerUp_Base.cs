using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class PowerUp_Base : ItemBase_Coin
{
    private MeshRenderer[] _meshRenderers;

    public PlayerController_Ball player;
    public float duration;

    void Awake()
    {
        GameObject playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<PlayerController_Ball>();

        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        feedbacks = GetComponentInChildren<MMF_Player>();
        
    }

    protected override void Collected()
    {
        foreach (var meshRenderer in _meshRenderers)
        {
            meshRenderer.enabled = false;
        }
                
        MMF_ParticlesInstantiation collected = feedbacks.GetFeedbackOfType<MMF_ParticlesInstantiation>();
        collected.Play(transform.position, 1);

        //ItemManager.instance.CollectKey(); 
        PowerUpStart();

        Destroy(gameObject, timeToDestroy);
    }

    protected virtual void PowerUpStart()
    {
        Debug.Log("POWERUP STARTED");
        Invoke("PowerUpEnd", duration);
    }

    protected virtual void PowerUpEnd()
    {
        Debug.Log("POWERUP ENDED");
    }
}
