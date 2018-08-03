using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using Pixelplacement.TweenSystem;

public class TweenTest : MonoBehaviour {

	TweenBase t1;
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			t1?.Cancel();
			Vector3 startPosition = this.transform.position;
			Vector3 targetPosition = this.transform.position + new Vector3(0, 1.5f, 0f);
			t1 = Tween.Position(this.transform, targetPosition, 10f, 0.5f);
		}
	}

	void OnGUI()
	{
		if (GUILayout.Button("Start")) t1.Start();
		if (GUILayout.Button("Cancel")) t1.Cancel();
		if (GUILayout.Button("Finish")) t1.Finish();
		if (GUILayout.Button("Rewind")) t1.Rewind();
	}

}
