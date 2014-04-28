using UnityEngine;
using System.Collections.Generic;

//The dialog widget is a subclass of widget, designed to be used by the TeaDialogTree.
//It's a widget that will generate a response and animation and lead to another DialogMenu.
public class DialogWidget : Widget
{
	//The choice menu that this dialog widget leads to. If it's null, then hitting it will finish the dialog tree.
	public DialogMenu DialogMenu;
	
	//The GameObject name of the next choice menu.
	public string choicemenu_name;
	
	//The response that this dialog widget generates, if any.
	public string response;
	//The audio clip for the response
	public AudioClip audio_clip;

	//the animation clip for the response
	public string animationClip;

	public string english_text;
	public string chinese_text;	
	public string pinyin;

	public int hint_on;
	
	public bool halt_until_good_action;

	//Contains an array of the names of future dialog widgets to not show if this widget is selected
	public string[] excluded_choices;

	//Called when the cheats button is pressed
	public void ToggleHint()
	{
		hint_on++;

		if(hint_on > 2){ hint_on = 0; }

		if(hint_on == 0)
		{
			SetText (chinese_text);
		}
		else if(hint_on == 1)
		{
			SetText(pinyin);
		}
		else if(hint_on == 2)
		{
			SetText(english_text);
		}
	}
	
	public bool IsTreeInEndCond()
	{
		if(!menu){ return false; }
		
		DialogMenu dmenu = (menu as DialogMenu);
		
		if(!dmenu){ return false; }
		
		return dmenu.start_check_for_end_cond;
	
	}
	
	public void SetChoiceMenu(DialogMenu achoicemenu)
	{
		DialogMenu = achoicemenu;
	}
	
	Vector3 orig_scale;
	
	// Use this for initialization
	new void Start () 
	{
		base.Start();
		SetText (chinese_text);
		orig_scale = transform.localScale;
	}
	
	// Update is called once per frame
	new void Update () 
	{
		if(IsTreeInEndCond()){ IsHover = false; }
		
		//This is for resetting the scale when not hovering over the widget.
		//We won't be changing the scale of this, so it should be fine.
		transform.localScale = orig_scale;

		//Disabled: Gray out the text
		if(disabled)
		{
			IsHover = false;
			background.collider.enabled = false;
			background.renderer.material.color = Color.gray;
			GetComponentInChildren<TextMesh>().renderer.material.color = new Color(0.25f,0.25f,0.25f,0.5f);
		}
		else
		{
			//For now, behavior is to turn magenta when hovered over, and green when selected.
			background.renderer.material.color = original_col;
			background.collider.enabled = true;
			GetComponentInChildren<TextMesh>().renderer.material.color = Color.black;
			
			if(IsHover)
			{
				transform.localScale = orig_scale * 1.1f;
			}
		}
		if(IsSelected)
		{
			GetComponentInChildren<TextMesh>().renderer.material.color = Color.blue;
		}
	}
	
	//What happens when you are hovering over this widget.
	public override void Hover()
	{
		if(IsTreeInEndCond()){ return; }
		
		//If hovering over, change state to hover
		IsHover = true;
		
		SetWidgetHover();
	}
	
	//What happens when you stop hovering over this widget.
	public override void Leave()
	{	
		if(IsTreeInEndCond()){ return; }
		
		//If stop hovering over, change state to unselected
		IsHover = false;
		
		SetWidgetLeave ();
	}
	
	//What happens when you click on this widget.
	public override void Click()
	{
		if(IsTreeInEndCond()){ return; }
		
		base.Click();

		/*
		if(halt_until_good_action)
		{
			((DialogMenu)menu).halt_until_good_action = true;
		}
		*/
	}
}
