using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using OranUnityUtils;

public class InfluenceMapVisualizer : MonoBehaviour {

	public int width, height;
	public GameObject cellPrefab;

	public Gradient influenceColorGradient;

	

//	public int influenceMapUnit = 25;
	public int mouseoverRange = 10;
	private List<Text> labels = new List<Text>();

	
	
	
	public InfluenceMap InflMap {
		get {
			return this._influenceMap;
		}
		private set { _influenceMap = value; }
	}
	[SerializeField] private InfluenceMap _influenceMap;

	[SerializeField] private InfluenceMapCell[,]  influenceMapCells;

	public void applyInfluence(int x, int y, int fullDistance, int reducedDistance, double distanceDecay, double influence)
	{
		InflMap.ApplyInfluence_Diamond(x, y, influence, fullDistance, reducedDistance, distanceDecay);
	}

	public int xOffset;
	public int zOffset;
	public int yOffset;

	private GameObject cellsContainer;
	public double maxInfluence;
	public double minInfluence;

	
	[BoxGroup("Codingame Encoded Turn Info")]
	public string gameInfo_encoded;
	[BoxGroup("Codingame Encoded Turn Info")]
	public string gameState_encoded;
	[BoxGroup("Codingame Encoded Turn Info")]
	public string pulzella_encoded;
	
	private void Awake()
	{
		//By defualt, we create an empty one.
		influenceMapCells = new InfluenceMapCell[width, height];
		_influenceMap = new InfluenceMap(width, height, minInfluence, maxInfluence, new EuclideanDistanceSqr());
		ReferenceCells();
	}

	private void ReferenceCells()
	{
		cellsContainer = transform.Find("Cells Container").gameObject;

		foreach (InfluenceMapCell cell in cellsContainer.GetComponentsInChildren<InfluenceMapCell>())
		{
			influenceMapCells[cell.x, cell.y] = cell;
			cell.OnMouseOver_Delegate = OnCellMouseOver;
			labels.Add(cell.influenceLabel);
		}
	}

	public void SetNewInfluenceMap(InfluenceMap m)
	{
		InflMap = m;
		foreach (var influenceMapCell in influenceMapCells)
		{
			influenceMapCell.influenceValue = (float) InflMap[influenceMapCell.x, influenceMapCell.y];
		}
	}

	public void OnCellMouseOver(InfluenceMapCell mouseOverCell)
	{
		int x1 = Mathf.Max(mouseOverCell.x - mouseoverRange, 0);
		int y1 = Mathf.Max(mouseOverCell.y - mouseoverRange, 0);
		int x2 = Mathf.Min(mouseOverCell.x + mouseoverRange, width-1);
		int y2 = Mathf.Min(mouseOverCell.y + mouseoverRange, height-1);
		
		Vector2 startPos = new Vector2(mouseOverCell.x, mouseOverCell.y);
		
		List<Text> adjacentLabels = new List<Text>();
		InfluenceMapCell bestCell = influenceMapCells[x1, y1], worstCell = influenceMapCells[x1, y1];
		float distToBestCell = float.MaxValue, distToWorstCell = float.MaxValue;;

		for (int i = x1; i <= x2; i++)
		{
			for (int j = y1; j <= y2; j++)
			{
				InfluenceMapCell cell = influenceMapCells[i, j];
				adjacentLabels.Add(cell.influenceLabel);
				if (cell.influenceValue >= bestCell.influenceValue)
				{
					if (cell.influenceValue == bestCell.influenceValue)
					{
						float distToCell = Vector2.Distance(startPos, new Vector2(cell.x, cell.y));
						if (distToCell < distToBestCell)
						{
							//We don't swap if the cell isn't any closer to the mouse.
							bestCell = cell;
							distToBestCell = distToCell;
						}
					}
					else
					{
						bestCell = cell;
					}

				}
				if (cell.influenceValue <= worstCell.influenceValue)
				{
					if (cell.influenceValue == worstCell.influenceValue)
					{
						float distToCell = Vector2.Distance(startPos, new Vector2(cell.x, cell.y));
						if (distToCell < distToWorstCell)
						{
							//We don't swap if the cell isn't any closer to the mouse.
							worstCell = cell;
							distToWorstCell = distToCell;
						}
					}
					else
					{
						worstCell = cell;
					}
					
				}
			}
		}

		bestCell.influenceLabel.color = Color.cyan;
		worstCell.influenceLabel.color = Color.magenta;

		labels.Except(adjacentLabels).ForEach(l => SetLabelColorAlpha(l, 0));
		adjacentLabels.ForEach(l => SetLabelColorAlpha(l, 1));
	}

	private void SetLabelColorAlpha(Text l, float alpha)
	{
		var color = l.color;
		color.a = alpha;
		l.color = color;
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.LeftControl))
		{
			labels.ForEach(l => SetLabelColorAlpha(l, 1));
			labels.ForEach(l => SetLabelColor(l, Color.red));
		}

		if (Input.mouseScrollDelta.y != 0)
		{
			int sign = Math.Sign(Input.mouseScrollDelta.y);
			mouseoverRange = mouseoverRange + sign;
			UpdateCells();
		}
	}

	private void SetLabelColor(Text text, Color col)
	{
		text.color = col;
	}

	private void AllLabelsTransparentExcept()
	{
		
	}

	private void Start()
	{
//		UpdateCells();
	}
	
	

	#region Map Generation
	[ContextMenu("Generate Map")]
	public void GenerateMap()
	{
		this.Assert(cellPrefab.GetComponent<InfluenceMapCell>() != null);

		influenceMapCells = new InfluenceMapCell[width, height];
		
		cellsContainer = new GameObject("Cells Container");
		cellsContainer.transform.parent = this.transform;

		for (int y = 0; y < height; y++)
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
		position.z = zOffset * y *-1;

		GameObject newTile = Instantiate(cellPrefab, position, Quaternion.identity) as GameObject;
		newTile.name = newTile.name.Replace("Clone", x + " ," + y);
		
//		newTile.GetComponent<Renderer>().sharedMaterial = cellPrefab.GetComponent<Renderer>().sharedMaterial;
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
	public void UpdateCells()
	{
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				var amount = InflMap[x, y];
				
				float normalizedAmount = Mathf.InverseLerp((float)minInfluence, (float)maxInfluence, (float)amount);
				var color = influenceColorGradient.Evaluate(normalizedAmount);
				influenceMapCells[x, y].ChangeColor(color);
				influenceMapCells[x, y].UpdateLabel();
				//influenceMapCells[x, y].GetComponent<Renderer>().material.color = color;
			}
		}
	}
}
