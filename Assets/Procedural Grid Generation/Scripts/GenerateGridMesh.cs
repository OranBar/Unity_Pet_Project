using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateGridMesh : MonoBehaviour {

	public int xSize, ySize;
	public Color GizmoColor = Color.black;

	private Vector3[] vertices;
	private Mesh mesh;

	void Awake(){
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.name = "Procedural Grid";
		StartCoroutine(Generate());
	}

	public IEnumerator Generate(){
		yield return StartCoroutine(GenerateVerticesAndUvs());
		yield return StartCoroutine(GenerateTriangles());
		mesh.RecalculateNormals();
	}

	private IEnumerator GenerateVerticesAndUvs(){
		vertices = new Vector3[(xSize+1) * (ySize+1)];
		Vector2[] uvs = new Vector2[vertices.Length];
		Vector4[] tangents = new Vector4[vertices.Length];
		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

		int i = 0;
		for (int y = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++) {
				vertices[i] = new Vector3(x,y);
				uvs[i] = new Vector2((float)x/xSize, (float)y/ySize);
				tangents[i] = tangent;
				i++;
				yield return new WaitForSeconds(0.02f);
			}
		}

		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.tangents = tangents;
		yield return null;
	}

	public IEnumerator GenerateTriangles(){

		int[] triangles = new int[ySize * xSize*6];

		for(int y=0, ti=0, vi=0; y < ySize; y++, vi++){
			for(int x=0; x < xSize; x++, ti+=6, vi++){
				triangles[ti] = vi+0;
				triangles[ti+1] = triangles[ti+4] = xSize +vi+1;
				triangles[ti+2] = triangles[ti+3] = vi+1;
				triangles[ti+5] = xSize +vi+2;
				mesh.triangles = triangles;
				yield return new WaitForSeconds(0.05f);
			}
		}
		yield return null;
	}


	private void OnDrawGizmos(){
		if(vertices == null){
			return;
		}
		Gizmos.color = GizmoColor;
		for (int i = 0; i < vertices.Length; i++) {
			Gizmos.DrawSphere(this.transform.position + vertices[i], 0.1f);
		}
	}
}