using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineRendererHelper : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform[] positions;    

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        positions = GetComponentsInChildren<Transform>().Where(t => t != transform).ToArray();
    }

    void Start()
    {
        lineRenderer.positionCount = positions.Length;
        SetPositions();
    }

    private void SetPositions()
    {
        for (int i = 0; i < positions.Length; ++i)
        {
            lineRenderer.SetPosition(i, positions[i].position);
        }
    }
}
