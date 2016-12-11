using UnityEngine;
using System.Collections;

public class WireframeRendering : MonoBehaviour {

	public bool enable = true;

	void OnPreRender() {
		GL.wireframe = enable;
	}

	void OnPostRender() {
		GL.wireframe = false;
	}
}
