using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
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




	public InfluenceMap InflMap
	{
		get { return this._influenceMap; }
		private set { _influenceMap = value; }
	}
	
	private InfluenceMap _influenceMap;

	public InfluenceMapCell_Unity[,]  InfluenceMapCellsUnity;

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

	[SerializeField]
	private bool _enableMouseInputCells;
	[SerializeField]
	private bool _showNumbers;

	private InfluenceMapCell_Unity currTile_mouseover;
	private InfluenceMapCell_Unity currBestTile_mouseover, currWorstTile_mouseover;
	private InfluenceMapCell_Unity prevTile_mouseover;
	private InfluenceMapCell_Unity prevBestTile_mouseover, prevWorstTile_mouseover;
	
	
	public Color defaultTextColor, bestTextColor, worstTextColor;
	private Position myQueenPosition = null;
	private List<Position> myEnemiesPositions = new List<Position>();
	private Position enemyQueenPosition;


	public bool EnableMouseInputCells
	{
		get { return _enableMouseInputCells; }
		set
		{
			if (value == _enableMouseInputCells)
			{
				return;
			}
			
			_enableMouseInputCells = value;
			EnableMouseInput(value);
		}
	}
	
	public bool ShowNumbers
	{
		get { return _showNumbers; }
		set
		{
			if (value == _showNumbers)
			{
				return;
			}
			
			_showNumbers = value;
			EnableNumbers(value);
		}
	}


	private void Awake()
	{
		//By defualt, we create an empty one.
		InfluenceMapCellsUnity = new InfluenceMapCell_Unity[width, height];
		_influenceMap = new InfluenceMap(width, height, minInfluence, maxInfluence, new EuclideanDistanceSqr(), LaPulzellaD_Orleans.INFLUENCEMAP_SQUARELENGTH);
		ReferenceCells();
	}

	private void ReferenceCells()
	{
		cellsContainer = transform.Find("Cells Container").gameObject;

		foreach (InfluenceMapCell_Unity cell in cellsContainer.GetComponentsInChildren<InfluenceMapCell_Unity>())
		{
			InfluenceMapCellsUnity[cell.x, cell.y] = cell;
			cell.OnMouseOver_Delegate = OnCellMouseOver;
			labels.Add(cell.influenceLabel);
		}
	}

	public void SetNewInfluenceMap(InfluenceMap m)
	{
		InflMap = m;
		foreach (var influenceMapCell in InfluenceMapCellsUnity)
		{
			influenceMapCell.influenceValue = (float) InflMap[influenceMapCell.x, influenceMapCell.y];
		}
	}

	public void OnCellMouseOver(InfluenceMapCell_Unity mouseOverCellUnity)
	{
		//Color cells in range of mouseovercell
		int x1 = Mathf.Max(mouseOverCellUnity.x - mouseoverRange, 0);
		int y1 = Mathf.Max(mouseOverCellUnity.y - mouseoverRange, 0);
		int x2 = Mathf.Min(mouseOverCellUnity.x + mouseoverRange, width-1);
		int y2 = Mathf.Min(mouseOverCellUnity.y + mouseoverRange, height-1);
		
		Vector2 startPos = new Vector2(mouseOverCellUnity.x, mouseOverCellUnity.y);
		
		List<Text> adjacentLabels = new List<Text>();
		InfluenceMapCell_Unity bestCellUnity = InfluenceMapCellsUnity[x1, y1], worstCellUnity = InfluenceMapCellsUnity[x1, y1];
		float distToBestCell = float.MaxValue, distToWorstCell = float.MaxValue;;

		if (currBestTile_mouseover != null)
		{
			currBestTile_mouseover.influenceLabel.color = defaultTextColor;
		}

		if (currWorstTile_mouseover != null)
		{
			currWorstTile_mouseover.influenceLabel.color = defaultTextColor;
		}
		
		for (int i = x1; i <= x2; i++)
		{
			for (int j = y1; j <= y2; j++)
			{
				InfluenceMapCell_Unity cellUnity = InfluenceMapCellsUnity[i, j];
				adjacentLabels.Add(cellUnity.influenceLabel);
				if (cellUnity.influenceValue >= bestCellUnity.influenceValue)
				{
					if (cellUnity.influenceValue == bestCellUnity.influenceValue)
					{
						float distToCell = Vector2.Distance(startPos, new Vector2(cellUnity.x, cellUnity.y));
						if (distToCell < distToBestCell)
						{
							//We don't swap if the cell isn't any closer to the mouse.
							bestCellUnity = cellUnity;
							distToBestCell = distToCell;
							currBestTile_mouseover = bestCellUnity;

						}
					}
					else
					{
						bestCellUnity = cellUnity;
						currBestTile_mouseover = bestCellUnity;
					}

				}
				if (cellUnity.influenceValue <= worstCellUnity.influenceValue)
				{
					if (cellUnity.influenceValue == worstCellUnity.influenceValue)
					{
						float distToCell = Vector2.Distance(startPos, new Vector2(cellUnity.x, cellUnity.y));
						if (distToCell < distToWorstCell)
						{
							//We don't swap if the cell isn't any closer to the mouse.
							worstCellUnity = cellUnity;
							distToWorstCell = distToCell;
							currWorstTile_mouseover = worstCellUnity;

						}
					}
					else
					{
						worstCellUnity = cellUnity;
						currWorstTile_mouseover = worstCellUnity;
					}
					
				}
			}
		}

		bestCellUnity.influenceLabel.color = bestTextColor;
		worstCellUnity.influenceLabel.color = worstTextColor;

		labels.Except(adjacentLabels).ForEach(l => SetLabelColorAlpha(l, 0));
		adjacentLabels.ForEach(l => SetLabelColorAlpha(l, 1));
		
		currTile_mouseover = mouseOverCellUnity;
		
		//Update range if mouse scroll (ctrl is already pressed if we're here, because of the check done before calling the delegate by influencemapcell
		if (Input.mouseScrollDelta.y != 0)
		{
			int sign = Math.Sign(Input.mouseScrollDelta.y);
			mouseoverRange = mouseoverRange + sign;
			UpdateCells();
		}
	}

	private void SetLabelColorAlpha(Text l, float alpha)
	{
		var color = l.color;
		color.a = alpha;
		l.color = color;
	}

	private void Update()
	{
		EnableMouseInputCells = _enableMouseInputCells;
		ShowNumbers = _showNumbers;
		
		if (Input.GetKeyUp(KeyCode.LeftControl))
		{
			labels.ForEach(l => SetLabelColorAlpha(l, 1));
			labels.ForEach(l => SetLabelColor(l, defaultTextColor));
		}

		

		prevTile_mouseover = currBestTile_mouseover;
	}

	private void SetLabelColor(Text text, Color col)
	{
		text.color = col;
	}

	private void AllLabelsTransparentExcept()
	{
		
	}


	#region Map Generation
	[Button("Generate Map")]
	public void GenerateMap()
	{
		this.Assert(cellPrefab.GetComponent<InfluenceMapCell_Unity>() != null);

		InfluenceMapCellsUnity = new InfluenceMapCell_Unity[width, height];
		
		cellsContainer = new GameObject("Cells Container");
		cellsContainer.transform.parent = this.transform;

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				GameObject cell = GenerateCell(x, y);
				cell.transform.parent = cellsContainer.transform;
				InfluenceMapCellsUnity[x, y] = cell.GetComponent<InfluenceMapCell_Unity>();
			}
		}
	}
	
	[Button("Destroy Map")]
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
		newTile.GetComponent<InfluenceMapCell_Unity>().Init(x, y);

		return newTile;
	}
	
	[Button("Regenerate Map")]
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
				InfluenceMapCellsUnity[x, y].ChangeColor(color);
				InfluenceMapCellsUnity[x, y].UpdateLabel();
				//influenceMapCells[x, y].GetComponent<Renderer>().material.color = color;
			}
		}

		if (myQueenPosition != null)
		{
			InfluenceMapCellsUnity[myQueenPosition.x, myQueenPosition.y].ChangeColor(Color.green);
		}
		
		if (enemyQueenPosition != null)
		{
			InfluenceMapCellsUnity[enemyQueenPosition.x, enemyQueenPosition.y].ChangeColor(Color.magenta);
		}

		foreach (var position in myEnemiesPositions)
		{
			InfluenceMapCellsUnity[position.x, position.y].ChangeColor(Color.red);
		}
		
	}

	public void EnableMouseInput(bool enable)
	{
		foreach (var influenceMapCell in InfluenceMapCellsUnity)
		{
			influenceMapCell.enableMouseInput = enable;
		}
	}
	
	public void EnableNumbers(bool enable)
	{
		foreach (var influenceMapCell in InfluenceMapCellsUnity)
		{
			influenceMapCell.influenceLabel.enabled = enable;
		}
	}

	public void SetMyQueenPosition(Position position)
	{
		myQueenPosition = position;
	}
	
	public void SetMyEnemyPosition(Position position)
	{
		myEnemiesPositions.Add(position);
	}
	public void SetEnemyQueenPosition(Position position)
	{
		enemyQueenPosition = position;
	}
}
