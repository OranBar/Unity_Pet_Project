using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class OnClickMergerFractal : MonoBehaviour {

	public Material boxMaterial;

	private List<GameObject> cubes = new List<GameObject>();
	private int noOfClicks = 0;

	// Use this for initialization
	void Start () {
		GameObject cube = CreateNewCube();
		/*
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.parent = this.transform;
		cube.GetComponent<Renderer>().material = boxMaterial;
		cube.isStatic = true;
		Destroy(cube.GetComponent<Collider>());
		*/
		cube.transform.position = Vector3.zero;
		cubes.Add(cube);
	}

	private void FractalStep(){
		List<GameObject> newCubeList = new List<GameObject>();
		foreach(GameObject cube in cubes){
			for(int x=-1; x<=1; x++){
				for(int y=-1; y<=1; y++){
					for(int z=-1; z<=1; z++){
						if(Mathf.Abs(x)+Mathf.Abs(y)+Mathf.Abs(z) <= 1){
							continue;
							//It's a middle box. Skip it
						}
						GameObject newCube = CreateNewCube();
						/*
						GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						newCube.GetComponent<Renderer>().material = boxMaterial;
						newCube.isStatic = true;
						Destroy(newCube.GetComponent<Collider>());
						newCube.transform.parent = this.transform;
						*/
						float scaleDown = cube.transform.localScale.x/3;
						newCube.transform.localScale = Vector3.one * scaleDown;
						newCube.transform.position = 
							cube.transform.position + new Vector3(x, y, z) * scaleDown;
						newCubeList.Add(newCube);

					}
				}
			}	
			Destroy(cube);
		}
		cubes = newCubeList;

	}

	private GameObject CreateNewCube(){
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.parent = this.transform;
		cube.GetComponent<Renderer>().material = boxMaterial;
		cube.isStatic = true;
		Destroy(cube.GetComponent<Collider>());

		return cube;
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1")){
			if(++noOfClicks >= 3){
				Debug.LogWarning("A further step may overload the program and lead to a crash " +
					"because of the excessive number of objects. If you really want to risk it, use " +
					"the right mouse button");
				return;
			}
			FractalStep();
		}
		else if(Input.GetButtonDown("Fire2")){
			FractalStep();
		}
	}
}
