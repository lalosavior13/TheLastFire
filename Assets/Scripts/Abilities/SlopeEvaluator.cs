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
	[SerializeField] private float _angleLimit; 			/// <summary>Angle's Limit.</summary>
	[SerializeField] private int _groundSensorID; 			/// <summary>Ground's Sensor ID.</summary>
	[SerializeField] private bool _useNormalAdjusterRight; 	/// <summary>User OrientationNormalAdjuster's Right.</summary>
	private float _dotLimit; 								/// <summary>Dot Product's Limit.</summary>
	private Vector3 _right; 								/// <summary>Right's orientation vector [not necessarily the same as Transform.right].</summary>
	private SensorSystem2D _sensorSystem; 					/// <summary>SensorSystem2D's Component.</summary>
	private OrientationNormalAdjuster _normalAdjuster; 		/// <summary>OrientationNormalAdjuster's Component.</summary>

	/// <summary>Gets and Sets angleLimit property.</summary>
	public float angleLimit
	{
		get { return _angleLimit; }
		set
		{
			_angleLimit = value;
			_dotLimit = VMath.AngleToDotProduct(value);
		}
	}

	/// <summary>Gets and Sets dotLimit property.</summary>
	public float dotLimit
	{
		get { return _dotLimit; }
		set { _dotLimit = value; }
	}

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

	/// <summary>SlopeEvaluator's instance initialization when loaded [Before scene loads].</summary>
	private void Awake()
	{
		dotLimit = _dotLimit = VMath.AngleToDotProduct(angleLimit);
	}
	
	/// <summary>SlopeEvaluator's tick at each frame.</summary>
	private void Update ()
	{
		RaycastHit2D hit = default(RaycastHit2D);
		Vector3 up = Vector3.zero;

		sensorSystem.GetSubsystemDetection(groundSensorID, out hit);

		up = hit.normal.ToVector3();

		normalAdjuster.up = hit.HasInfo() && Vector3.Dot(up, Vector3.up) >= dotLimit ? up : Vector3.up;
	}
}
}