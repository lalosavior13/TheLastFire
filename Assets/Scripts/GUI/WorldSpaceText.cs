using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Voidless;

namespace Flamingo
{
public class WorldSpaceText : PoolGameObject
{
	[SerializeField] private Text _text; 	/// <summary>World-Space's Text.</summary>

	/// <summary>Gets text property.</summary>
	public Text text { get { return _text; } }
}
}