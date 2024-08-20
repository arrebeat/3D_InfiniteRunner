using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu]
public class SOPlayerData_Teletustra : ScriptableObject
{
    [Header("Walk")]
    public float walkMaxSpeed;
    public float walkAccelTime;
    public float walkDeccelTime;
    
    [Header("Run")]
    public float runMaxSpeed;
    public float runAccelTime;
    public float runDeccelTime;
    public float doubleTapToRunTime;
    
    [Header("Jump")]
    public float jumpSpeed;
    public float coyoteTime;
    public float jumpInputBufferTime;

    [Header("Teleport")]
    public float teleportChargeTimeFactor;
    public float maxTeleportDistance;

}
