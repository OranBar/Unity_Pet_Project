/* Author: Oran Bar
 * Summary: This attribute automatically assigns a class variable to one of the gameobject's component. 
 * It basically acts as an automatic GetComponent on Awake.
 * Using [Auto(true)], the behaviour can be extendend to act like an AddOrGetComponent: The component will be added if it is not found, instead of an error being thrown.
 * 
 * usage example
 * 
 * public class Foo{
 *		[Auto]public BoxCollier myBoxCollier;
 *		
 *		//Methods...
 * }
 * 
 * Copyrights to Oran Bar™
 */
 
using System;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class AutoAttribute : Attribute {

	private const string MonoBehaviourNameColor = "green";

	private bool autoAdd;

	private Component targetComponent;

	public AutoAttribute()
	{
		autoAdd = false;
	}

	public AutoAttribute(bool autoAdd)
	{
		this.autoAdd = autoAdd;
	}

	public void Execute(MonoBehaviour mb, FieldInfo field)
	{
		//Execute(mb, field.FieldType, field.Name);
		GameObject go = mb.gameObject;

		Type componentType = field.FieldType;

		//if (field.IsPrivate && field.GetCustomAttributes(typeof(SerializeField), true).Length == 0)
		//{
		//	Debug.LogErrorFormat("[Auto]: Variable {0} can't be auto referenced. Either make the {0} public, or add [SerializeField] to it", field.Name);
		//	return;
		//}

		Component componentToReference = go.GetComponent(componentType);
		if (componentToReference == null)
		{
			if (autoAdd)
			{
				Debug.LogWarning(string.Format("[Auto]: <color={3}><b>{1}</b></color> automatically added component <color=#cc3300><b>{0}</b></color> on <color=#e68a00>{1}</color>",
					componentType.Name, mb.GetType().Name, go.name, MonoBehaviourNameColor)
					, go);
				componentToReference = mb.gameObject.AddComponent(componentType);
			}
			else
			{
				Debug.LogError(
					string.Format("[Auto]: <color={3}><b>{1}</b></color> couldn't find <color=#cc3300><b>{0}</b></color> on <color=#e68a00>{2}</color>",
						componentType.Name, mb.GetType().Name, go.name, MonoBehaviourNameColor)
					, go);
				return;
			}
		}

		field.SetValue(mb, componentToReference);
	}

	public void Execute(MonoBehaviour mb, PropertyInfo prop)
	{
		//Execute(mb, prop.PropertyType, prop.Name);
		GameObject go = mb.gameObject;

		Type componentType = prop.PropertyType;

		Component componentToReference = go.GetComponent(componentType);
		if (componentToReference == null)
		{
			if (autoAdd)
			{
				Debug.LogWarning(string.Format("[Auto] Warning: Script {1} automatically added component {0} on GameObject {1}",
					componentType.Name, mb.GetType().Name, go.name), go);
				componentToReference = mb.gameObject.AddComponent(componentType);
			}
			else
			{
				Debug.LogError(string.Format("[Auto] Error: Script {1} couldn't AutoReference component {0} of GameObject {2}",
					componentType.Name, mb.GetType().Name, go.name), go);
				return;
			}
		}

		prop.SetValue(mb, componentToReference, null);
	}

	//public void Execute(MonoBehaviour mb, Type varType, string varName)
	//{
	//	GameObject go = mb.gameObject;

	//	Type componentType = varType;
		
	//	Component componentToReference = go.GetComponent(componentType);
	//	if (componentToReference == null)
	//	{
	//		if (autoAdd)
	//		{
	//			Debug.LogWarning(string.Format("[Auto] Warning: Script {1} automatically added component {0} on GameObject {1}",
	//				componentType.Name, mb.GetType().Name, go.name), go);
	//			componentToReference = mb.gameObject.AddComponent(componentType);
	//		}
	//		else
	//		{
	//			Debug.LogError(string.Format("[Auto] Error: Script {1} couldn't AutoReference component {0} of GameObject {2}",
	//				componentType.Name, mb.GetType().Name, go.name), go); 
	//			return;
	//		}
	//	} 

	//	var castedMb = new SerializedObject(mb);
	//	var myProperty = castedMb.FindProperty(varName);
	//	myProperty.objectReferenceValue = componentToReference;
	//	castedMb.ApplyModifiedProperties();
	//} 

}