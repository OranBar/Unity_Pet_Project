using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;


public class MouseOverCameraHorizontalMovement : MonoBehaviour
{
	private DropdownList<Vector2> vectorValues = new DropdownList<Vector2>()
	{
		{"Right", Vector2.right},
		{"Left", Vector2.left},
		{"Up", Vector2.up},
		{"Down", Vector2.down}
	};

	[Dropdown("vectorValues")]
	public Vector2 scrollDirection;

	//Set registering event trigger in unity
	private bool MouseOverElement { get; set; }

	//Register using event trigger in unity
	public void SetMouseOverTrue()
	{
		MouseOverElement = true;
	}
	//Register using event trigger in unity

	public void SetMouseOverFalse()
	{
		MouseOverElement = false;
	}
	
	// Update is called once per frame
	private void Update()
	{
		if(MouseOverElement)
		{
			Camera.main.transform.Translate(scrollDirection);
		}
	}
}
