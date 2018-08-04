using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMapCell : MonoBehaviour {

	public int x, y;
	public float influenceValue;
	public int influenceModifier = 15;
	public int fullDistance = 3;
	public int reducedDistance = 2;
	public double distanceDecay = 1;

	private Material myMaterial;

	private void Update()
	{
		var amount = influenceMapVisualizer.InflMap[x, y];
		if(amount != influenceValue)
		{
			influenceValue = (float) amount;
		}
	}

	[AutoParent] private InfluenceMapVisualizer influenceMapVisualizer;

	private void Awake()
	{
		this.myMaterial = Instantiate(this.GetComponent<Renderer>().material);
		Init(x,y);
	}

	public void Init(int x, int y)
	{
		this.x = x;
		this.y = y;
		this.GetComponent<Renderer>().material = this.myMaterial;
	}

	private void OnMouseDown()
	{
		var newInfluence = influenceModifier;
		if (Input.GetKey(KeyCode.LeftShift))
		{
			newInfluence *= -1;
		}
		influenceMapVisualizer.applyInfluence(x,y, fullDistance, reducedDistance, distanceDecay, newInfluence);
		influenceMapVisualizer.UpdateInfluenceColor();
	}

	public void ChangeColor(Color color)
	{
		myMaterial.color = color;
	}
}
