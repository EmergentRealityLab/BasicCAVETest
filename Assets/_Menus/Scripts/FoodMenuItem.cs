using UnityEngine;
using System.Collections;

/*
  *MenuItem is a widget designed for the teahouse food menu.
  *Items will have a food name and price for now.
*/
public class FoodMenuItem : Widget 
{
	public string food_name;
	public float price;

	// Use this for initialization
	new void Start () 
	{
		base.Start();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//For now:
		//If hovered over, a pink filled rectangle will appear over the item.
		//If selected, a green filled rectangle will stay over the item until it is unselected.
		
		background.renderer.enabled = false;
		background.renderer.material.color = Color.magenta;
		
		if(IsHover == true)
		{
			background.renderer.enabled = true;
		}
		if(IsSelected == true)
		{
			background.renderer.enabled = true;
			background.renderer.material.color = Color.green;
		}
	}
	
	//What happens when you hover over this widget.
	new public void Hover()
	{
		base.Hover();
	}
	
	//What happens when you stop hovering over this widget.
	new public void Leave()
	{
		base.Leave();
	}
	
	//What happens when you click on this widget.
	new public void Click()
	{
		base.Click ();
	}
	
}
