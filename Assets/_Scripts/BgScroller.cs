using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;

public class BgScroller : MonoBehaviour
{
    [SerializeField] private RawImage Bg;

    public bool ScrollActive;

    [Header("Scrolling Speed")]
    public float _x;
    public float _y;

    // Update is called once per frame
    void Update()
    {
        if (ScrollActive)
        {
            Bg.uvRect = new Rect(Bg.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, Bg.uvRect.size);
        }
        
    }
}
