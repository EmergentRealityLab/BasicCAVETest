using UnityEngine;
using System.Collections;

/*
 * LabelWidget represents a widget that cannot be interacted with. Good to represent titles of menus, where you want
 * the title to take on a string that is read in at run-time.
 * If you don't need to read in a string at run-time, a hard-drawn image file of the menu title may be a better choice.
 */
public class LabelWidget : Widget 
{
	// Use this for initialization
	protected new void Start () 
	{
		base.Start();
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	
}
