using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public enum FuzzyRegion 																/// <summary>Fuzzy Set's Regions.</summary>
{
	Core, 																				/// <summary>Full membership of set [f(x) = 1].</summary>
	Support, 																			/// <summary>Non-zero membership of set [f(x) = 0].</summary>
	Boundary 																			/// <summary>Partial membership of set [0 >= f(x) >= 1].</summary>
}

public class FuzzySystem<T> where T : struct
{
	private const string NOT_ENUM = "Argument provided of type T is not an Enum."; 		/// <summary>Not enumerator argument exceotion message.</summary>
	private const string SETS_PROVIDED = "The number of sets provided is "; 			/// <summary>Number of sets provided warning's message.</summary>
	private const string SETS_SMALLER = "smaller "; 									/// <summary>Smaller sets provided warning/Exception's message.</summary>
	private const string SETS_BIGGER = "bigger "; 										/// <summary>Bigger sets provided warning/Exception's message.</summary>
	private const string ENUM_VALUES = " than the number of enumerator values."; 		/// <summary>Enum values argument's message.</summary>
	private const string UNEXPECTED_RESULT = "Unexpected result, returning default"; 	/// <summary>Unexpected result's warning message.</summary>
	private const float HEIGHT_MIN = 0.0f; 												/// <summary>Height's value [min normalized value].</summary>
	private const float HEIGHT_MAX = 1.0f; 												/// <summary>Height's value [max normalized value].</summary>

	private static FuzzySystem<T> _Instance; 											/// <summary>Singleton's Instance reference.</summary>

	private FuzzySubset<T>[] _fuzzySet; 												/// <summary>Set of Fuzzy Sets.</summary>
	private FloatRange _range; 															/// <summary>System's Range.</summary>
	private Dictionary<T, FuzzySubset<T>> _setMapping; 									/// <summary>Fuzzy Sets' Mapping.</summary>
	private int _setsLength; 															/// <summary>How many values the variable has [calculated in constructor].</summary>
	private bool _isEven; 																/// <summary>Is the Fuzzy Sets' Set Even?.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets Instance property.</summary>
	public static FuzzySystem<T> Instance
	{
		get { return _Instance; }
		set { _Instance = value; }
	}

	/// <summary>Gets and Sets fuzzySet property.</summary>
	public FuzzySubset<T>[] fuzzySet
	{
		get { return _fuzzySet; }
		private set { _fuzzySet = value; }
	}

	/// <summary>Gets and Sets range property.</summary>
	public FloatRange range
	{
		get { return _range; }
		private set { _range = value; }
	}

	/// <summary>Gets and Sets setMapping property.</summary>
	public Dictionary<T, FuzzySubset<T>> setMapping
	{
		get { return _setMapping; }
		private set { _setMapping = value; }
	}

	/// <summary>Gets and Sets setsLength property.</summary>
	public int setsLength
	{
		get { return _setsLength; }
		private set { _setsLength = value; }
	}

	/// <summary>Gets and Sets isEven property.</summary>
	public bool isEven
	{
		get { return _isEven; }
		private set { _isEven = value; }
	}
#endregion

	/// <summary>Fuzzy System's private constructor.</summary>
	private FuzzySystem()
	{
		Type enumType = typeof(T);
		//if(!enumType.IsEnum) throw new ArgumentException(NOT_ENUM);
		setsLength = Enum.GetValues(typeof(T)).Length;
		isEven = (setsLength % 2 == 0);
	}

	/// <summary>Fuzzy System constructor. Builds automagically a Fuzzy set, out of the range defined</summary>
	/// <param name="_range">System's range.</param>
	public FuzzySystem(FloatRange _range): this()
	{
		range = _range;
		CreateFuzzySetAndMapping();
	}

	/// <summary>Fuzzy System constructor. Use this constructor to make a custom Fuzzy Set.</summary>
	/// <param name="_fuzzySet">Set of Fuzzy Sets.</param>
	public FuzzySystem(params FuzzySubset<T>[] _fuzzySet): this()
	{
		
		range = new FloatRange(_fuzzySet[0].valueRange.Max(), _fuzzySet.GetLast().valueRange.Min());
		fuzzySet = _fuzzySet;
		CreateFuzzySetMapping();
	}

	/// <summary>Creates Fuzzy Subsets and Mapping.</summary>
	private void CreateFuzzySetAndMapping()
	{
		setMapping = new Dictionary<T, FuzzySubset<T>>();
		fuzzySet = new FuzzySubset<T>[setsLength];
		FloatRange newValueRange = 0.0f;
		FloatRange newSetRange = 0.0f;
		Vector2? intersectionPoint = null;
		float rangeSplit = (range.GetLength() / ((setsLength - 1) * 1.0f));
		int i = 0;

		foreach(T value in Enum.GetValues(typeof(T)))
		{
			newValueRange =
			i == 0 ? new FloatRange(Mathf.NegativeInfinity, range.min)
				: i == (setsLength - 1) ? new FloatRange(range.max, Mathf.Infinity)
					: range.min + (i * rangeSplit);
			newSetRange =
			i == 0 ? new FloatRange(Mathf.NegativeInfinity, (range.min + ((i + 1) * rangeSplit)))
				: i == (setsLength - 1) ? new FloatRange(range.min + ((i - 1) * rangeSplit), Mathf.Infinity)
					: new FloatRange(range.min + ((i - 1) * rangeSplit), (range.min + ((i + 1) * rangeSplit)));

			fuzzySet[i] = new FuzzySubset<T>(newSetRange, newValueRange, value);
			setMapping.Add(value, fuzzySet[i]);
			i++;
		}

		for(i = 0; i < setsLength; i++)
		{
			if(i < (setsLength - 1))
			{
				intersectionPoint = VMath.CalculateIntersectionBetween(fuzzySet[i].GetMaxRangeRay(), fuzzySet[i + 1].GetMinRangeRay());

				if(i == 0) fuzzySet[i].intersectionRange.min = Mathf.NegativeInfinity;
				if(intersectionPoint.HasValue)
				{
					fuzzySet[i].intersectionRange.max = intersectionPoint.Value.x;
					fuzzySet[i + 1].intersectionRange.min = intersectionPoint.Value.x;
				}
				else
				{
					fuzzySet[i].intersectionRange.max = fuzzySet[i].range.max;
					fuzzySet[i + 1].intersectionRange.min = fuzzySet[i + 1].range.min;
				}
			}
			else fuzzySet[i].intersectionRange.max = Mathf.Infinity;
		}
	}

	/// <summary>Creates Mapping and assigns the intersection points for each subset.</summary>
	private void CreateFuzzySetMapping()
	{
		if(fuzzySet.Length < setsLength) throw new ArgumentException(SETS_PROVIDED + SETS_SMALLER + ENUM_VALUES);
		else if(fuzzySet.Length > setsLength) Debug.LogWarning(SETS_PROVIDED + SETS_BIGGER + ENUM_VALUES);

		setMapping = new Dictionary<T, FuzzySubset<T>>();
		//FuzzySubset<T> fuzzySubset = default(FuzzySubset<T>);
		Vector2? intersectionPoint = null;

		for(int i =0; i < setsLength; i++)
		{
			if(i < (setsLength - 1))
			{
				intersectionPoint = VMath.CalculateIntersectionBetween(fuzzySet[i].GetMaxRangeRay(), fuzzySet[i + 1].GetMinRangeRay());

				if(i == 0) fuzzySet[i].intersectionRange.min = Mathf.NegativeInfinity;
				if(intersectionPoint.HasValue && IntersectionPointOnRange(intersectionPoint.Value))
				{
					fuzzySet[i].intersectionRange.max = intersectionPoint.Value.x;
					fuzzySet[i + 1].intersectionRange.min = intersectionPoint.Value.x;
				}
				else
				{
					fuzzySet[i].intersectionRange.max = fuzzySet[i].range.max;
					fuzzySet[i + 1].intersectionRange.min = fuzzySet[i + 1].range.min;
				}
			}
			else fuzzySet[i].intersectionRange.max = Mathf.Infinity;
			setMapping.Add(fuzzySet[i].value, fuzzySet[i]);
		}
	}

	/// <summary>Defuzzyficates input to get a Mermbership degree values.</summary>
	/// <param name="_fuzzySubset">Fuzzy Set's to evaluate with input.</param>
	/// <param name="_input">Input to evaluate with Fuzzy Set.</param>
	/// <returns>Defuzzyficated membership degree value.</returns>
	private float? Defuzzyficate(FuzzySubset<T> _fuzzySubset, float _input)
	{
		if(_input >= _fuzzySubset.valueRange.Min() && _input <= _fuzzySubset.valueRange.Max())
		{ /// In Core Region.
			return 1.0f;
		} else if(_input > _fuzzySubset.range.Min() && _input < _fuzzySubset.valueRange.Min())
		{ /// In Boundary Region.
			return MembershipFunction(_fuzzySubset.range.Min(), _fuzzySubset.valueRange.Min(), _input);

		} else if(_input > _fuzzySubset.valueRange.Max() && _input < _fuzzySubset.range.Max())
		{ /// In Boundary Region.
			return MembershipFunction(_fuzzySubset.range.Max(), _fuzzySubset.valueRange.Max(), _input);
		} /// In Support Region.
		else return null;
	}

	/// <summary>Membership Function, returning a Membership Degree.</summary>
	/// <param name="_min">Minimum value.</param>
	/// <param name="_max">Maximum value.</param>
	/// <param name="_input">Input to evaluate.</param>
	/// <returns>Value between 0.0 and 1.0 representing the membership degree.</returns>
	private float MembershipFunction(float _min, float _max, float _input)
	{
		return Mathf.Clamp(((_input - _min) / (_max - _min)), HEIGHT_MIN, HEIGHT_MAX);
	}

	/// <summary>Rearranges Fuzzy System, given a new float Range.</summary>
	/// <param name="_newRange">Fuzzy Set's new Range.</param>
	public void Rearrange(FloatRange _newRange)
	{
		range = _newRange;
		FloatRange newValueRange = 0.0f;
		FloatRange newSetRange = 0.0f;
		Vector2? intersectionPoint = null;
		float rangeSplit = (range.GetLength() / ((setsLength - 1) * 1.0f));
		int i = 0;

		foreach(T value in Enum.GetValues(typeof(T)))
		{
			newValueRange = i == 0 ? range.min : i == (setsLength - 1) ? range.max : range.min + (i * rangeSplit);
			newSetRange =
			i == 0 ? new FloatRange(Mathf.NegativeInfinity, (range.min + ((i + 1) * rangeSplit)))
				: i == (setsLength - 1) ? new FloatRange(range.min + ((i - 1) * rangeSplit), Mathf.Infinity)
					: new FloatRange(range.min + ((i - 1) * rangeSplit), (range.min + ((i + 1) * rangeSplit)));

			fuzzySet[i] = new FuzzySubset<T>(newSetRange, newValueRange, value);
			setMapping[value] = fuzzySet[i];
			i++;
		}

		for(i = 0; i < setsLength; i++)
		{
			if(i < (setsLength - 1))
			{
				intersectionPoint = VMath.CalculateIntersectionBetween(fuzzySet[i].GetMaxRangeRay(), fuzzySet[i + 1].GetMinRangeRay());

				if(i == 0) fuzzySet[i].intersectionRange.min = Mathf.NegativeInfinity;
				if(intersectionPoint.HasValue && IntersectionPointOnRange(intersectionPoint.Value))
				{
					fuzzySet[i].intersectionRange.max = intersectionPoint.Value.x;
					fuzzySet[i + 1].intersectionRange.min = intersectionPoint.Value.x;
				}
				else
				{
					fuzzySet[i].intersectionRange.max = fuzzySet[i].range.max;
					fuzzySet[i + 1].intersectionRange.min = fuzzySet[i + 1].range.min;
				}
			}
			else fuzzySet[i].intersectionRange.max = Mathf.Infinity;
		}
	}

	/// <summary>Gets Value's Membership Degree given an input.</summary>
	/// <param name="_value">Value to evaluate with input.</param>
	/// <param name="_input">Input to evaluate with value.</param>
	/// <returns>Value's Membership degree relative to the input provided.</returns>
	public float GetValueMembershipDegree(T _value, float _input)
	{
		return Defuzzyficate(setMapping[_value], _input) ?? 0.0f;
	}

	/// <summary>Gets Variable's value given a provided input.</summary>
	/// <param name="_input">Input to evaluate.</param>
	/// <returns>A value if the input is on a set's range, null otherwise.</returns>
	public T GetValue(float _input)
	{
		for(int i = 0; i < setsLength; i++)
		{
			if(_input >= fuzzySet[i].intersectionRange.min && _input <= fuzzySet[i].intersectionRange.max)
			return (T)(object)(i);
		}

		Debug.LogWarning(UNEXPECTED_RESULT);
		return default(T);
	}

	/// <summary>Gets Variable's value index, given an input.</summary>
	/// <param name="_input">Input to evaluate.</param>
	/// <returns>An input if it retrieved something, none if it didn't.</returns>
	public int GetValueIndex(float _input)
	{
		for(int i = 0; i < setsLength; i++)
		{
			if(_input >= fuzzySet[i].intersectionRange.min && _input <= fuzzySet[i].intersectionRange.max)
			return i;
		}

		Debug.LogWarning(UNEXPECTED_RESULT);
		return 0;
	}

	/// <summary>Gets Fuzzy Result [Value with Membership Degree] given an input.</summary>
	/// <param name="_input">Provided input.</param>
	/// <returns>Fuzzy Set, with Set's Range, Value's Range, Variable's value and Membership Degree.</returns>
	public FuzzyResult<T> GetFuzzyResult(float _input)
	{
		foreach(FuzzySubset<T> fuzzySubset in fuzzySet)
		{
			if(_input >= fuzzySubset.intersectionRange.min && _input <= fuzzySubset.intersectionRange.max)
			return new FuzzyResult<T>(fuzzySubset.value, Defuzzyficate(fuzzySubset, _input) ?? 0.0f);
		}

		Debug.LogWarning(UNEXPECTED_RESULT);
		return default(FuzzyResult<T>);
	}

	/// <summary>Gets Membership degree given an input.</summary>
	/// <param name="_input">input value.</param>
	/// <returns>Normalized Membership degree.</returns>
	public float GetMembershipDegree(float _input)
	{
		foreach(FuzzySubset<T> fuzzySubset in fuzzySet)
		{
			float? defuzzyfication = Defuzzyficate(fuzzySubset, _input);
			
			if(defuzzyfication.HasValue) return defuzzyfication.Value;
		}

		return HEIGHT_MIN;
	}

	/// <summary>Gets Crisp value, given a variable value and its membership degree.</summary>
	/// <param name="_value">Variable's value.</param>
	/// <param name="_membershipDegree">Assiciated membership's degree.</param>
	/// <returns>Crisp value of the range relative to the membership degree provided.</returns>
	public float GetCrispValue(T _value, float membershipDegree)
	{
		FuzzySubset<T> fuzzySubset = setMapping[_value];
		return Mathf.Lerp(fuzzySubset.range.min, fuzzySubset.range.max, membershipDegree);
	}

	/// <summary>Gets Fuzzy Region acconding to the provided membership degree.</summary>
	/// <param name="_membershipDegree">Membership's Degree.</param>
	/// <returns>Fuzzy Region between { Core, Support, Boundary }.</returns>
	public FuzzyRegion GetMembershipRegion(float _memberShipDegree)
	{
		_memberShipDegree = Mathf.Clamp(_memberShipDegree, HEIGHT_MIN, HEIGHT_MAX);
		return _memberShipDegree == 1.0f ? FuzzyRegion.Core : _memberShipDegree == HEIGHT_MIN ? FuzzyRegion.Support : FuzzyRegion.Boundary;
	}

	/// <summary>Checks if calculated intersection point is on fuzzy's graph range [if the Y component is between normalized's range].</summary>
	/// <param name="_intersectionPoint">Intersection Point to evaluate.</param>
	/// <returns>True if the Y component of the intersection point is within the normalized's range.</returns>
	private bool IntersectionPointOnRange(Vector2 _intersectionPoint)
	{
		return (_intersectionPoint.y >= HEIGHT_MIN && _intersectionPoint.y <= HEIGHT_MAX);
	}

	/// <returns>String representing this Fuzzy System.</returns>
	public override string ToString()
	{
		StringBuilder builder = new StringBuilder();

		builder.Append(typeof(T).Name.ToString());
		builder.Append(" Fuzzy System's Set.");
		builder.Append("\n");
		builder.Append("With Range: ");
		builder.Append(range.ToString());
		builder.Append("\n");
		builder.Append("Composed of the following subsets: ");
		builder.Append("\n");
		builder.Append("\n");
		foreach(FuzzySubset<T> fuzzySubset in fuzzySet)
		{
			builder.Append(fuzzySubset.ToString());
			builder.Append("\n");
		}

		return builder.ToString();
	}
}
}