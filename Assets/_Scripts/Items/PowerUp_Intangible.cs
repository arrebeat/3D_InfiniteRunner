using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp_Intangible : PowerUp_Base
{
    protected override void PowerUpStart()
    {
        base.PowerUpStart();
        player.PowerUpIntangibleStart(duration);
    }
}
