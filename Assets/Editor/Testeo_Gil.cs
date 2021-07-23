using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Voidless
{
public class Testeo_Gil : EditorWindow
{
	protected const string TESTEO_GIL_PATH = "Test/MyWindow/Testeo_Gil's PATH"; 	/// <summary>Testeo_Gil's path.</summary>

	public static Testeo_Gil testeo_Gil;                                        /// <summary>Testeo_Gil's static reference</summary>

	string myString = "HelloWorld";
	private int userArraySize;

	private static TESTInfoGil infoGil;                                     /// <summary> Serialiazable class to save data </summary>
		string[] arrayInfo;													/// <summary> array text info </summary>

	bool enableGroup;                                                       /// <summary> Bool to interact with ToggleGroup </summary>
	float toggleFloat;                                                      /// <summary> toogle group float slider value </summary>

		bool showShiet;

	/// <summary>Creates a new Testeo_Gil window.</summary>
	/// <returns>Created Testeo_Gil window.</summary>
	[MenuItem(TESTEO_GIL_PATH)]
	public static Testeo_Gil CreateTesteo_Gil()
	{
		testeo_Gil = GetWindow<Testeo_Gil>("MyFirstWindowGil");
			infoGil = new TESTInfoGil();
		return testeo_Gil;
	}

	/// <summary>Use OnGUI to draw all the controls of your window.</summary>
	private void OnGUI()
	{
		CreateLabel("This is a testing to do a input field");
		myString = EditorGUILayout.TextField("Text Field", myString);

		CreateLabel("This is a testing to create an array");
		EditArrayLenght();
		ShowArray();

		CreateLabel("This is a test to create a HORIZONTAL group");
		DrawnHorizontalGroup();

		CreateLabel("This is a test to create a VERTICAL group");
		DrawnVerticalGroup();

		CreateLabel("This is a test to create a TOGGLE group");
		ToogleGroup();

		CreateLabel("This is a test to create a TOGGLE");
		ToogleStuff();



		}

	/// <summary> Gives a space and a label to separate seccions </summary>
	void CreateLabel(string _text)
    {
		EditorGUILayout.Space();
		GUILayout.Label(_text, EditorStyles.boldLabel);
	}

#region Array : 
	/// <summary> Shows and array with the values that the user gives </summary>
	void ShowArray()
    {
		EditorGUILayout.Space();
        for (int i = 0; i < userArraySize; i++)
        {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(i.ToString() + " : ", GUILayout.Width(50f));
			infoGil.arrayInfo[i] = EditorGUILayout.TextField(infoGil.arrayInfo[i], GUILayout.Width(150f));
			EditorGUILayout.EndHorizontal();
        }
		
       
    }

	/// <summary> Allow to modifie the values of the array</summary>
	void EditArrayLenght()
    {
		EditorGUILayout.Space();

			if (infoGil == null) return;

		userArraySize = EditorGUILayout.DelayedIntField("Array size: ", userArraySize);
		infoGil.ResizeInfoArray(userArraySize);
	}
	#endregion

# region Groups
	void DrawnHorizontalGroup()
    {
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("First Text", GUILayout.Width(150F));
		GUILayout.Label("Second Text", GUILayout.Width(150F));
		EditorGUILayout.EndHorizontal();
	}
	void DrawnVerticalGroup()
	{
		EditorGUILayout.BeginVertical();
		GUILayout.Label("First Text", GUILayout.Width(150F));
		GUILayout.Label("Second Text", GUILayout.Width(150F));
		EditorGUILayout.EndVertical();
	}

	void ToogleGroup()
    {
		enableGroup = EditorGUILayout.BeginToggleGroup("ToggleMenu", enableGroup);
		toggleFloat = EditorGUILayout.Slider("Float Slider", toggleFloat, -3, 3);
		EditorGUILayout.EndToggleGroup();
    }


	void ToogleStuff()
    {
			showShiet = EditorGUILayout.Toggle("Show some shiet", showShiet);
			if(showShiet)
            {
				GUILayout.Label("You have toggle some shiet!, good for you", EditorStyles.boldLabel);
			}

    }



#endregion
}

}
