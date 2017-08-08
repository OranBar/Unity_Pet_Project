using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CreateAssetMenu(fileName = "CurrStyle", menuName = "Continuum/CreateStyle", order = 1)]
public class Style : ScriptableObject {

	public GUIStyle style;

	
}

public class StyleCreator
{
	[MenuItem("Assets/Create/Style")]
	public static void CreateStyle()
	{
		Style newStyle = ScriptableObject.CreateInstance<Style>();

		AssetDatabase.CreateAsset(newStyle, "Assets/9_Project_Continuum/Styles/NewStyle.asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();
		Selection.activeObject = newStyle;
	}
}

#endif