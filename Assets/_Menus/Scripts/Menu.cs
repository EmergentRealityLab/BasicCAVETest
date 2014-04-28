using UnityEngine;
using System.Collections.Generic;

/*
 *Menu is the superclass for all menus.
 *The menu should have widgets attached to its gameobject. Depending on the menu type, a submit button may or may not be added.
 *You can attach all menus to the camera as a child gameobject so that wherever the camera looks, the menu will follow.
 *Also make sure that the menu and its widgets are not affected by lighting.
 *
 *Menu has a background that is a gameobject. This may contain the background texture, and its size dictates the menu size for scaling reasons.
 *Menus must have a background named "Background".
 *
*/
public class Menu : MonoBehaviour 
{
	public List<Widget> widgets;	//List of non-label widgets (i.e. widgets that can be interacted with)
	
	public List<LabelWidget> labels;	//List of label widgets (i.e. widgets that cannot be interacted with)
	
	public SubmitButton Submit;		//Menu's submit button
	
	public Wand wand;	//Wand in the scene. This shouldn't be necessary anymore given PointerControl, but some 
	
	public bool use_dynamic_widgets;	//Turn this on to have the menu dynamically generate widgets, instead of attaching widgets to the menu before run-time.
										//If it's turned on, you shouldn't attach any widgets to the menu.
	
	public Widget dynamic_widget;	//The dynamic widget prefab to use. This must be set if you're using dynamic widgets (which means you will be adding widgets at runtime).
	
	public GameObject background;	//Background of this menu
	
	private float width;		//Width of the menu, in Unity meters (internal usage only)
	private float height;	//Height of the menu, in Unity meters (internal usage only)
	
	public bool hidden;		//Is the menu hidden?

	//Get the y position at the top of the menu.
	private float GetMenuTop()
	{
		return GetMenuHeight() / 2f;
	}

	//Set private width and height variables
	private void SetWidthAndHeight()
	{
		width = background.transform.localScale.x;
		height = background.transform.localScale.y;
	}
	
	//Scale the background gameobject depending on the width and height
	private void ScaleBackground()
	{
		SetWidthAndHeight();
		
		background.transform.localScale = new Vector3(width,height,background.transform.localScale.z);
		
	}
	
	//This function will return the y-spacing needed between widgets in the menu.
	//It computes this using the menu height, the widget height and number of widgets.
	//(These heights are simply the heights of the plane gameobjects.)
	private float GetWidgetYSpacing(int num_widgets)
	{
		//Formula: (MH - (WH * NW) ) / (NW + 1)
		//where MH = menu height, WH = widget height, and NW = number of widgets.
		
		float MH = GetMenuHeight();
		float WH = GetWidgetHeight();
		int NW = num_widgets;
		
		return (MH - (WH * NW) ) / (float)(NW + 1.0f);
	}

	//Get the height of the menu
	private float GetMenuHeight()
	{
		return height;
	}

	//Get the height of the dynamic widget (only if it is used)
	private float GetWidgetHeight()
	{
		return dynamic_widget.height;
	}
	
	//Dynamically add a list of widgets to this menu whose text is the list of strings provided as argument.
	//The default implementation will lay them out vertically, top-to-bottom.
	public virtual void SetDynamicWidgets(List<string> messages)
	{
		//Pos should start at the top of the menu
		Vector3 pos = new Vector3(0,GetMenuTop(),-0.1f);
		
		//Set this to the left of the menu
		float x_pos = 0;
		
		float y_offset = GetWidgetYSpacing(messages.Count) + GetWidgetHeight()/2f;
		
		foreach(string message in messages)
		{
			x_pos = 0;

			pos = new Vector3(x_pos,pos.y - y_offset,pos.z);	
	
			Widget new_widget = Instantiate(dynamic_widget,Vector3.zero,background.transform.rotation) as Widget;
			
			new_widget.menu = this;
			new_widget.OnWidgetClick += OnWidgetClick;
			new_widget.OnWidgetHover += OnWidgetHover;
			new_widget.OnWidgetLeave += OnWidgetLeave;
			
			new_widget.transform.parent = transform;
			new_widget.transform.localPosition = pos;
			new_widget.SetText(message);
			
			y_offset = GetWidgetYSpacing(messages.Count) + GetWidgetHeight();
			
			widgets.Add(new_widget);
		}	
	}

	//Space the widgets evenly vertically.
	public void ArrangeWidgets()
	{
		//Pos should start at the top of the menu
		Vector3 pos = new Vector3(0,GetMenuTop(),-0.1f);
		
		//Set this to the left of the menu
		float x_pos = 0;
		
		float y_offset = GetWidgetYSpacing(widgets.Count) + GetWidgetHeight()/2f;
		
		foreach(Widget w in widgets)
		{
			x_pos = 0;
			
			//Since the menu's rotated, it's actual z position is it's "logical" y position
			//pos = new Vector3(x_pos,pos.y,pos.z - y_offset);	
			pos = new Vector3(x_pos,pos.y - y_offset,pos.z);	
	
			w.transform.localPosition = pos;
			w.transform.rotation = background.transform.rotation;
			
			y_offset = GetWidgetYSpacing(widgets.Count) + GetWidgetHeight();
		}		
	}

	//Add a widget dynamically to this menu
	public void AddWidget(Widget w)
	{
		w.menu = this;
		
		w.OnWidgetClick += OnWidgetClick;
		w.OnWidgetHover += OnWidgetHover;
		w.OnWidgetLeave += OnWidgetLeave;
		
		w.transform.parent = transform;
		
		widgets.Add(w);	
	}

	//Remove a widget dynamically to this menu
	public void RemoveWidget(Widget w)
	{
		widgets.Remove(w);
		Destroy(w.gameObject);
	}
	
	public void Awake()
	{
		Transform tr = transform.Find("Background");
		
		if(!tr)
		{
			Debug.LogError("Error in " + name + " menu : no background gameobject child attached with name \"Background\"");
			Destroy(gameObject);
		}
		
		background = tr.gameObject;
		
		ScaleBackground();
		
		wand = GameObject.FindObjectOfType(typeof(Wand)) as Wand;

		widgets = new List<Widget>();
		
		Widget[] awidgets = GetComponentsInChildren<Widget>();
		for(int i=0;i<awidgets.Length;i++)
		{
			if(awidgets[i] is SubmitButton)
			{
				Submit = (SubmitButton)awidgets[i];
				((SubmitButton)awidgets[i]).OnSubmit += OnSubmit;	//Subscribe the OnSubmit method to SubmitButton's OnSubmit event.
			}
			//Add non-label widgets
			else if(!(awidgets[i] is LabelWidget))
			{
				widgets.Add (awidgets[i]);
			
				awidgets[i].OnWidgetClick += OnWidgetClick;
				awidgets[i].OnWidgetHover += OnWidgetHover;
				awidgets[i].OnWidgetLeave += OnWidgetLeave;
			}
			//Add label widgets
			else if(awidgets[i] is LabelWidget)
			{
				labels.Add(awidgets[i] as LabelWidget);
			}
		}
		SortWidgets();
		
	}
	
	// Use this for initialization
	protected void Start() 
	{
		
	}
	
	public void SortWidgets()
	{
		widgets.Sort ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	//Event that's fired when submit button is clicked.
	public virtual void OnSubmit(SubmitButton submitButton)
	{
	}
	
	//Event that's fired when a widget is hovered over.
	public virtual void OnWidgetHover(Widget widget)
	{
	}
	
	//Event that's fired when a widget is selected
	public virtual void OnWidgetClick(Widget widget)
	{
	}
	
	//Event that's fired when a widget is no longer selected.
	public virtual void OnWidgetLeave(Widget widget)
	{
	}
	
	//Gets the number of selected widgets
	public int GetNumSelectedWidgets()
	{
		int counter = 0;
		for(int i=0;i<widgets.Count;i++)
		{
			if(widgets[i].IsSelected)
			{
				counter++;
			}
		}
		return counter;
		
	}
	
	public void DeselectAllWidgets()
	{
		foreach(Widget w in widgets)
			w.IsSelected = false;
	}

	//When widgets are disabled, they are greyed out and no longer selectable, but still visible
	public void DisableAllWidgets()
	{
		foreach(Widget w in widgets)
			w.Disable();
	}
	public void EnableAllWidgets()
	{
		foreach(Widget w in widgets)
			w.Enable();
	}
	
	public bool AllWidgetsDisabled()
	{
		if(widgets.Count == 0) 
			return true;
		
		int counter = 0;
		foreach(Widget w in widgets)
		{
			if(w.disabled)
				counter ++;
		}
		return counter == widgets.Count;
	}

	//When widgets are invisible, they are not visible and can't be selected
	public void SetVisible(bool visible)
	{
		if(!visible)
		{
			hidden = true;
			DisableAllWidgets();
		}
		else
		{
			hidden = false;
			EnableAllWidgets();
		}
		
		foreach(Renderer r in GetComponentsInChildren<Renderer>())
		{
			if(r.name != "Background")
				r.enabled = visible; 
		}
	}

}
