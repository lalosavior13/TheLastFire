using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(ScreenFaderGUI))]
[RequireComponent(typeof(Canvas))]
public class GameplayGUIController : MonoBehaviour
{
	private ScreenFaderGUI _screenFaderGUI; 	/// <summary>ScreenFaderGUI's Component.</summary>
	private Canvas _canvas; 					/// <summary>Canvas' Component.</summary>

	/// <summary>Gets screenFaderGUI Component.</summary>
	public ScreenFaderGUI screenFaderGUI
	{ 
		get
		{
			if(_screenFaderGUI == null) _screenFaderGUI = GetComponent<ScreenFaderGUI>();
			return _screenFaderGUI;
		}
	}

	/// <summary>Gets canvas Component.</summary>
	public Canvas canvas
	{ 
		get
		{
			if(_canvas == null) _canvas = GetComponent<Canvas>();
			return _canvas;
		}
	}

#region UnityMethods:
	/// <summary>GameplayGUIController's instance initialization.</summary>
	private void Awake()
	{
		
	}

	/// <summary>GameplayGUIController's starting actions before 1st Update frame.</summary>
	private void Start ()
	{
		
	}
	
	/// <summary>GameplayGUIController's tick at each frame.</summary>
	private void Update ()
	{
		
	}
#endregion
}
}