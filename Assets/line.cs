using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class line : MonoBehaviour {
    public LineRenderer LineRenderer;
	// Use this for initialization 
	
	
	// Update is called once per frame
	
    public void DrawLine(Vector3 a, Vector3 b)
    {
        Vector3[] list = { a, b };
        LineRenderer.SetPositions(list);
    }
}
