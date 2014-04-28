using UnityEngine;
using System.Collections;

/*
 * Submit button is a subclass of widget.
 * Uses an event called OnSubmit, which suscribers can bind to it once it gets fired (which happens when the submit button gets clicked)
*/
public class SubmitButton : Widget 
{
	public delegate void OnSubmitEvent(SubmitButton submitButton);
  	public event OnSubmitEvent OnSubmit;
	
	// Use this for initialization
	new void Start () 
	{
		base.Start();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//For now, behavior is to turn green when hovered over.
		background.renderer.material.color = original_col;
		
		if(IsHover)
		{
			background.renderer.material.color = Color.green;
		}
		
	}
	
	//What happens when you hover over this widget.
	public new void Hover()
	{
		base.Hover();
	}
	
	//What happens when you stop hovering over this widget.
	public new void Leave()
	{
		base.Leave();
	}
	
	//What happens when you click on this widget.
	public new void Click()
	{
		OnSubmit(this);		//Fire the event when it's clicked
	}
	
}
