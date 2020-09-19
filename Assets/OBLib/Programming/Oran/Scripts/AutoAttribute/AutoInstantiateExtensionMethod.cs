using UnityEngine;

public static class AutoInstantiateExtensionMethod 
{
    public static GameObject Instantiate_And_AssignAutoVariables(this MonoBehaviour mb, GameObject original){
		GameObject newGo = GameObject.Instantiate(original);
		AutoAttributeManager.AutoReference(newGo);
		return newGo;
	}

	public static GameObject Instantiate_And_AssignAutoVariables(this MonoBehaviour mb, GameObject original, Transform parent){
		GameObject newGo = GameObject.Instantiate(original, parent);
		AutoAttributeManager.AutoReference(newGo);
		return newGo;
	}

	public static GameObject Instantiate_And_AssignAutoVariables(this MonoBehaviour mb, GameObject original, Vector3 position , Quaternion rotation){
		GameObject newGo = GameObject.Instantiate(original, position, rotation);
		AutoAttributeManager.AutoReference(newGo);
		return newGo;
	}

	public static GameObject Instantiate_And_AssignAutoVariables(this MonoBehaviour mb, GameObject original, Vector3 position , Quaternion rotation, Transform parent){
		GameObject newGo = GameObject.Instantiate(original, position, rotation, parent);
		AutoAttributeManager.AutoReference(newGo);
		return newGo;
	}

	public static GameObject Instantiate_And_AssignAutoVariables(this MonoBehaviour mb, GameObject original, Transform parent, bool instantiateInWorldSpace){
		GameObject newGo = GameObject.Instantiate(original, parent, instantiateInWorldSpace);
		AutoAttributeManager.AutoReference(newGo);
		return newGo;
	}
}
