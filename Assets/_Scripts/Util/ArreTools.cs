using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using UnityEditor;

public static class ArreTools
{
#if UNITY_EDITOR
    [MenuItem("arre/Execute %m")]
    public static void Test()
    {
        Debug.Log("Execute order 66");
    }
#endif
    
    public static T GetRandom<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}
