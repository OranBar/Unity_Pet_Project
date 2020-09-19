using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.Assertions;
using BarbarO.Utils.Reflection;

public static class ComponentEx
{
    #region GetComponent
    /// <summary>
    /// This method will immediately throw an exception if the component is not found. Use this when the T component is necessary for the correct execution of the program, 
    /// as opposed to a normal GetComponent with a null check
    /// </summary>														
	public static T GetRequiredComponent<T>(this Component thisComponent) where T : class
    {
		Debug.Assert(thisComponent != null);
        var retrievedComponent = thisComponent.GetComponent<T>();
        Assert.IsNotNull(retrievedComponent,
            string.Format("Script {1} on GameObject \"{0}\" does not have the required component of type {2}", thisComponent.name, thisComponent.GetType(), typeof(T)));

        return retrievedComponent;
    }
    /// <summary>
    /// This method will immediately throw an exception if no component is found. Use this when the T component is necessary for the correct execution of the program, 
    /// as opposed to a normal GetComponent with a null check
    /// </summary>														
	public static T GetRequiredComponentInChildren<T>(this Component thisComponent) where T : class
    {
        var retrievedComponent = thisComponent.GetComponentInChildren<T>();
        Assert.IsNotNull(retrievedComponent,
            string.Format("Script {1} on GameObject \"{0}\" does not have a child with component of type {2}", thisComponent.name, thisComponent.GetType(), typeof(T)));

        return retrievedComponent;
    }

    /// <summary>
    /// This method will immediately throw an exception if no component is found. Use this when the T component is necessary for the correct execution of the program, 
    /// as opposed to a normal GetComponent with a null check
    /// </summary>														
	public static T GetRequiredComponentInParent<T>(this Component thisComponent) where T : class
    {
        var retrievedComponent = thisComponent.GetComponentInParent<T>();
        Assert.IsNotNull(retrievedComponent,
            string.Format("Script {1} on GameObject \"{0}\" does not have a parent with component of type {2}", thisComponent.name, thisComponent.GetType(), typeof(T)));

        return retrievedComponent;
    }
    #endregion
}

namespace OranUnityUtils
{
	public static class ComponentEx1 {

		public static T GetCopyOf<T>(this Component comp, T other) where T : Component
		{ 
			Debug.Assert(comp != null);

			Type type = comp.GetType();
			if (type != other.GetType()) return null; // type mis-match
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
			PropertyInfo[] pinfos = type.GetProperties(flags);
			foreach (var pinfo in pinfos) {
				if (pinfo.CanWrite) {
					try {
						pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
					}
					catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
				}
			}
			FieldInfo[] finfos = type.GetFields(flags);
			foreach (var finfo in finfos) {
				finfo.SetValue(comp, finfo.GetValue(other));
			}
			return comp as T;
		}
		
		public static bool ValuesEqual<T>(this T component1, T component2) where T : Component
		{
			ReflectionHelperMethods reflectionHelper = new ReflectionHelperMethods();

			foreach(var variable in reflectionHelper.GetAllVariables(typeof(T)))
			{
				//Early Skip
				if (variable.Name == "name" || variable.Name == "hideFlags" || variable.Name == "tag")
				{
					continue;
				}

				var component1Value = Convert.ChangeType(reflectionHelper.GetValue(variable, component1), reflectionHelper.GetVariableType(variable));
				var component2Value = Convert.ChangeType(reflectionHelper.GetValue(variable, component2), reflectionHelper.GetVariableType(variable));


				//component1Value = Convert.ChangeType(component1Value, reflectionHelper.GetVariableType(variable));
				//component2Value = Convert.ChangeType(component2Value, reflectionHelper.GetVariableType(variable));

				if(component1Value == null && component2Value != null)
				{
					return false;
				}

				if (component1Value != null && component1Value.Equals(component2Value)==false)
				{
					Debug.Log("Found discrepancy with variable "+variable.Name+". First value was "+component1Value+", while second was "+ component2Value);
					return false;
				}

			}
			return true;
		}
    }
}
