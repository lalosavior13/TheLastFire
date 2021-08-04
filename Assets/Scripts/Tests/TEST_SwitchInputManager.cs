using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;
using nn.hid;

namespace Flamingo
{
[Flags]
public enum NintendoSwitchButton
{
	None,
    A = 0x1 << 0,
    B = 0x1 << 1,
    X = 0x1 << 2,
    Y = 0x1 << 3,
    StickL = 0x1 << 4,
    StickR = 0x1 << 5,
    L = 0x1 << 6,
    R = 0x1 << 7,
    ZL = 0x1 << 8,
    ZR = 0x1 << 9,
    Plus = 0x1 << 10,
    Minus = 0x1 << 11,
    Left = 0x1 << 12,
    Up = 0x1 << 13,
    Right = 0x1 << 14,
    Down = 0x1 << 15,
    StickLLeft = 0x1 << 16,
    StickLUp = 0x1 << 17,
    StickLRight = 0x1 << 18,
    StickLDown = 0x1 << 19,
    StickRLeft = 0x1 << 20,
    StickRUp = 0x1 << 21,
    StickRRight = 0x1 << 22,
    StickRDown = 0x1 << 23,
    LeftSL = 0x1 << 24,
    LeftSR = 0x1 << 25,
    RightSL = 0x1 << 26,
    RightSR = 0x1 << 27,
}

public class TEST_SwitchInputManager : MonoBehaviour
{
	public NintendoSwitchButton TESTSwitchButton;
	public Rect rect;
	private Vector2 leftAxes;
	private Vector2 rightAxes;
	private string ERROR_MESSAGE;
	private StringBuilder additionalBuilder;

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
		additionalBuilder = new StringBuilder();

		try
		{
			NpadStates = new NpadState[1];
			NpadStates[0] = new NpadState();
			Npad.Initialize();
			/*Npad.SetSupportedStyleSet(supportedNpadStyles);
			NpadJoy.SetHoldType(supportedHoldTypes);
			Npad.SetSupportedIdType(supportedIDTypes);*/
		}
		catch(Exception e)
		{
			ERROR_MESSAGE = e.Message;

			/*supportedNpadStyles = NpadStyle.Handheld;
			supportedIDTypes = new NpadId[1];

			Npad.SetSupportedStyleSet(supportedNpadStyles);
			NpadJoy.SetHoldType(supportedHoldTypes);
			Npad.SetSupportedIdType(supportedIDTypes);*/
		}
		finally
		{
			BindIDs(true);
		}

		attackInput = NpadButton.A;
		jumpInput = NpadButton.B;
	}

	/// <summary>Binds set of supported IDs.</summary>
	/// <param name="_bind">Bind them? True by default.</param>
	private void BindIDs(bool _bind = true)
	{
		foreach(NpadId ID in supportedIDTypes)
		{
			switch(_bind)
			{
				case true:
				Npad.BindStyleSetUpdateEvent(ID);
				break;

				case false:
				Npad.DestroyStyleSetUpdateEvent(ID);
				break;
			}	
		}
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

		HandleIDsState();
	}

	private void HandleIDsState()
	{
		additionalBuilder.Clear();
		additionalBuilder.AppendLine();

		foreach(NpadId ID in supportedIDTypes)
		{
			bool updated = Npad.IsStyleSetUpdated(ID);
			NpadStyle style = Npad.GetStyleSet(ID);

			additionalBuilder.Append("Current Npad ID: ");
			additionalBuilder.AppendLine(ID.ToString());
			additionalBuilder.Append("Is Style Set Updated? ");
			additionalBuilder.AppendLine(updated.ToString());
			additionalBuilder.Append("Current ID's Style Set: ");
			additionalBuilder.AppendLine(style.ToString());
		}

		additionalBuilder.AppendLine();
	}

	public override string ToString()
	{
		NpadState state = NpadStates[0];
		StringBuilder builder = new StringBuilder();
		int  i = 0;

		builder.Append("Supported Style's Set: ");
		builder.AppendLine(Npad.GetSupportedStyleSet().ToString());

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

		builder.Append(additionalBuilder.ToString());

		return builder.ToString();
	}
#endif
}
}