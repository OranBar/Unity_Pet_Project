using System.Linq;
using UnityEngine;
using System.Reflection;
using BarbarO.Utils.Reflection;
using BarbarO.Attributes.ScriptTiming;

#if UNITY_EDITOR
#endif

[ScriptTiming(-100)]
[ExecuteInEditMode]
public class EditorUpdate : MonoBehaviour {

	private void Awake()
	{
		if(Application.isPlaying == true)
		{
			this.enabled = false;
		}
	}

	#if UNITY_EDITOR	
	void Update()
	{	
		if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode == false)
		{
			DestroyDuplicatesOn(this.gameObject);

			//Call Editor Update
			MonoBehaviour[] monoBehaviours = this.GetComponents<MonoBehaviour>();
			foreach (MonoBehaviour mb in monoBehaviours)
			{
				var method = FindEditorUpdateMethod(mb);
				
				if (method != null)
				{
					method.Invoke(mb, null);
				}
			}
		}
	}

	private void DestroyDuplicatesOn(GameObject target)
	{
		if (target.GetComponents<EditorUpdate>().Length > 1)
		{
			target.GetComponents<EditorUpdate>().Skip(1).ForEach(eu => DestroyImmediate(eu));
		}
	}

	private MethodInfo FindEditorUpdateMethod(MonoBehaviour mb)
	{
		MethodInfo method = null;
		method = mb.GetType().GetMethod("EditorUpdate", BindingFlags.Public | BindingFlags.Instance);
		if (method == null)
		{
			//let's try to see if it's a private method declared in one of the parent classes
			ReflectionHelperMethods rhm = new ReflectionHelperMethods();
			method = rhm.GetNonPublicMethodInBaseClasses(mb.GetType(), "EditorUpdate");
		}

		return method;
	}
	#endif
}

