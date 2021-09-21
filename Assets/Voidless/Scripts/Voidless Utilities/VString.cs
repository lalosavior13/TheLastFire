using System.Text;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace Voidless
{
public static class VString
{
	public const int SIZE_BITS_SHORT = sizeof(short) * 8; 													/// <summary>Size in Bits of Short Integer.</summary>
	public const int SIZE_BITS_INT = sizeof(int) * 8; 														/// <summary>Size in Bits of Integer.</summary>
	public const int SIZE_BITS_LONG = sizeof(long) * 8; 													/// <summary>Size in Bits of Long.</summary>
	public const string ALPHABET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"; 		/// <summary>Abecedary.</summary>
	public const string PATH_ROOT_VOIDLESS_UTILITIES = "Voidless Utilties"; 								/// <summary>Voidless Utilities' Root.</summary>
	public const string PATH_ROOT_VOIDLESS_TOOLS = "Voidless Tools"; 										/// <summary>Voidless Tools' Root Path.</summary>
	public const string PATH_SCRIPTABLE_OBJECTS = PATH_ROOT_VOIDLESS_UTILITIES + "/Scriptable Objects"; 	/// <summary>Scriptable Objects' Path.</summary>
	public const string EDITOR_DATA_KEY_MAPPING_PATH = "Path_InputMapping_File";  							/// <summary>Input Mapping File's path for Editor's Data.</summary>

	/// <summary>Converts given string into the format on inspector [spaced upper camel case].</summary>
	/// <param name="_string">String to convert.</param>
	/// <returns>Converted String.</returns>
	public static string ToInspectorFormat(this string _string)
	{
		StringBuilder builder = new StringBuilder();
		int index = 0;
		char lastChar = _string[index];

		while(lastChar == '_')
		{
			index++;
			lastChar = _string[index];
		}

		if(Char.IsLower(lastChar) && Char.IsLower(_string[index + 1]))
		{
			builder.Append(Char.ToUpper(lastChar));
			index++;
		}
		else builder.Append(lastChar);
		
		for(int i = index; i < _string.Length; i++)
		{
			char current = _string[i];

			if(Char.IsUpper(current) && Char.IsLower(lastChar)) builder.Append(" ");

			builder.Append(current);

			lastChar = current;
		}

		return builder.ToString();
	}

	/// <summary>Sets string to Camel Case format.</summary>
	/// <param name="_text">Text to format to Camel Case.</param>
	/// <returns>Formated text.</returns>
	public static string ToCamelCase(this string _text)
	{
		return _text.Replace(_text[0], char.ToLower(_text[0]));
	}

	/// <summary>Gives a string, replacing all instances of chars into a new char.</summary>
	/// <param name="_text">String to replace chars to.</param>
	/// <param name="_from">Char instance to replace.</param>
	/// <param name="_to">Char to substitute.</param>
	/// <returns>String with chars replaced.</returns>
	public static string WithReplacedChars(this string _text, char _from, char _to)
	{
		StringBuilder result = new StringBuilder();

		for(int i = 0; i < _text.Length; i++)
		{
			result.Append((_text[i] == _from) ? _to : _text[i]);
		}

		return result.ToString();
	}

	/// <summary>Replaces all chars' instances of a string into a new char.</summary>
	/// <param name="_text">String to replace chars to.</param>
	/// <param name="_from">Char instance to replace.</param>
	/// <param name="_to">Char to substitute.</param>
	public static void ReplaceChars(ref string _text, char _from, char _to)
	{
		StringBuilder result = new StringBuilder();

		for(int i = 0; i < _text.Length; i++)
		{
			result.Append((_text[i] == _from) ? _to : _text[i]);
		}

		_text = result.ToString();
	}

	public static string GenerateRandomString(int length, string _string = ALPHABET)
	{
		Random random = new Random();
		StringBuilder result = new StringBuilder(length);
		for(int i = 0; i < length; i++)
		{
			result.Append(_string[random.Next(_string.Length)]);
		}

		return result.ToString();
	}

	/// <summary>Converts Snake Case Text to Spaced Case.</summary>
	/// <param name="_text">Text to convert.</param>
	/// <returns>Text with spaces instead of underscores.</returns>
	public static string SnakeCaseToSpacedText(this string _text)
	{
		return _text.Replace("_", " ");
	}

	/// <summary>Creates a string of characters repeated n times.</summary>
	/// <param name="_character">Character to repeat.</param>
	/// <returns>String of characters repeated n times.</returns>
	public static string CharactersPeriodically(char _character, int _count)
	{
		StringBuilder builder = new StringBuilder();

		for(int i = 0; i < _count; i++)
		{
			builder.Append(_character);
		}

		return builder.ToString();
	}

	/// <summary>Creates a string of strings repeated n times.</summary>
	/// <param name="_character">Character to repeat.</param>
	/// <returns>String of strings repeated n times.</returns>
	public static string StringsPeriodically(string _text, int _count)
	{
		StringBuilder builder = new StringBuilder();

		for(int i = 0; i < _count; i++)
		{
			builder.Append(_text);
		}

		return builder.ToString();
	}

	/// <summary>Retreives Object's Name.</summary>
	/// <param name="_object">Object to get name from.</param>
	/// <returns>Object's Name.</returns>
	public static string ClassName<T>(this T _object)
	{
		return _object.GetType().Name;
	}

	/// <summary>Gets Bit Chain from Integer.</summary>
	/// <param name="x">Byte to get Bit Chain from.</param>
	/// <returns>String representing Bit Chain.</returns>
	public static string GetBitChain(this byte x)
	{
		StringBuilder builder = new StringBuilder();

		builder.Append("{ ");

		for(int i = SIZE_BITS_INT ; i > -1; i--)
		{
			builder.Append((x | (1 << i)) == x ? 1 : 0);
			if(i > 0) builder.Append(", ");
		}

		builder.Append(" }");

		return builder.ToString();
	}

	/// <summary>Gets Bit Chain from Integer.</summary>
	/// <param name="x">Integer to get Bit Chain from.</param>
	/// <returns>String representing Bit Chain.</returns>
	public static string GetBitChain(this int x)
	{
		StringBuilder builder = new StringBuilder();

		builder.Append("{ ");

		for(int i = SIZE_BITS_INT ; i > -1; i--)
		{
			builder.Append((x | (1 << i)) == x ? 1 : 0);
			if(i > 0) builder.Append(", ");
		}

		builder.Append(" }");

		return builder.ToString();
	}

	/// <summary>Gets Bit Chain from Long.</summary>
	/// <param name="x">Long to get Bit Chain from.</param>
	/// <returns>String representing Bit Chain.</returns>
	public static string GetBitChain(this long x)
	{
		StringBuilder builder = new StringBuilder();

		builder.Append("{ ");

		for(int i = SIZE_BITS_LONG ; i > -1; i--)
		{
			builder.Append((x | (1 << i)) == x ? 1 : 0);
			if(i > 0) builder.Append(", ");
		}

		builder.Append(" }");

		return builder.ToString();
	}

	/// <summary>Evaluates whether string begins with given superstring.</summary>
	/// <param name="_string">String to evaluate.</param>
	/// <param name="_superString">Superstring that the string ought to contain.</param>
	/// <returns>True if string begins with Superstring.</returns>
	public static bool HasSuperstring(this string _string, string _superString)
	{
		if(string.IsNullOrEmpty(_string)
		|| string.IsNullOrEmpty(_superString)
		|| (_string.Length < _superString.Length)) return false;

		int length = _superString.Length;

		for(int i = 0; i < length; i++)
		{
			if(_string[i] != _superString[i]) return false;
		}

		return true;
	}

	/// <summary>Gets a subtring from a given string.</summary>
	/// <param name="_string">String to get the substring from.</param>
	/// <param name="_superString">SuperString from the string.</param>
	/// <returns>Substring from string.</returns>
	public static string Substring(this string _string, string _superString)
	{
		return _string.Substring(_superString.Length);
	}

	/// <summary>Builds a String that contains information of each item of a Collection.</summary>
	/// <param name="_collection">Given Collection.</param>
	/// <returns>String representing each item of given Collection.</returns>
	public static string CollectionToString<T>(this ICollection<T> _collection)
	{
		StringBuilder builder = new StringBuilder();
		IEnumerator<T> iterator = _collection.GetEnumerator();
		
		builder.Append("Collection<");
		builder.Append(typeof(T).ToString());
		builder.AppendLine(">: ");

		while(iterator.MoveNext())
		{
			builder.Append("\n");
			builder.Append(iterator.Current.ToString());
		}

		return builder.ToString();
	}

	/// <summary>Creates a string representing a HashSet and each of its containing elements.</summary>
	/// <param name="_hashSet">HashSet to represent to string.</param>
	/// <returns>String representing HashSet.</returns>
	public static string HashSetToString<T>(this HashSet<T> _hashSet)
	{
		StringBuilder builder = new StringBuilder();
		int index = 0;

		builder.Append("HashSet<");
		builder.Append(typeof(T).ToString());
		builder.Append(">: ");

		foreach(T item in _hashSet)
		{
			builder.Append("\n Item ");
			builder.Append(index.ToString());
			builder.Append(": ");
			builder.Append(item.ToString());
			index++;
		}

		return builder.ToString();
	}

	/// <summary>Creates a string representing a Dictionary and each of its containing elements.</summary>
	/// <param name="_dictionary">Dictionary to represent to string.</param>
	/// <returns>String representing Dictionary.</returns>
	public static string DictionaryToString<K, V>(this Dictionary<K, V> _dictionary)
	{
		StringBuilder builder = new StringBuilder();
		int index = 0;

		builder.Append("Dictionary<");
		builder.Append(typeof(K).ToString());
		builder.Append(", ");
		builder.Append(typeof(V).ToString());
		builder.AppendLine(">");

		foreach(KeyValuePair<K, V> pair in _dictionary)
		{
			builder.Append("Item ");
			builder.Append(index.ToString());
			builder.Append(": { ");
			builder.Append(pair.Key.ToString());
			builder.Append(", ");
			builder.Append(pair.Value.ToString());
			builder.AppendLine(" }");
			index++;
		}

		return builder.ToString();
	}

	/*/// <summary>Creates a string representing a ICollection of generic type T and each of its containing elements.</summary>
	/// <param name="_hashSet">ICollection of generic type T to represent to string.</param>
	/// <returns>String representing ICollection of generic type T.</returns>
	public static string CollectionToString<T>(this ICollection<T> _collection)
	{
		StringBuilder builder = new StringBuilder();
		int index = 0;

		builder.Append("Collection<");
		builder.Append(typeof(T).ToString());
		builder.Append(">: ");

		foreach(T item in _collection)
		{
			builder.Append("\n Item ");
			builder.Append(index.ToString());
			builder.Append(": ");
			builder.Append(item.ToString());
			index++;
		}

		return builder.ToString();
	}*/

	/// <summary>Gets Methods from object.</summary>
	/// <param name="_object">Object.</param>
	/// <returns>Methods' Information from given object.</returns>
	public static string GetMethods(object _object)
	{
		return GetMethods(_object.GetType());
	}

	/// <summary>Gets Methods from object.</summary>
	/// <param name="type">Type.</param>
	/// <returns>Methods' Information from given object.</returns>
	public static string GetMethods(Type type)
	{
		StringBuilder builder = new StringBuilder();
		MemberInfo[] membersInfo = type.GetMembers();
		int i = 0;

		builder.Append(type.Name);
		builder.AppendLine("'s Members' Information: ");

		if(membersInfo != null) foreach(MemberInfo memberInfo in membersInfo)
		{
			builder.Append("Member info #");
			builder.Append(i.ToString());
			builder.Append(": ");
			builder.AppendLine(memberInfo.ToString());
			i++;
		}

		return builder.ToString();
	}
}
}