using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp_Hover : PowerUp_Base
{
    public float height;

    protected override void PowerUpStart()
    {
        base.PowerUpStart();
        player.PowerUpHoverStart(duration, height);
    }
}
