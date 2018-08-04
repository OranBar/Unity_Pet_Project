using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMapVisualizer : MonoBehaviour {

	public int width, heigth;
	public GameObject cellPrefab;

	public Gradient influenceColorGradient;

	public InfluenceMap InflMap {
		get {
			return this._influenceMap;
		}
	}
	[SerializeField] private InfluenceMap _influenceMap;

	[SerializeField] private InfluenceMapCell[,]  influenceMapCells;

	public void applyInfluence(int x, int y, int fullDistance, int reducedDistance, double distanceDecay, double influence)
	{
		InflMap.applyInfluence(influence, fullDistance, reducedDistance, distanceDecay, x, y);
	}

	public int xOffset;
	public int zOffset;
	public int yOffset;

	private GameObject cellsContainer;
	public double maxInfluence;
	public double minInfluence;

	private void Awake()
	{
		_influenceMap = new InfluenceMap(width, heigth, minInfluence, maxInfluence, new EuclideanDistanceSqr());
		cellsContainer = transform.Find("Cells Container").gameObject;

		influenceMapCells = new InfluenceMapCell[width, heigth];

		foreach (InfluenceMapCell cell in cellsContainer.GetComponentsInChildren<InfluenceMapCell>())
		{
			influenceMapCells[cell.x, cell.y] = cell;
		}
		
	}

	private void Start()
	{

		UpdateInfluenceColor();
	}

	#region Map Generation
	[ContextMenu("Generate Map")]
	public void GenerateMap()
	{
		this.Assert(cellPrefab.GetComponent<InfluenceMapCell>() != null);

		influenceMapCells = new InfluenceMapCell[width, heigth];
		
		cellsContainer = new GameObject("Cells Container");
		cellsContainer.transform.parent = this.transform;

		for (int y = 0; y < heigth; y++)
		{
			for (int x = 0; x < width; x++)
			{
				GameObject cell = GenerateCell(x, y);
				cell.transform.parent = cellsContainer.transform;
				influenceMapCells[x, y] = cell.GetComponent<InfluenceMapCell>();
			}
		}
	}
	
	[ContextMenu("Destroy Map")]
	public void Destroy()
	{
		DestroyMap();
	}

	private void DestroyMap()
	{
		DestroyImmediate(this.transform.Find("Cells Container").gameObject);
	}

	private GameObject GenerateCell(int x, int y)
	{
		Vector3 position = Vector3.zero;
		position.x = xOffset * x;
		position.z = zOffset * y;

		GameObject newTile = Instantiate(cellPrefab, position, Quaternion.identity) as GameObject;
		newTile.name = newTile.name.Replace("Clone", x + " ," + y);

		newTile.GetComponent<InfluenceMapCell>().Init(x, y);

		return newTile;
	}
	
	[ContextMenu("Regenerate Board")]
	public void RegenerateBoard()
	{
		DestroyMap();
		GenerateMap();
	}
	#endregion
	
	//Alligns the unity 3d objects with the InfluenceMap's current state
	public void UpdateInfluenceColor()
	{
		for (int y = 0; y < heigth; y++)
		{
			for (int x = 0; x < width; x++)
			{
				var amount = InflMap[x, y];
				float normalizedAmount = Mathf.InverseLerp((float)minInfluence, (float)maxInfluence, (float)amount);
				var color = influenceColorGradient.Evaluate(normalizedAmount);
				influenceMapCells[x, y].ChangeColor(color);
				//influenceMapCells[x, y].GetComponent<Renderer>().material.color = color;
			}
		}
	}
}
