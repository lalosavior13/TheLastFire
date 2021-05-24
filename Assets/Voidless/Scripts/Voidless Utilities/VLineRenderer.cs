using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public static class VLineRenderer
{
	public static void DrawCone(this LineRenderer _lineRenderer, float a, float r)
	{
		int points = 3;
	    float radian = a * Mathf.Deg2Rad;
	    //Debug.Log("Rad:\t" + radian + " Angle:\t" + (Mathf.Rad2Deg * radian) );
	    float radianFract = radian / (float)points;
	    //Debug.Log("Fract:\t" + radianFract);
	    Vector3 center = _lineRenderer.transform.position;    //Where unit stands
	     
	    _lineRenderer.SetVertexCount(points + 3);    //Add start/finish points
	    Vector3 vect = center;    //Start point is center point.
	    _lineRenderer.SetPosition(0, vect);

	    for(int x = 0; x < points + 1; x++)
	    {
	        vect = center;
	        vect.x += (float)(Math.Cos(radianFract * x) * r);
	        vect.z += (float)(Math.Sin(radianFract * x) * r);
	        _lineRenderer.SetPosition(x + 1, vect);    //Skip first/last points.
	    }
	     
	    vect = center;
	    _lineRenderer.SetPosition(points + 2, vect);    //Last point is center point.											
	}
}
}