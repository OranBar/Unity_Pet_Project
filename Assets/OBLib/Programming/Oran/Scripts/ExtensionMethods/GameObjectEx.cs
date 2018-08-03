﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using OranUnityUtils;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

public static class GameObjectEx
{

	/// <summary>
	/// Same as GameObject.FindObjectsOfType<T>, but also get inactive objects
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static T[] FindAllOfType<T>() where T : UnityEngine.Object
	{
		if (typeof(T) == typeof(GameObject))
		{
			throw new Exception("Use Transform instead of GameObject when calling GameObjectEx.FindAllOfType<>");
		}

		return Resources.FindObjectsOfTypeAll<T>()
			.Where(obj => obj.GameObject().scene == SceneManager.GetActiveScene())
			.ToArray();
	}

	/// <summary>
	/// Generic version of Instantiate
	/// </summary>					  
	public static T Instantiate<T>(T prefab) where T : Component
	{
		return (T)UnityEngine.Object.Instantiate(prefab);
	}

	/// <summary>
	/// Generic version of Instantiate
	/// </summary>					  
	public static T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
	{
		var newObj = Instantiate<T>(prefab);

		newObj.transform.position = position;
		newObj.transform.rotation = rotation;

		return newObj;
	}

	/// <summary>
	/// If a component of type T is already attached to the target go, that component is returned. Else, the component is added. 
	/// Useful to avoid duplication of the same component on objects when adding new components, since usually only one is needed.
	/// </summary>
	public static T AddOrGetComponent<T>(this GameObject go, out bool wasComponentAdded) where T : Component
	{
		T myComponent = go.GetComponent<T>();
		wasComponentAdded = myComponent == null;
		return (myComponent != null) ? myComponent : go.AddComponent<T>();
	}

	public static Component AddOrGetComponent(this GameObject go, Type componentType, out bool wasComponentAdded)
	{
		Component myComponent = go.GetComponent(componentType);
		wasComponentAdded = myComponent == null;
		return (myComponent != null) ? myComponent : go.AddComponent(componentType);
	}


	/// <summary>
	/// If a component of type T is already attached to the target go, that component is returned. Else, the component is added. 
	/// Useful to avoid duplication of the same component on objects when adding new components, since usually only one is needed.
	/// </summary>
	//[System.Obsolete("Use AddOrGetComponent")]
	public static T GetOrAddComponent<T>(this GameObject go) where T : Component
	{
		T myComponent = go.GetComponent<T>();
		return (myComponent != null) ? myComponent : go.AddComponent<T>();
	}

	/// <summary>
	/// If a component of type T is already attached to the target go, that component is returned. Else, the component is added. 
	/// Useful to avoid duplication of the same component on objects when adding new components, since usually only one is needed.
	/// </summary>
	public static T AddOrGetComponent<T>(this GameObject go) where T : Component
	{
		bool wasComponentAdded;
		return go.AddOrGetComponent<T>(out wasComponentAdded);
	}


	public static Component AddOrGetComponent(this GameObject go, Type componentType)
	{
		Component myComponent = go.GetComponent(componentType);
		return (myComponent != null) ? myComponent : go.AddComponent(componentType);
	}


	/// <summary>com
	/// Adds to the target gameobject a copy of the target component
	/// </summary>												   
	public static T AddComponentCopy<T>(this GameObject go, T toCopy) where T : Component
	{
		if (go.GetComponent<T>() != null)
		{
			Debug.LogErrorFormat(go, "Can't add component of type {0} to GameObject {1}: Component is already there", typeof(T), go.name);
		}
		return go.AddComponent<T>().GetCopyOf(toCopy) as T;
	}

	#region GetComponent

	public static Component GetComponent(this GameObject go, Type componentType, bool includeInactiveObjects)
	{
		foreach (var component in go.GetComponentsInChildren(componentType, includeInactiveObjects))
		{
			if (component.GameObject() != go)
			{
				break;
			}
			return component;
		}
		return null;
	}

	public static T GetComponent<T>(this GameObject go, bool includeInactiveObjects)
	{
		//if (includeInactiveObjects == false)
		//{
		//	Debug.LogError("Go to hell");
		//}

		if (go.activeSelf)
		{
			return go.GetComponent<T>();
		}

		T result = default(T);
		foreach (var component in go.GetComponentsInChildren<T>(includeInactiveObjects))
		{
			if (component.GameObject() != go)
			{
				break;
			}
			if (component is T)
			{
				result = (T)component;
				break;
				//return (T) component;
			}
		}

		return result;
	}


	public static T[] GetComponents<T>(this GameObject go, bool includeInactiveObjects) where T : Component
	{
		List<T> result = new List<T>();
		foreach (var component in go.GetComponentsInChildren<T>(includeInactiveObjects))
		{
			//If a monoBehaviour is missing, the component can be null
			if (component == null)
			{ continue; }


			if (component.GameObject() != go)
			{
				break;
			}
			if (component is T)
			{
				result.Add((T)component);
			}
		}
		return result.ToArray();
	}

	public static Component GetComponentInParent(this GameObject go, Type componentType, bool includeInactiveObjects)
	{
		Component result = null;

		do
		{
			result = go.GetComponent(componentType, true);
			go = go.transform.parent.IfNotNull(p => p.gameObject);
		} while (result == null && go != null);

		return result;
	}

	public static Component GetComponentInParent<T>(this GameObject go, bool includeInactiveObjects) where T : Component
	{
		return go.GetComponentInParent(typeof(T), includeInactiveObjects);
	}

	/// <summary>
	/// This method will immediately throw an exception if the component is not found. Use this when the T component is necessary for the correct execution of the program, 
	/// as opposed to a normal GetComponent with a null check
	/// </summary>														
	public static T GetRequiredComponent<T>(this GameObject thisGameobject, bool includeInactiveObjects = false)
	{
		var retrievedComponent = thisGameobject.GetComponent<T>(includeInactiveObjects);
		if (retrievedComponent is Component == false)
		{
			string msg = string.Format("Script {1} on GameObject \"{0}\" does not have the required component of type {2}", thisGameobject.name, typeof(T).Name, typeof(T));
			throw new Exception(msg);
		}
		//Debug.Assert(retrievedComponent is Component,
		//	string.Format("Script {1} on GameObject \"{0}\" does not have the required component of type {2}", thisGameobject.name, thisGameobject.GetType(), typeof(T)), thisGameobject);

		return retrievedComponent;
	}
	/// <summary>
	/// This method will immediately throw an exception if no component is found. Use this when the T component is necessary for the correct execution of the program, 
	/// as opposed to a normal GetComponent with a null check
	/// </summary>														
	public static T GetRequiredComponentInChildren<T>(this GameObject thisGameobject, bool includeInactiveObjects = false)
	{
		var retrievedComponent = thisGameobject.GetComponentInChildren<T>(includeInactiveObjects);
		if (retrievedComponent != null)
		{
			string msg = string.Format("Script {1} on GameObject \"{0}\" does not have a child with component of type {2}", thisGameobject.name, thisGameobject.GetType(), typeof(T));
			throw new Exception(msg);
		}
		//Assert.IsNotNull(retrievedComponent as Component,
		//	string.Format("Script {1} on GameObject \"{0}\" (NetId={3}) does not have a child with component of type {2}", thisGameobject.name, thisGameobject.GetType(), typeof(T), thisGameobject.GetNetId());

		return retrievedComponent;
	}

	/// <summary>
	/// This method will immediately throw an exception if no component is found. Use this when the T component is necessary for the correct execution of the program, 
	/// as opposed to a normal GetComponent with a null check
	/// </summary>														
	public static T GetRequiredComponentInParent<T>(this GameObject thisGameobject)
	{
		var retrievedComponent = thisGameobject.GetComponentInParent<T>();
		if (retrievedComponent == null)
		{
			string msg = string.Format("Script {1} on GameObject \"{0}\" does not have a parent with component of type {2}", thisGameobject.name, thisGameobject.GetType(), typeof(T));
			throw new Exception(msg);
		}
		//Assert.IsNotNull(retrievedComponent as Component,
		//    string.Format("Script {1} on GameObject \"{0}\" (NetId={3}) does not have a parent with component of type {2}", thisGameobject.name, thisGameobject.GetType(), typeof(T), thisGameobject.GetNetId());

		return retrievedComponent;
	}

	public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag, bool forceActive = false) where T : Component
	{
		if (parent == null) { throw new System.ArgumentNullException(); }
		if (string.IsNullOrEmpty(tag) == true) { throw new System.ArgumentNullException(); }

		T[] list = parent.GetComponentsInChildren<T>(forceActive);
		foreach (T t in list)
		{
			if (t.CompareTag(tag) == true)
			{
				return t;
			}
		}
		return null;
	}

	#endregion
}


namespace OranUnityUtils
{
	public static class GameObjectEx1
	{

		/// <summary>
		/// This method returns the first component in parent excluding the one who called it.
		/// </summary>
		public static T GetComponentInParentExceptThis<T>(this GameObject go) where T : Component
		{
			if (go.transform.parent == null)
			{
				Debug.LogWarning("There are no parents");
				return null;
			}

			return go.transform.parent.GetComponentInParent<T>();
		}

		/// <summary>
		/// This method returns the first component in children excluding the one who called it.
		/// </summary>
		public static T GetComponentInChildrenExceptThis<T>(this GameObject go) where T : Component
		{
			T[] components = go.GetComponentsInChildren<T>().ToArray();
			T component = null;
			for (int i = 0; i < components.Length; i++)
			{
				if (components[i].gameObject == go) continue;
				else { component = components[i]; break; }
			}
			return component;
		}

		/// <summary>
		/// This method returns all components in parent excluding the one who called it.
		/// </summary>
		public static T[] GetComponentsInParentExceptThis<T>(this GameObject go) where T : Component
		{
			if (go.transform.parent == null)
			{
				Debug.LogWarning("There are no parents");
				return null;
			}

			return go.transform.parent.GetComponentsInParent<T>();
		}

		/// <summary>
		/// This method returns all components in children excluding the one who called it.
		/// </summary>
		public static T[] GetComponentsInChildrenExceptThis<T>(this GameObject go) where T : Component
		{
			return go.GetComponentsInChildren<T>().Where(myGo => myGo.gameObject != go).ToArray();
		}

		/// <summary>
		/// This method gets a MonoBehaviour from a GameObject. Useful if you need to call MonoBehaviour functions that need to be executed from target gameobject: (i.e. Coroutines, Destroy, ...)
		/// </summary>
		public static MonoBehaviour GetMonoBehaviour(this GameObject go)
		{
			if (go.GetComponent<MonoBehaviour>() == null)
			{
				return go.AddComponent<CoroutineHelper>();
			}
			else
			{
				return go.GetComponent<MonoBehaviour>();
			}
		}

		/// <summary>
		/// Execute an action with delay. The action can be passed as: Method group, delegate or lambda. 
		/// </summary>
		public static void ExecuteDelayed(this GameObject go, Action action, float delay)
		{
			go.AddOrGetComponent<CoroutineHelper>().StartCoroutineTimeline(
				go.WaitForSeconds_Coro(delay),
				go.ToIEnum(action)
			);
		}

		/// <summary>
		/// Execute an action with delay of 1 FixedUpdate frame. The action can be passed as: Method group, delegate or lambda. 
		/// </summary>
		public static void ExecuteDelayedFixedUpdate(this GameObject go, Action action)
		{
			go.AddOrGetComponent<CoroutineHelper>().StartCoroutineTimeline(
				go.WaitForSeconds_Coro(Time.fixedDeltaTime),
				go.ToIEnum(action)
			);
		}




		public static void ChangeLayer(this GameObject go, string newLayer, bool withChildren = false)
		{
			ChangeLayer(go, LayerMask.NameToLayer(newLayer), withChildren);
		}

		public static void ChangeLayer(this GameObject go, int newLayer, bool withChildren = false)
		{
			go.layer = newLayer;
			if (withChildren)
			{
				Transform[] children = go.transform.GetChildren().ToArray();
				for (int i = 0; i < children.Length; i++)
				{
					children[i].gameObject.layer = newLayer;
				}
			}

		}

		#region Coroutines: SuperCoroutine Timeline and SuperCoroutine
		/// <summary>
		/// Starts a SuperCoroutine timeline, executing each routine in order.
		/// Each routine waits for the previous one to finish before executing
		/// </summary>
		/// <returns>The SuperCoroutine timeline.</returns>
		/// <param name="routines">Routines.</param>
		public static Coroutine StartCoroutineTimeline(this GameObject go, params IEnumerator[] routines)
		{
			return go.AddOrGetComponent<CoroutineHelper>().StartCoroutine(go.StartCoroutineTimeline_Coro(routines));
		}

		private static IEnumerator StartCoroutineTimeline_Coro(this GameObject go, params IEnumerator[] routines)
		{
			foreach (IEnumerator currentRoutine in routines)
			{
				yield return go.AddOrGetComponent<CoroutineHelper>().StartCoroutine(currentRoutine);
			}
		}

		public static Coroutine StartCoroutine(this GameObject go, IEnumerator routine)
		{
			return go.AddOrGetComponent<CoroutineHelper>().StartCoroutine(routine);
		}

		public static StoppableCoroutine StartStoppableCoroutine(this GameObject go, IEnumerator routine)
		{
			return go.AddOrGetComponent<CoroutineHelper>().StartStoppableCoroutine(routine);
		}

		#endregion


	}
}
