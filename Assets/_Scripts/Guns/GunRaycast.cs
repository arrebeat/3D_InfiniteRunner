using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.Feedbacks;
using System;

public class GunRaycast : MonoBehaviour
{
    [SerializeField]
    private Color tintColor = Color.red;
    [SerializeField]
    private float lenght = 10f;
    [SerializeField]
    public List<Enemy> gunTarget = new List<Enemy>();

    private PlayerControllerCowboy player;
    
    private void Awake()
    {
        player = GetComponentInParent<PlayerControllerCowboy>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastSingle();
    }

    private void RaycastSingle()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.right;

        Debug.DrawRay(origin, direction * lenght, Color.red);
        Ray ray = new Ray(origin, direction);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, LayerMask.GetMask("Ignore Raycast")))
        {
            //do nothing;
        }
        
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            if (raycastHit.collider.CompareTag("Enemy"))
            {
                if (gunTarget[0] != null)
                {
                    gunTarget[0] = null;
                }

                gunTarget[0] = raycastHit.collider.GetComponent<Enemy>();
                raycastHit.collider.GetComponent<Enemy>().Targeted(raycastHit);    
            }
            //Debug.Log(raycastHit);
        }
        else
        {
            if (gunTarget[0] != null)
            {
                gunTarget[0] = null;
            }
        }
    }

    private void AddTarget(Enemy enemy)
    {
        gunTarget[0] = enemy;
    }

    private void RemoveTarget(Enemy enemy)
    {
        gunTarget.Remove(enemy);
    }
}
