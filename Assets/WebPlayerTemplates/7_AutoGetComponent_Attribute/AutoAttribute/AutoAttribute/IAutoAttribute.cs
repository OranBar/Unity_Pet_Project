using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public interface IAutoAttribute {

	void Execute(MonoBehaviour mb, FieldInfo field);
	void Execute(MonoBehaviour mb, PropertyInfo prop);

}
