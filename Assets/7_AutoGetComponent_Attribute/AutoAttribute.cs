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
		GameObject go = mb.gameObject;

		Type componentType = field.FieldType;

		Component componentToReference = go.GetComponent(componentType);

		if (componentToReference == null)
		{
			if (autoAdd)
			{
				Debug.LogWarning(string.Format("[Auto] Warning: Automatically added component {0} from GameObject {1}",
					componentType.Name, go.name), go);
				componentToReference = mb.gameObject.AddComponent(componentType);
			}
			else
			{
				Debug.LogError(string.Format("[Auto] Error: Couldn't AutoReference the component {0} from GameObject {1}",
					componentType.Name, go.name), go);
				return;
			}
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
			if (autoAdd)
			{
				componentToReference = mb.gameObject.AddComponent(componentType);
			}
			else
			{
				Debug.LogError(string.Format("[Auto] Error: Add component {0} to GameObject {1} ",
					componentType.Name, go.name), go);
				return;
			}
		}

		prop.SetValue(mb, componentToReference, null);
	}
}
