using UnityEngine;
using System.Collections.Generic;

/*
 * This is a menu that allows you to slide sentences together. Good for the codebreaking episode or
 * sentence formation quizzes.
 * 
 * Uses StringWidgets (see class & prefab).
 * 
 * You can place the widgets ANYWHERE on the menu, as long as they are the children of a gameobject with SentenceMenu
 * script. The widgets will rearrange themselves from left to right, in alphabetical order of their names.
 * 
 * Each widget can be set a number of string choices. Then, the menu can be used to form different sentence combinations
 * by dragging the widgets up or down, then clicking the submit button.
 * 
 */
 
public class SentenceMenu : Menu 
{
	public TextMesh textPrefab;	//Prefab for the text mesh to use
	
	public List<GameObject> brackets;	//A list of bracket_planes
	public GameObject bracket_plane;	//This is the quad that contains the bracket texture
	
	public float col_dist;	//Distance between sentence columns
	public float row_dist;	//Distance between choices in each column
	
	public float widget_length;	
	public float widget_height;
	
	public float choice_height;	//Height of a "choice" in a sentence
	public float choice_length;  //Length of a "choice" in a sentence

	public List<StringWidget> stringWidgets;	//A list of the stringwidgets in this menu
	
	public float menu_screen_y;	//0f-1f, where should the menu's y position begin?

	//Set this to true to autogenerate column spacing to automatically fit the screen.
	public bool res_independence;	
	
	public Color bright;	//Color when widget is hovered over
	public Color dim;		//Color when widget is not hovered over

	//Get width of camera box in unity meters
	public float GetFrustrumWidth()
	{
		Camera center_camera = Camera.main;
		Rect camera_rect = center_camera.pixelRect;	//You'll need to change this if there is not a single, unique camera tagged "MainCamera" in the scene.
		float cam_dist_from_menu = Vector3.Distance (center_camera.transform.position, transform.position);
		
		float frustumHeight = 2.0f * cam_dist_from_menu * Mathf.Tan(center_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		float frustumWidth = frustumHeight * center_camera.aspect;
		
		return frustumWidth;
	}
	
	public float GetTranslatedY()
	{
		Camera center_camera = Camera.main;
		float cam_dist_from_menu = Vector3.Distance (center_camera.transform.position, transform.position);
		Vector3 newpos = center_camera.ViewportToWorldPoint(new Vector3(0f,menu_screen_y,cam_dist_from_menu));
		return newpos.y;
	}
	
	new void Awake()
	{
		base.Awake();
	}
	
	// Use this for initialization
	new void Start () 
	{
		brackets = new List<GameObject>();
		stringWidgets = new List<StringWidget>();
		
		for(int i=0;i<widgets.Count;i++)
		{
			if(widgets[i] is StringWidget)
			{
				StringWidget theWidget = (StringWidget)widgets[i];
				theWidget.column_num = stringWidgets.Count;
				stringWidgets.Add(theWidget);
			}
		}

		widget_length = stringWidgets[0].transform.lossyScale.x;	
		widget_height = stringWidgets[0].transform.lossyScale.z;	
		
		choice_height = 0.5f;	//Height of a "choice" in a sentence
		choice_length = 0.5f;  //Length of a "choice" in a sentence
		
		row_dist = 0.5f;	
		
		if(res_independence)
		{	
			float frustumWidth = GetFrustrumWidth();
			col_dist = (frustumWidth - (stringWidgets.Count * widget_length)) / (float)(stringWidgets.Count + 1.0f);
		}
		
		float num_cols = stringWidgets.Count;
		
		float f1 = (0.5f*widget_length*(1-num_cols)) + (0.5f*col_dist*(1-num_cols));
		float fx = num_cols <= 1 ? 0 : (2*f1 / (num_cols-1));
		
		for(int j=0;j < num_cols; j++)
		{
			float leftX = -f1 + (j*fx);

			stringWidgets[j].transform.localPosition = new Vector3(leftX,0.1f,0f);
			Vector3 pos = stringWidgets[j].transform.position;
			pos.y = GetTranslatedY();
			stringWidgets[j].transform.position = pos;
			
			GameObject bracket = Instantiate (bracket_plane) as GameObject;
			
			bracket.transform.parent = transform.parent;
			bracket.transform.localPosition = new Vector3(leftX,0.1f,0f);
			bracket.transform.position = pos;
			
			brackets.Add(bracket);
			
			stringWidgets[j].bracket_scale = bracket.transform.localScale;
		}	
	}
	
	public bool init = false;
	
	// Update is called once per frame
	void Update () 
	{
		if(!init)
		{
			for(int i=0;i<stringWidgets.Count;i++)
			{
				stringWidgets[i].InitStringWidget();
			}
			init = true;
		}
	}
	
	//What happens when the submit button for this menu is clicked?
	public override void OnSubmit(SubmitButton submitButton)
	{ 
		string sentence = "";
		for(int i=stringWidgets.Count-1;i>=0;i--)
		{
			sentence += (stringWidgets[i]).GetChoice();
			if(i != 0)
				sentence += " ";
		}
		
		sentence += ".";
			
		Debug.Log (sentence);
		
	}
}
