using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InfluenceMapCell_Unity : MonoBehaviour {

	public int x, y;
	public float influenceValue;
	public int influenceModifier = 15;
	public int fullDistance = 3;
	public int reducedDistance = 2;
	public double distanceDecay = 1;

	public bool enableMouseInput = false;

	public Text influenceLabel;
	private Material myMaterial;

	public Action<InfluenceMapCell_Unity> OnMouseOver_Delegate { get; set; }
	
	private void Start()
	{
//		UpdateLabel();
	}

//	private void Update()
//	{
//		var amount = influenceMapVisualizer.InflMap[x, y];
//		if(amount != influenceValue)
//		{
//			influenceValue = (float) amount;
//			UpdateLabel();
//		}
//	}

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

//	private void OnMouseDown()
//	{
//		if (enableMouseInput == false){ return; }
//		
//		
//		var newInfluence = influenceModifier;
//		if (Input.GetKey(KeyCode.LeftShift))
//		{
//			newInfluence *= -1;
//		}
//		influenceMapVisualizer.applyInfluence(x,y, fullDistance, reducedDistance, distanceDecay, newInfluence);
//		influenceMapVisualizer.UpdateInfluenceColor();
//	}

	private void OnMouseOver()
	{
		OnMouseOver_Delegate?.Invoke(this);
	}

	public void ChangeColor(Color color)
	{
		myMaterial.color = color;
	}

	public void UpdateLabel()
	{
		if (influenceValue != 0)
		{
			influenceLabel.text = influenceValue.ToString(".00");
		}
		else
		{
			influenceLabel.text = "";
		}
	}
}
