using System;
using System.Collections.Generic;

public interface DistanceFunc
{
	double computeDistance(int x1, int y1, int x2, int y2);
}

public class EuclideanDistanceSqr : DistanceFunc {

	public double computeDistance(int x1, int y1, int x2, int y2)
	{
		return Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2);
	}
}

public struct XAndY
{
	public int x, y;

	public XAndY(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}

public class InfluenceMap
{
	protected double[][] _influenceMap;
	protected int width, height;

	private double minInfluence, maxInfluence;

	public int getWidth()
	{
		return width;
	}

	public int getHeight()
	{
		return height;
	}

	public DistanceFunc computeDistanceFunc;


	public double this[int x, int y] {
		get {
			return get(x, y);
		}
		set {
			_influenceMap[x][y] = value;
		}
	}

	public InfluenceMap()
	{
			
	}
	
	public InfluenceMap(int width, int height, double minInfluence, double maxInfluence, DistanceFunc computeDistanceFunc)
	{
		this.width = width;
		this.height = height;
		this._influenceMap = new double[width][];
		for (int i = 0; i < width; i++)
		{
			this._influenceMap[i] = new double[height];
		}
		this.computeDistanceFunc = computeDistanceFunc;
		this.minInfluence = minInfluence;
		this.maxInfluence = maxInfluence;
		myHashset = new HashSet<XAndY>();
	}

	private bool isInBounds(int x, int y)
	{
		return x >= 0 && x < width && y>=0 && y < height;
	}

	/**
	 * Returns 1 if the amount was correctly set, 0 if the x,y were out of the map's bounds. 
	 */
	private int SetAmount_IfInBounds(int x, int y, double amount)
	{
		if (isInBounds(x, y))
		{
			this[x, y] = amount;
			return 1;
		}

		return 0;
	}
	
	/**
	 * Returns 1 if the amount was correctly set, 0 if the x,y were out of the map's bounds. 
	 */
	private int AddAmount_IfInBounds(int x, int y, double amount)
	{
		if (isInBounds(x, y))
		{
			var currAmount = this[x, y];
			this[x, y] = currAmount + amount;
			return 1;
		}

		return 0;
	}

	public double get(int x, int y)
	{
		var amount = _influenceMap[x][y];
		var clampedAmount = Clamp(amount, minInfluence, maxInfluence);
		return clampedAmount;
	}
	
	public double Clamp(double value, double min, double max)  
    {  
        return (value < min) ? min : (value > max) ? max : value;  
    }

	public List<XAndY> getNeighbours(int x, int y)
	{
		List<XAndY> neighbours = new List<XAndY>();
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i == 0 && j == 0) { continue; }

				int xNeighbour = x - i, yNeighbour = y - j;
				if (xNeighbour >= 0 && xNeighbour <= width - 1
				&& yNeighbour >= 0 && yNeighbour <= height - 1)
				{
					neighbours.Add(new XAndY( xNeighbour, yNeighbour ));
				}
			}
		}
		return neighbours;
	}


	//public int[][] getNeighbours(int x, int y)
	//{
	//	int noOfNeighbours = 8;
	//	if (x == 0 || x == width - 1)
	//	{
	//		noOfNeighbours -= 3;
	//	}
	//	if (y == 0 || y == width - 1)
	//	{
	//		if (noOfNeighbours < 8)
	//		{
	//			noOfNeighbours -= 2;
	//		}
	//		noOfNeighbours -= 3;
	//	}

	//	int currNeighbours = 0;
	//	int[][] neighbours = new int[noOfNeighbours][];
	//	for (int i = -1; i <= 1; i++)
	//	{
	//		for (int j = -1; j <= 1; j++)
	//		{
	//			if (i == 0 && j == 0) { continue; }

	//			int xNeighbour = x - i, yNeighbour = y - j;
	//			if (xNeighbour >= 0 && xNeighbour <= width - 1
	//			&& yNeighbour >= 0 && yNeighbour <= height - 1)
	//			{
	//				neighbours[currNeighbours] = new int[] { xNeighbour, yNeighbour };
	//				currNeighbours++;
	//			}
	//		}
	//	}
	//	return neighbours;
	//}

	private HashSet<XAndY> myHashset;

	public void applyInfluence(int x, int y, double amount, int fullDistance, int decayedDistance, double distanceDecay)
	{
		AddAmount_IfInBounds(x, y, amount);

		for (int range = 1; range <= fullDistance + decayedDistance; range++)
		{
			if (range > fullDistance)
			{
				amount *= distanceDecay;
			}
			AddAmount_IfInBounds(x + range, y + 0, amount);
			AddAmount_IfInBounds(x - range, y + 0, amount);
			
			AddAmount_IfInBounds(x + 0, y + range, amount);
			AddAmount_IfInBounds(x + 0, y - range, amount);

			for (int diagonal = 1; diagonal < range; diagonal++)
			{
				double influenceToApply = amount;
				//we fill the rombo by mirroring 1/4 of the shape. (an edge connected to the center)
				int fails = 0;
				fails += AddAmount_IfInBounds(x + range - diagonal, y - diagonal, amount);
				fails += AddAmount_IfInBounds(x + range - diagonal, y + diagonal, amount);
				fails += AddAmount_IfInBounds(x - range + diagonal, y + diagonal, amount);
				fails += AddAmount_IfInBounds(x - range + diagonal, y - diagonal, amount);
				//if(fails == 4) { break; }
				//_influenceMap[x + range - diagonal][y - diagonal] = amount;
				//_influenceMap[x + range - diagonal][y + diagonal] = amount;
				//_influenceMap[x + range + diagonal][y + diagonal] = amount;
				//_influenceMap[x + range + diagonal][y - diagonal] = amount;
			}
		}
	}

	public void applyInfluenceStars(int x, int y, double amount, int fullDistance, int decayedDistance, double distanceDecay)
	{
		AddAmount_IfInBounds(x, y, amount);

		for (int range = 1; range < fullDistance + decayedDistance; range++)
		{
			if (range > fullDistance)
			{
				amount *= distanceDecay;
			}
			AddAmount_IfInBounds(x + range, y + 0, amount);
			AddAmount_IfInBounds(x + 0, y + range, amount);
			AddAmount_IfInBounds(x - range, y + 0, amount);
			AddAmount_IfInBounds(x + 0, y - range, amount);
		}
	}

//	private int TrySetAmount(int x, int y, double amount)
//	{
//		try
//		{
//			_influenceMap[x][y] += amount;
//			return 1;
////			return true;
//		}
//		catch { return 0; } //out of map
//	}

	/// FullAmount Distance is the distance where the full amount will be applied. Then, reducedAmountDistance is the distance that will suffer from decay
	//public void applyInfluence(int x, int y, double amount, int fullAmountDistance, int reducedAmountDistance, double distanceDecay)
	//{
	//	List <XAndY> alreadyExplored = new List<XAndY>();
	//	applyInfluenceRecursive(x, y, amount, fullAmountDistance, reducedAmountDistance, distanceDecay, alreadyExplored);
	//}
	public void applyInfluenceRecursive(int x, int y, double amount, int fullAmountDistance, int reducedAmountDistance, double distanceDecay, List<XAndY> alreadyExplored)
	{
		if (fullAmountDistance < 0 && reducedAmountDistance < 0) { throw new Exception("Error"); }

		try
		{
			double foo = _influenceMap[x][y];
		}
		catch 
		{
			//End of map
			return;
		}


		//if (fullAmountDistance > 0)
		//{
		//	//System.err.println("x "+x+ " y "+y);
		//	_influenceMap[x][y] = amount;
		//	myHashset.Clear();
		//	myHashset.AddRange(getNeighbours(x, y));
		//	myHashset.ExceptWith(alreadyExplored);

		//	foreach (int[] neighbour in getNeighbours(x, y))
		//	{
		//		applyInfluenceRecursive(neighbour[0], neighbour[1], amount, fullAmountDistance - 1, reducedAmountDistance, distanceDecay);
		//	}
		//}

		//if (reducedAmountDistance > 0)
		//{
		//	_influenceMap[x][y] = amount;
		//	foreach (int[] neighbour in getNeighbours(x, y))
		//	{
		//		applyInfluenceRecursive(neighbour[0], neighbour[1], amount * distanceDecay, fullAmountDistance, reducedAmountDistance - 1, distanceDecay);
		//	}
		//}

		//System.err.println("x "+x+ " y "+y);


		if(alreadyExplored.Contains(new XAndY(x, y))==false)
		{
			_influenceMap[x][y] += amount;
			alreadyExplored.Add(new XAndY(x,y));
		}
		myHashset = new HashSet<XAndY>();
		foreach (var neighbour in getNeighbours(x,y))
		{
			myHashset.Add(neighbour);
		}
//		myHashset.AddRange(getNeighbours(x, y));
		myHashset.ExceptWith(alreadyExplored);
		

		foreach (XAndY neighbour in myHashset)
		{
			if (fullAmountDistance > 0)
			{
				applyInfluenceRecursive(neighbour.x, neighbour.y, amount, fullAmountDistance - 1, reducedAmountDistance, distanceDecay, alreadyExplored);

			}
			else if(reducedAmountDistance > 0)
			{
				applyInfluenceRecursive(neighbour.x, neighbour.y, amount * distanceDecay, fullAmountDistance, reducedAmountDistance - 1, distanceDecay, alreadyExplored);

			}
		}
	}

	public void applyInfluence(double amount, int fullAmountDistance, int reducedAmountDistance, double distanceDecay, params int[] points)
	{
		if (points.Length % 2 == 1)
		{
		    throw new Exception("invalid number of points args");
		}

		int noOfPoints = points.Length / 2;

		amount /= noOfPoints;

		for (int i = 0; i < noOfPoints; i++)
		{
			int pointX = points[(i * 2)];
			int pointY = points[(i * 2) + 1];

			applyInfluence(pointX, pointY, amount, fullAmountDistance, reducedAmountDistance, distanceDecay);
		}

	}
	
	//Use two opposite corners, where x1<x2 or y1<y2. They will define the bounds of the search. 
	//The tile with highest score is selected. If multiple bests, it will select the last
	public Tuple<int, int> selectBestInBox(int x1, int y1, int x2, int y2)
	{
		//TODO: handle casse where x1>x2 or y1>y2?
		double currBestScore = double.MinValue;
		Tuple<int, int> currBest = Tuple.Create(-1, -1);
		for (int currX = x1; currX <= x2; currX++)
		{
			for (int currY = y1; currY <= y2; currY++)
			{
				if (get(currX, currY) > currBestScore)
				{
					currBestScore = get(currX, currY);
					currBest = Tuple.Create(currX, currY);
				}
			}
		}
		return currBest;
	}



}