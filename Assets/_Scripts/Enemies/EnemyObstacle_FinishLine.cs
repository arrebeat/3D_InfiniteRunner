using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObstacle_FinishLine : EnemyObstacle
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag(tagPlayer))
        {
            player.Win();
        }
    }
}
