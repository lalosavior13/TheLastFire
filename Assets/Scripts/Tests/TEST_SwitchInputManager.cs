using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;
using nn.hid;

namespace Flamingo
{
/*[Flags]
public enum NintendoSwitchButton : ulong
{
	None = 0,
    A = NpadButton.A,
    B = NpadButton.B,
    X = NpadButton.X,
    Y = NpadButton.Y,
    StickL = NpadButton.StickL,
    StickR = NpadButton.StickR,
    L = NpadButton.L,
    R = NpadButton.R,
    ZL = NpadButton.ZL,
    ZR = NpadButton.ZR,
    Plus = NpadButton.Plus,
    Minus = NpadButton.Minus,
    Left = NpadButton.Left,
    Up = NpadButton.Up,
    Right = NpadButton.Right,
    Down = NpadButton.Down,
    StickLLeft = NpadButton.StickLLeft,
    StickLUp = NpadButton.StickLUp,
    StickLRight = NpadButton.StickLRight,
    StickLDown = NpadButton.StickLDown,
    StickRLeft = NpadButton.StickRLeft,
    StickRUp = NpadButton.StickRUp,
    StickRRight = NpadButton.StickRRight,
    StickRDown = NpadButton.StickRDown,
    LeftSL = NpadButton.LeftSL,
    LeftSR = NpadButton.LeftSR,
    RightSL = NpadButton.RightSL,
    RightSR = NpadButton.RightSR,
}*/

public class TEST_SwitchInputManager : MonoBehaviour
{
	public Rect rect;
	private Vector2 leftAxes;
	private Vector2 rightAxes;
	private string ERROR_MESSAGE;

	private void OnGUI()
	{
		GUI.Label(rect, ToString());
	}

#if UNITY_SWITCH  || !UNITY_EDITOR || NN_PLUGIN_ENABLE
	[SerializeField] private Mateo mateo; 							/// <summary>Mateo's Reference.</summary>
	[Space(5f)]
	[SerializeField] private NpadId[] supportedIDTypes; 			/// <summary>Supported ID Types.</summary>
	[SerializeField] private NpadStyle supportedNpadStyles; 		/// <summary>Supported Controller's Styles.</summary>
	[SerializeField] private NpadJoyHoldType supportedHoldTypes; 	/// <summary>Supported NpadJoy's Types.</summary>
	[Space(5f)]
	[Header("Input's Settings:")]
	[NonSerialized] private NpadButton attackInput; 				/// <summary>Attack's Input.</summary>
	[NonSerialized] private NpadButton jumpInput; 					/// <summary>Jump's Input.</summary>
	[NonSerialized] private NpadButton fireConjuringFrontalInput0; 	/// <summary>Fire Conjuring Frontal Input #0.</summary>
	[NonSerialized] private NpadButton fireConjuringFrontalInput1; 	/// <summary>Fire Conjuring Frontal Input #1.</summary>
	private NpadState[] NpadStates;

	/// <summary>TEST_SwitchInputManager's instance initialization.</summary>
	private void Awake()
	{
		try
		{
			NpadStates = new NpadState[1];
			NpadStates[0] = new NpadState();
			Npad.Initialize();
			Npad.SetSupportedStyleSet(supportedNpadStyles);
			NpadJoy.SetHoldType(supportedHoldTypes);
			Npad.SetSupportedIdType(supportedIDTypes);
		}
		catch(Exception e)
		{
			ERROR_MESSAGE = e.Message;

			supportedNpadStyles = NpadStyle.Handheld;
			supportedIDTypes = new NpadId[1];

			Npad.SetSupportedStyleSet(supportedNpadStyles);
			NpadJoy.SetHoldType(supportedHoldTypes);
			Npad.SetSupportedIdType(supportedIDTypes);
		}

		attackInput = NpadButton.A;
		jumpInput = NpadButton.B;
	}
	
	/// <summary>TEST_SwitchInputManager's tick at each frame.</summary>
	private void Update ()
	{
		if(mateo == null) return;

		NpadState state = NpadStates[0];

		foreach(NpadId ID in supportedIDTypes)
		{
			NpadStyle style = Npad.GetStyleSet(ID);
			Npad.GetState(ref state, ID, style);
			AnalogStickState rightAnalogStick = state.analogStickR;
			AnalogStickState leftAnalogStick = state.analogStickL;
			rightAxes = new Vector2(rightAnalogStick.fx, rightAnalogStick.fy);
			leftAxes = new Vector2(leftAnalogStick.fx, leftAnalogStick.fy);

			if(state.GetButtonDown(attackInput))
			{
				mateo.SwordAttack(leftAxes);
			}
			if(state.GetButtonDown(jumpInput))
			{
				mateo.Jump(leftAxes);
			}

			if(leftAxes.x != 0.0f) mateo.Move(leftAxes.WithY(0.0f));

			mateo.OnLeftAxesChange(leftAxes);
			mateo.OnRightAxesChange(rightAxes);
		}
	}

	/// <summary>Updates TEST_SwitchInputManager's instance at the end of each frame.</summary>
	private void LateUpdate()
	{
	}

	/*private NpadButton ToNpadButton(NintendoSwitchButton _button)
	{
		return (NpadButton)_button;
	}*/

	public override string ToString()
	{
		NpadState state = NpadStates[0];
		StringBuilder builder = new StringBuilder();
		int  i = 0;

		foreach(NpadId ID in supportedIDTypes)
		{
			NpadStyle style = Npad.GetStyleSet(ID);
			Npad.GetState(ref state, ID, style);
			AnalogStickState rightAnalogStick = state.analogStickR;
			AnalogStickState leftAnalogStick = state.analogStickL;
			rightAxes = new Vector2(rightAnalogStick.fx, rightAnalogStick.fy);
			leftAxes = new Vector2(leftAnalogStick.fx, leftAnalogStick.fy);

			if(!string.IsNullOrEmpty(ERROR_MESSAGE))
			{
				builder.Append("ERROR MESSAGE: ");
				builder.AppendLine(ERROR_MESSAGE);
			}
			builder.Append("ID with index ");
			builder.AppendLine(i.ToString());
			builder.Append("Npad ID: ");
			builder.AppendLine(ID.ToString());
			builder.Append("Style: ");
			builder.AppendLine(style.ToString());
			builder.Append("Left-Axes: " );
			builder.AppendLine(leftAxes.ToString());
			builder.Append("Right-Axes: " );
			builder.AppendLine(rightAxes.ToString());
			builder.Append("N-Pad State: ");
			builder.AppendLine(state.ToString());
			builder.AppendLine();

			i++;
		}

		return builder.ToString();
	}
#endif
}
}