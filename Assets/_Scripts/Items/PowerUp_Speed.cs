using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp_Speed : PowerUp_Base
{
    public float speedMultiplier;
    
    protected override void PowerUpStart()
    {
        base.PowerUpStart();
        player.PowerUpSpeedStart(duration, speedMultiplier);
    }
}
