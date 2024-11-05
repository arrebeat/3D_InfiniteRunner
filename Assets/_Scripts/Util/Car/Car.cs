using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public GameObject prefab;
    public Vector3 spawnPos;
    public int speed = 10;
    public int gear = 5;

    public int totalSpeed
    {
        get { return speed * gear; }
    }

    public void Spawn()
    {
        var a = Instantiate(prefab);
        a.transform.position = spawnPos;
    }
}
