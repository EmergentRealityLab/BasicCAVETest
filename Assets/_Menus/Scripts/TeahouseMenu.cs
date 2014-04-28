using UnityEngine;
using System.Collections.Generic;

/*
 * This is the menu used for the teahouse scene, and can be used for any "Restaraunt-type" menus.
 * User selects some items, then hits a submit button.
 *When submit is clicked, for now, a Debug string returns the menu items ordered and their price.
*/
public class TeahouseMenu : Menu
{
	// On start, all the widgets are added to the Menu's list of widgets.
	new void Start () 
	{
		//base.Start ();
	}
	new void Awake()
	{
		base.Awake();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Don't display the submit button if there are no orders.
		if(GetNumSelectedWidgets() > 0)
		{
			Submit.Enable();
		}
		else
		{
			Submit.Disable();
		}
	}
	
	//What happens when the submit button for this menu is clicked?
	public override void OnSubmit(SubmitButton submitButton)
	{ 
		string order = "";
		for(int i=0;i<widgets.Count;i++)
		{
			if(widgets[i].IsSelected && widgets[i] is FoodMenuItem)
			{
				order += ((FoodMenuItem)widgets[i]).food_name += " : ";
				order += ((FoodMenuItem)widgets[i]).price.ToString ("C");
				order += "\n";
			}
		}
		Debug.Log (order);
		
		//For now, reset all selections and set num_submitted to 0 after submission so you can order again.
		for(int i=0;i<widgets.Count;i++)
		{
			widgets[i].Unselect();
		}
		
	}
	
}
