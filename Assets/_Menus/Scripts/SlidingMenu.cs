using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 *Sliding menu is a fancy menu that will scroll if the user points to the left or to the right of the screen. 
 *Sliding menu's widgets can be anywhere - they will align themselves properly.
 *However, all widgets should the same width and height.
 *
 *To use:
 *Specify the amount of rows and columns. (only a rectangular amount of widgets are supported.)
 *Then, add child widgets to the Sliding Menu gameobject, until you have (row * column) amount of widgets.
 *As with all menus, you should make it a child of the camera, so that wherever the camera rotates, the menu will follow along.
 *The widgets should arrange themselves from left to right, then top to bottom, with respect to their alphabetic names.
 *So name the widgets to specify the way they will be arranged.
*/
public class SlidingMenu : Menu
{
	//If both are used, the number of widgets must equal num_cols * num_rows.
	public int num_cols = 1;
	public int num_rows = 4;
	
	//The initial index you want to start at.
	public int start_x;
	public int start_y;
	
	float x_index = 0;
	float y_index = 0;
	
	public float col_dist = 0.5f;
	public float row_dist = 0.5f;
	
	float widget_length;
	float widget_height;
	
	new void Awake()
	{
		base.Awake();
	}
	
	// Use this for initialization
	new void Start () 
	{
		wand = GameObject.FindObjectOfType(typeof(Wand)) as Wand;
		
		if(!wand)
			Debug.LogError("Error: Could not find a wand in the scene; menu will not work");
		//else
		//	Debug.Log (wand);
		
	    x_index = start_x;
	    y_index = start_y;
	
		widgets = new List<Widget>();
		Widget[] awidgets = GetComponentsInChildren<Widget>();
		
		Array.Sort(awidgets, (x,y) => y.name.CompareTo(x.name) );
		
		for(int i=0;i<num_rows;i++)
		{
			Array.Sort(awidgets, i*(num_cols), num_cols);
		}
		
		for(int i=0;i<awidgets.Length;i++)
		{
			widgets.Add (awidgets[i]);
			
			awidgets[i].OnWidgetClick += OnWidgetClick;
			awidgets[i].OnWidgetHover += OnWidgetHover;
			awidgets[i].OnWidgetLeave += OnWidgetLeave;
		}
		
		if(widgets.Count != (num_rows * num_cols))
		{
			//Debug.Log (widgets.Count + " != " + (num_rows * num_cols));
			Debug.LogError("Error: Menu does not have proper amount of widgets for its row/col amounts");
			Destroy(gameObject);
		}
		if(widgets.Count == 0)
		{
			Debug.LogError("Error: No widgets in menu");
			Destroy(gameObject);
		}
		
		widget_length = widgets[0].transform.lossyScale.x * 10f;	//This is only for planes-change this for real meshes
		widget_height = widgets[0].transform.lossyScale.z * 10f;	//This is only for planes-change this for real meshes
		
		float f1 = (0.5f*widget_height*(1-num_rows)) + (0.5f*row_dist*(1-num_rows));	//This is distance from center point to leftmost widget's center point
		fy = num_rows <= 1 ? 0 : (2*f1 / (num_rows-1));		//This is the distance between consecutive widget's center points
					
		float f2 = (0.5f*widget_length*(1-num_cols)) + (0.5f*col_dist*(1-num_cols));
		fx = num_cols <= 1 ? 0 : (2*f2 / (num_cols-1));
				
	}
	
	bool start_slide = false;
	int slide_hori = 0;	//1 = slide left, 2 = slide right
	int slide_vert = 0; //1 = slide up, 2 = slide down
	float timer = 0;
	
	float sX;
	float sY;
	
	float fx;	
	float fy;	
	
	// Update is called once per frame
	void Update () 
	{
		if(x_index < 0)
			x_index = 0;
		if(x_index > num_cols - 1)
			x_index = 0;
		if(y_index < 0)
			y_index = 0;
		if(y_index > num_rows - 1)
			y_index = num_rows;
		
		for(int i=0;i< num_rows;i++)
		{
			float upY = (i*fy) + (y_index * (widget_height + row_dist));//-f1 + (i*f2);
				
			for(int j=0;j< num_cols; j++)
			{
				float leftX = (j*fx) + (x_index * (widget_length + col_dist));//-f1 + (i*f2);
				
				widgets[(i*num_cols)+j].transform.localPosition = new Vector3(-leftX,-upY,0.1f);
			}
		}
		
		//Not sliding
		if(start_slide == false)
		{
			//Ensures that index will take integer values
			x_index = Mathf.Round(x_index);
			y_index = Mathf.Round(y_index);
			
			//Is the wand pointing to the left of the screen? Then shift the menu left if possible
			if(wand.IsPointingLeft())
			{
				if(x_index > 0)
				{
					start_slide = true;
					slide_hori = 1;
					sX = x_index;
				}
			}
			//Is the wand pointing to the right of the screen? Then shift the menu right if possible
			else if(wand.IsPointingRight())
			{
				if(x_index < num_cols - 1)
				{
					start_slide = true;
					slide_hori = 2;
					sX = x_index;
				}
			}
			//Is the wand pointing to the bottom of the screen? Then shift the menu right if possible
			if(wand.IsPointingDown())
			{
				if(y_index > 0)
				{
					start_slide = true;
					slide_vert = 1;
					sY = y_index;
				}
			}
			//Is the wand pointing to the top of the screen? Then shift the menu right if possible
			else if(wand.IsPointingUp())
			{
				if(y_index < num_rows - 1)
				{
					start_slide = true;
					slide_vert = 2;
					sY = y_index;
				}
			}
		}
		else
		{ 
			timer += Time.deltaTime;
			
			if(slide_hori == 1)
			{
				x_index = sX - Mathf.SmoothStep (0,1,timer*2.5f);
			}
			else if(slide_hori == 2)
			{
				x_index = sX + Mathf.SmoothStep (0,1,timer*2.5f);
			}
			
			if(slide_vert == 1)
			{
				y_index = sY - Mathf.SmoothStep (0,1,timer*2.5f);
			}
			else if(slide_vert == 2)
			{
				y_index = sY + Mathf.SmoothStep (0,1,timer*2.5f);
			}
			
			if(timer >= 0.4)
			{
				timer = 0;
				start_slide = false;
				slide_hori = 0;
				slide_vert = 0;
				
				x_index = Mathf.Round(x_index);
				y_index = Mathf.Round(y_index);
			}
		}
		
	}
}
