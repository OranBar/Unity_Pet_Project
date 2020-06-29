/* Author: Oran Bar
 * Summary: This attribute automatically assigns a class variable to one of the gameobject's component. 
 * It basically acts as an automatic GetComponentInChildren on Awake.
 * Using [AutoChildren(true)], the behaviour can be extendend to act like an AddOrGetComponent: The component will be added if it is not found, instead of an error being thrown.
 * 
 * usage example
 * 
 * public class Foo
 * {
 *		[AutoChildren] public BoxCollier myBoxCollier;	//This assigns the variable to the BoxColider attached on your object
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
public class AutoChildrenAttribute : Attribute, IAutoAttribute
{
	private const string MonoBehaviourNameColor = "green";

	private bool logErrorIfMissing = true;

	private Component targetComponent;

	public AutoChildrenAttribute(bool autoAdd)
	{
		this.logErrorIfMissing = true;
	}

	public AutoChildrenAttribute(bool autoAdd = false, bool getMadIfMissing = true)
	{
		this.logErrorIfMissing = getMadIfMissing;
	}

	public void Execute(MonoBehaviour mb, FieldInfo field)
	{
		GameObject go = mb.gameObject;

		Type componentType = field.FieldType;

		Component componentToReference = go.GetComponentInChildren(componentType, true);
		if (componentToReference == null)
		{
			if (logErrorIfMissing)
			{
				Debug.LogError(
					string.Format("[Auto]: <color={3}><b>{1}</b></color> couldn't find <color=#cc3300><b>{0}</b></color> on <color=#e68a00>{2}</color>",
						componentType.Name, mb.GetType().Name, go.name, MonoBehaviourNameColor)
					, go);
			}
			return;
		}

		field.SetValue(mb, componentToReference);
	}

	public void Execute(MonoBehaviour mb, PropertyInfo prop)
	{
		GameObject go = mb.gameObject;

		Type componentType = prop.PropertyType;

		Component componentToReference = go.GetComponent(componentType);
		if (componentToReference == null)
		{
			if (logErrorIfMissing)
			{
				Debug.LogError(string.Format("[Auto] Error: Script {1} couldn't AutoReference component {0} of GameObject {2}",
					componentType.Name, mb.GetType().Name, go.name), go);
				return;
			}
		}

		prop.SetValue(mb, componentToReference, null);
	}

}