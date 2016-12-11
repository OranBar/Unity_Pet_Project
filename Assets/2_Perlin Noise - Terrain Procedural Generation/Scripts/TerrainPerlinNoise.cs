using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainPerlinNoise : MonoBehaviour {

	public int width = 1000, height = 1000;
	public int scale = 100;
	public float flyingSpeed = 0.1f;
	public float perlinNoiseOffset = 0.2f;
	public float maxZHeight;

	public int updateFrequency = 2;

	private int updateIter = 0;
	private float currentPerlinNoiseOffset = 0f;
	private int xSize, ySize;
	public Color GizmoColor = Color.black;

	private Vector3[] vertices;
	private Mesh mesh;

	void Awake(){
		xSize = width / scale;
		ySize = height / scale;

		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.name = "Procedural Grid";
		StartCoroutine(Generate());
	}

	public IEnumerator Generate(){
		vertices = new Vector3[(xSize+1) * (ySize+1)];
		Vector2[] uvs = new Vector2[vertices.Length];
		Vector4[] tangents = new Vector4[vertices.Length];
		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

		int i = 0;
		for (int y = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++) {
				vertices[i] = new Vector3(x*scale,y*scale);
				uvs[i] = new Vector2((float)x/xSize, (float)y/ySize);
				tangents[i] = tangent;
				i++;
			}
		}

		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.tangents = tangents;

		int[] triangles = new int[ySize * xSize*6];

		for(int y=0, ti=0, vi=0; y < ySize; y++, vi++){
			for(int x=0; x < xSize; x++, ti+=6, vi++){
				triangles[ti] = vi+0;
				triangles[ti+1] = triangles[ti+4] = xSize +vi+1;
				triangles[ti+2] = triangles[ti+3] = vi+1;
				triangles[ti+5] = xSize +vi+2;
				mesh.triangles = triangles;
			}
		}
		mesh.RecalculateNormals();
		yield return null;
	}

	public void Update(){
		unchecked{
			updateIter++;
		}
		if(updateIter%updateFrequency == 0){
			UpdateMesh();
		}
	}

//	void draw() {
//
//	flying -= 0.1;
//
//	float yoff = flying;
//	for (int y = 0; y < rows; y++) {
//		float xoff = 0;
//		for (int x = 0; x < cols; x++) {
//			terrain[x][y] = map(noise(xoff, yoff), 0, 1, -100, 100);
//			xoff += 0.2;
//		}
//		yoff += 0.2;
//	}

	private void UpdateMesh(){
		currentPerlinNoiseOffset -= flyingSpeed;
		float yOff = currentPerlinNoiseOffset;

		int i = 0;
		Vector3[] vertices = mesh.vertices;
		for (int y = 0; y <= ySize; y++) {
			float xOff = 0;
			for (int x = 0; x <= xSize; x++) {
				Vector3 newVector = vertices[i];
				newVector.z = Mathf.PerlinNoise(xOff, yOff) * maxZHeight*2 - maxZHeight;
				vertices[i] = newVector;
				xOff += perlinNoiseOffset;
				i++;
			}
			yOff += perlinNoiseOffset;
		}
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}

	private void OnDrawGizmos(){
		if(vertices == null){
			return;
		}
		Gizmos.color = GizmoColor;
		for (int i = 0; i < vertices.Length; i++) {
			Vector3 point = this.transform.position + vertices[i];
			point = this.transform.rotation * point;
			Gizmos.DrawSphere(point, 0.1f);
		}
	}
}




