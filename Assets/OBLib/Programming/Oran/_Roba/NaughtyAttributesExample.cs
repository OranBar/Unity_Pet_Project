using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using System.Linq;

namespace Testing
{
	[Serializable]
	public enum NaughtyTestEnum
	{
		Collider,
		Orange,
		Skybox,
		Skull,
		Material
	}

	public class NaughtyAttributesExample : MonoBehaviour
	{
		#region Inspector
		[BoxGroup("BoxGroup 1")]
		public int aInt;

		[BoxGroup("BoxGroup 1")]
		public Transform aTr;

		[BoxGroup("BoxGroup 2")]
		public string aStr;

		[BoxGroup("BoxGroup 2")]
		public GameObject aGo;

		[Button]
		public void Foo()
		{

		}

		[BoxGroup("DropDown and Reordarable Arrays")]
		[Dropdown("intValues")]
		[OnValueChanged("aDropdownValueChanged")]
		[InfoBox("Has Callback To value being set to 125")]
		public int aDropdownValue;

		private void aDropdownValueChanged()
		{
			if (aDropdownValue == 125)
			{
				Debug.Log("Naughty Callback");
			}
		}

		[BlankSpace]

		[BoxGroup("DropDown and Reordarable Arrays")]
		[ReorderableList]
		[InfoBox("This list is fed as the available dropdown values")]
		public int[] intValues = new int[] { 1, 5, 125 };

		[BoxGroup("DropDown and Reordarable Arrays")]
		[Dropdown("enumElements")]
		[InfoBox("Try to set as Aliens", InfoBoxType.Warning, "IsNotAliens")]
		[ValidateInput("CantBeAliens", "Input validation failed: 404 Aliens not found")]
		public string aDropdownStr = "Aliens";
		
		private string[] enumElements = new string[] { "Jambalaya", "FooBar", "GengisKhan", "Aliens" };

		private bool IsNotAliens() => aDropdownStr != "Aliens";
		private bool CantBeAliens(string param) => param != "Aliens";

		[MinMaxSlider(-3.14f, 3.14f)]
		public Vector2 twoValues;

		[BoxGroup("Min and Max Values")]
		[MinValue(-3.14f), MaxValue(3.14f)]
		public float intornoPi2;
		#endregion

		[Section("Section 42")]
		[ReadOnly]
		public bool readOnly;

		[Required]
		public Transform required;

		[ShowIf("IsRequiredTransformMe")]
		[ResizableTextArea]
		public string showIf = "you found the magic variable";

		private bool IsRequiredTransformMe() => required == this.transform;
		
		[ShowAssetPreview]
		public GameObject go;

		[ShowNativeProperty]
		public int transformCount {
			get {
				return this.transform.childCount;
			}
		}


	}
}
