using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(SensorSystem2D))]
[RequireComponent(typeof(OrientationNormalAdjuster))]
public class SlopeEvaluator : MonoBehaviour
{
	[SerializeField] private int _groundSensorID; 			/// <summary>Ground's Sensor ID.</summary>
	[SerializeField] private bool _useNormalAdjusterRight; 	/// <summary>User OrientationNormalAdjuster's Right.</summary>
	private Vector3 _right; 								/// <summary>Right's orientation vector [not necessarily the same as Transform.right].</summary>
	private SensorSystem2D _sensorSystem; 					/// <summary>SensorSystem2D's Component.</summary>
	private OrientationNormalAdjuster _normalAdjuster; 		/// <summary>OrientationNormalAdjuster's Component.</summary>

	/// <summary>Gets and Sets groundSensorID property.</summary>
	public int groundSensorID
	{
		get { return _groundSensorID; }
		set { _groundSensorID = value; }
	}

	/// <summary>Gets and Sets right property.</summary>
	public Vector3 right
	{
		get { return _right; }
		set { _right = value; }
	}

	/// <summary>Gets sensorSystem Component.</summary>
	public SensorSystem2D sensorSystem
	{ 
		get
		{
			if(_sensorSystem == null) _sensorSystem = GetComponent<SensorSystem2D>();
			return _sensorSystem;
		}
	}

	/// <summary>Gets normalAdjuster Component.</summary>
	public OrientationNormalAdjuster normalAdjuster
	{ 
		get
		{
			if(_normalAdjuster == null) _normalAdjuster = GetComponent<OrientationNormalAdjuster>();
			return _normalAdjuster;
		}
	}
	
	/// <summary>SlopeEvaluator's tick at each frame.</summary>
	private void Update ()
	{
		RaycastHit2D hit = default(RaycastHit2D);

		sensorSystem.GetSubsystemDetection(groundSensorID, out hit);
		normalAdjuster.up = hit.HasInfo() ? hit.normal.ToVector3() : Vector3.up;

		//Debug.DrawRay(transform.position, normalAdjuster.up * 5.0f, Color.magenta);
	}
}
}