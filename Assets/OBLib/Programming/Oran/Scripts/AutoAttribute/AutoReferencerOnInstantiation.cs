using UnityEngine;
using BarbarO.Attributes.ScriptTiming;

[ScriptTiming(-900)]
public class AutoReferencerOnInstantiation : MonoBehaviour {

    public bool alsoReferenceChildren = true;

	void Awake() 
    {
		AutoAttributeManager.AutoReference(this.gameObject, out int succ, out int fail);

        if(alsoReferenceChildren)
        {
            foreach(Transform child in this.transform)
            {
                AutoAttributeManager.AutoReference(child.gameObject, out succ, out fail);
            }
        }
	}

    private void RecursivelyReferenceChildren(GameObject go)
    {
        AutoAttributeManager.AutoReference(go, out int succ, out int fail);

        foreach(Transform child in this.transform)
        {
            RecursivelyReferenceChildren(child.gameObject);  
        }
    }

}
