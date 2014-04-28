using UnityEngine;
using System.Collections.Generic;

/*
* The widget used by SentenceMenu. Has a string array representing the different choices you can make.
*
* Set them in the inspector.
*
*/
public class StringWidget : Widget
{
	public string[] choices;
	List<TextMesh> GUI_choices;
	
	public int index = 0;        
	
	public int column_num;        //0 is left column, 1 is next right, etc.
	
	SentenceMenu smenu;
	
	public Vector3 bracket_scale;
	
	public string GetChoice()
	{
		return choices[index];
	}
	
	// Use this for initialization
	new void Start ()
	{
		GUI_choices = new List<TextMesh>();
		base.Start();        
	}
	
	public void Resize()
	{
		Vector3 scale = transform.localScale;
		scale.z *= choices.Length;
		transform.localScale = scale;
	}
	
	public void ErrorHandler()
	{
		if(choices.Length <= 0)
		{
			Debug.LogError ("Error: In " + menu.name + ", a choice widget has no choices");
			Destroy(menu.gameObject);
			return;
		}
	}
	
	//Note: This comes after smenu sets the widget positions.
	public void InitStringWidget()
	{
		background.renderer.enabled = false;
		
		smenu = (SentenceMenu)menu;
		
		ErrorHandler();

		Resize();
		
		//Shift the widgets down correspondingly to the number of indices.
		float shift_down = ((choices.Length-1) / 2.0f) * smenu.widget_height;
		transform.position += new Vector3(0f,-shift_down,0f);
		
		//Handle global y-positioning of the choices text in the widget
		Vector3 pos = new Vector3(transform.position.x,smenu.GetTranslatedY(),-0.1f);
		
		for(int i=0;i<choices.Length;i++)
		{
			//transform.position = pos;
			//transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y,-0.1f);

			TextMesh added = Instantiate(((SentenceMenu)smenu).textPrefab,pos,Quaternion.identity) as TextMesh;
			added.transform.parent = gameObject.transform;
			
			added.text = choices[i];
			
			GUI_choices.Add(added);
			
			pos.y -= (smenu.row_dist + smenu.choice_height);
		}
		
		move_dist = smenu.choice_height + smenu.row_dist;
		
		//Set the indices and default starting y position
		
		index = (choices.Length - 1) / 2;
		
		if(index < 0) index = 0;
		if(index > choices.Length) index = choices.Length;
		
		transform.position += new Vector3(0f,move_dist*index,0f);
		
	}
	
	public void BrightenAndBold()
	{
		foreach(TextMesh text in GUI_choices)
		{
			//text.fontStyle = FontStyle.Bold;
			text.renderer.material.color = smenu.bright;
		}
		smenu.brackets[column_num].transform.localScale = bracket_scale * 1.01f;
		smenu.brackets[column_num].renderer.material.color = smenu.bright;
	}
	
	public void DimAndUnbold()
	{
		foreach(TextMesh text in GUI_choices)
		{
			//text.fontStyle = FontStyle.Normal;
			text.renderer.material.color = smenu.dim;
		}
		smenu.brackets[column_num].transform.localScale = bracket_scale * 1f;
		smenu.brackets[column_num].renderer.material.color = smenu.dim;
	}
	
	int pull = 0;        //0 = no pull, 1 = up, 2 = down
	
	float timer = 0f;
	Vector3 dest;
	float move_dist;
	
	// Update is called once per frame
	void Update ()
	{
		if(!smenu || !smenu.init)
		{
			return;
		}
		
		//If user is not holding down click, this widget is no longer selected.
		if(!menu.wand.IsHoldingClick())
		{
			IsSelected = false;
		}
		
		if(IsHover == true || IsSelected == true)
		{
			BrightenAndBold();
		}
		else
		{
			DimAndUnbold();
		}
		
		//Check for pulling
		if(pull == 0)
		{
			if(IsSelected)
			{
				if(menu.wand.IsMovingUp() && index < choices.Length-1)
				{
					index++;
					pull = 1;
					dest = transform.position + new Vector3(0f,move_dist,0f);
				}
				else if(menu.wand.IsMovingDown() && index > 0)
				{
					index--;
					pull = 2;
					dest = transform.position - new Vector3(0f,move_dist,0f);
				}
				
			}
		}
		//Pull up
		else if(pull == 1)
		{
			//Move up by 1x choice height and 1x row dist
			
			timer += Time.deltaTime;
			
			transform.position = Vector3.Lerp(transform.position,dest,Time.deltaTime*5f);
			
			if(Vector3.Distance(transform.position,dest) < 0.02f || timer > 1f)
			{
				transform.position = dest;
				pull = 0;
				timer = 0;
			}
		}
		//Pull down
		else if(pull == 2)
		{
			timer += Time.deltaTime;
			
			transform.position = Vector3.Lerp(transform.position,dest,Time.deltaTime*5f);
			
			if(Vector3.Distance(transform.position,dest) < 0.02f || timer > 1f)
			{
				transform.position = dest;
				pull = 0;
				timer = 0;
			}
			
		}
		
	}
}