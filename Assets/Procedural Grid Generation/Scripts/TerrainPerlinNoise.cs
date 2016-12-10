using UnityEngine;
using System.Collections;
using System.Linq;

public class TerrainPerlinNoise : MonoBehaviour {

	public float width = 1000f, height = 1000f;
	public int scale = 1000;

	public float perlinNoiseTranslation = 0;

	// Use this for initialization
	void Start () {
		CreateMesh();
	}

	protected void CreateMesh(){
		this.gameObject.AddComponent<MeshFilter>();
		this.gameObject.AddComponent<MeshRenderer>();

		Mesh mesh = new Mesh();

		int rows = (int)(height/scale);
		int cols = (int)(width/scale);

		print("rows "+ rows);
		print("cols "+ cols);

		Vector3[] vertices = new Vector3[rows*cols*4];
		Vector2[] newUV = new Vector2[rows*cols*4];
		Vector3[] normals = new Vector3[rows*cols*4];

		int[] tri = new int[rows*cols*6];

		int currVertex = 0;

		for (int y = 0; y < cols-1; y++) {
			for (int x = 0; x < rows-1; x++) {

				vertices[currVertex] = new Vector3(x*scale, y*scale, 0);
				vertices[currVertex+1] = new Vector3((x+1)*scale, y*scale, 0);
				vertices[currVertex+2] = new Vector3(x*scale, (y+1)*scale, 0);
				vertices[currVertex+3] = new Vector3((x+1)*scale, (y+1)*scale, 0);

				/*
				print("x*scale "+ x*scale);
				print("(x+1)*scale "+ ((x+1)*scale));
				print("y*scale "+ y*scale);
				print("(y+1)*scale "+ ((y+1)*scale));

				print("Vectors");
				vertices.ToList()
					.ForEach(v=>Debug.Log(v));
				*/




				//  Lower left triangle.
				tri[currVertex] = currVertex;
				tri[currVertex+1] = currVertex+2;
				tri[currVertex+2] = currVertex+1;

				//  Upper right triangle.   
				tri[currVertex+3] = currVertex+2;
				tri[currVertex+4] = currVertex+3;
				tri[currVertex+5] = currVertex+1;

				currVertex += 4;
			}
			print("row "+y+" done");
		}



		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = vertices;
		mesh.uv = newUV;
		mesh.normals = normals;
		mesh.triangles = tri;
	}




}
