using UnityEngine;
using System.Collections;
using UnityEditor;

public class RandomMovement : MonoBehaviour {

	public int changeDirectionFrequencyInFrames = 60;
	public float speed = 1;

	private Vector3 currTarget;

	private int frame = 0;

	public void Update(){
		unchecked{
			frame++;
		}
		if(frame%changeDirectionFrequencyInFrames == 0){
			
			currTarget = new Vector3(Random.Range(2, 100), 
				Random.Range(2, 100),
				Random.Range(2, 100));
		}

		Vector3 dirToTarget = (currTarget - this.transform.position).normalized;
		this.transform.position = this.transform.position + dirToTarget * speed;


	}


}
