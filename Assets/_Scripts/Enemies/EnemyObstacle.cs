using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class EnemyObstacle : MonoBehaviour
{
    public MeshRenderer meshRenderer { get; private set; }
    public MMF_Player feedbacks { get; private set; }
    public PlayerController_Ball player { get; private set; }

    public string tagPlayer = "Player";


    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        feedbacks = GetComponentInChildren<MMF_Player>();
        player = GameObject.Find("Player").GetComponent<PlayerController_Ball>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag(tagPlayer))
        {
            player.Hit();
        }
    }
}
