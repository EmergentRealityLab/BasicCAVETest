using UnityEngine;
using System.Collections;
using System;

/*The Widget class is the superclass for all GUI buttons/items.
 *Attach widget gameobjects to Menu gameobjects (see Menu script). Menu will then process all its attached widgets. 
 *Various subclasses of widgets are used for different widgets. In addition, SubmitButton is a subclass of widget.
 *
 *Like with menu, widget has a background gameobject which represents the background. This will be the gameobject that changes color and has the collider
 *for the raycast. Widgets must have a background named "Background".
 *
 *To display 3D text, attach a 3D text mesh as a child. The widget will automatically incorperate it as an attribute.
*/
public class Widget : MonoBehaviour , IComparable<Widget>
{
	//Widget's text can change dynamically. This saves the first string it was set to.
	public string orig_text;
	
	public TextMesh text_mesh;	//The 3d text for this widget. 
	
	public delegate void OnHoverEvent(Widget widget);
  	public event OnHoverEvent OnWidgetHover;	
	
	public delegate void OnLeaveEvent(Widget widget);
  	public event OnLeaveEvent OnWidgetLeave;
	
	public delegate void OnClickEvent(Widget widget);
  	public event OnClickEvent OnWidgetClick;
	
	public Menu menu;	//The widget's parent menu
	
	public GameObject background;	//Controller background of this widget (detects collision)
	public WidgetBackground widget_background;	//View background of this widget (shows image)
	
	public bool use_word_wrap;		//Turn this on if you want the widget's text to wrap. 
	public float max_width;		//When scale_to_text is true, max_width defines the maximum width of the word wrap;
	public float orig_max_width;

	//Stuff to default to when width and height are set to 0
	private const float DEFAULT_WIDTH = 3f;
	private const float DEFAULT_HEIGHT = 1.5f;
	
	protected Color original_col;	//Original color of the widget.
	
	public bool disabled;	//Is the widget disabled? If true, then it is greyed out.
	public bool IsHover;	//Is the widget being hovered over?
	public bool IsSelected;	//Is the widget selected?
	
	//Width of the widget, in Unity meters. FOR INTERNAL USAGE, YOU CAN'T SET THIS. CHANGE SCALE IF YOU WANT TO CHANGE SIZE OF WIDGET
	public float width
	{
		get 
		{
			if(!widget_background)
				return background.transform.localScale.x;
			else
				return widget_background.total_width;
		}
	}
	
	//Height of the widget, in Unity meters. FOR INTERNAL USAGE, YOU CAN'T SET THIS. CHANGE SCALE IF YOU WANT TO CHANGE SIZE OF WIDGET
	public float height
	{
		get 
		{
			if(!widget_background)
				return background.transform.localScale.y;
			else
				return widget_background.total_height;
		}
	}
	
	//Update and scale the background gameobject according to the text
	//This will also scale the widget background as such.
	public void ScaleBackgroundToText()
	{
		float new_width = 0;
		float new_height = 0;
		
		if(!widget_background)
		{
			new_width = text_mesh.renderer.bounds.size.x;
			new_height = text_mesh.renderer.bounds.size.y;
		}
		else
		{
			widget_background.BuildBackground(text_mesh,this);
			new_width = widget_background.total_width;
			new_height = widget_background.total_height;
		}
		
		background.transform.localScale = new Vector3(new_width,new_height,0);
		
	}

	//Here we find all attached backgrounds and set them to the variables. This should only be called once, and as early as possible.
	void SetBackgroundGameObjects()
	{
		Transform tr = transform.Find("Background");
		
		if(!tr)
		{
			Debug.LogError("Error in " + name + " widget : no background gameobject child attached with name \"Background\"");
			Destroy(gameObject);
		}
		
		background = tr.gameObject;
		
		widget_background = GetComponentInChildren<WidgetBackground>();
		
	}
	
	//Format the text so that it's width does not go beyond max_width
	//Note: Be sure than if the text has newlines in it, there are no spaces directly after the newline(s), otherwise
	//the stretching won't work properly.
	//This is already handled by the XML parser so you only need to worry about this if you're entering in text from non-XML source
	//Warning: in rare corner cases this method seems to be buggy and causes an infinite loop, so there is an infinte loop guard.
	public void WordWrap()
	{
		if(text_mesh.text == "")
			return;
		
		text_mesh.text = text_mesh.text.TrimEnd('\r', '\n');
		
		string new_segment;
		bool split_word = false;
		bool last_char_in_line = false;
		bool last_word_in_line = false;
		
		int inf_loop_amt = 500;
		int loop = 0;
		int loop2 = 0;
		
		bool debug = false;
		
		do
		{
			loop++;
			
			if(loop > inf_loop_amt){ Debug.Log ("Warning: An infinite loop has occured!"); goto outer; }
			
			char last_removed_char = '\0';
			
			new_segment = "";
			
			split_word = true;
			
			last_char_in_line = false;
			
			//Keep removing characters until the bounds size is less than or equal to the maximum
			while(text_mesh.renderer.bounds.size.x > max_width && loop2 <= inf_loop_amt && loop < inf_loop_amt)
			{
				loop2++;
				
				if(loop2 > inf_loop_amt){ Debug.Log ("Warning: An infinite loop has occured!"); goto outer; }
								
				if(debug) { 
				Debug.Log("Current text: " + text_mesh.text);
				}
				split_word = true;
				
				//If we are on the last word in the line:
				if(StringUtils.IsLastLineOneWord(text_mesh.text))
				{
					split_word = false;
					if(debug) { 
					Debug.Log("The last line is one word. split_word set to " + split_word.ToString());
					}
				}
					
				if(StringUtils.IsLastLineOneLetter(text_mesh.text))
				{
					//Debug.Log ("Letter: " + text_mesh.text[text_mesh.text.Length-1]);
					//if(max_width < text_mesh.renderer.bounds.size.x) 
						max_width = text_mesh.renderer.bounds.size.x;
					if(debug) { 
					Debug.Log("\tThe last line is one letter. Max width is now " + max_width.ToString());
					Debug.Log("\tBreaking...");
					}
					break;
				}
				
				
				if(split_word)
				{
					if(debug)
						Debug.Log("Split_word is true: Not splitting by letter, but by word");
					//Repeatedly remove the last character, adding it to the beginning of new_segment, until a space or newline is encountered
					do
					{
						last_removed_char = text_mesh.text[text_mesh.text.Length-1];
						text_mesh.text = text_mesh.text.Remove(text_mesh.text.Length-1);
						new_segment = last_removed_char + new_segment;
						if(debug) { 
						Debug.Log("\tNot splitting word.");
						Debug.Log("\tRemoved " + last_removed_char);
						Debug.Log("\tnew_segment is now " + new_segment);
						}
						
						if(last_removed_char == ' '){ break; }
						if(last_removed_char == '\n'){ break; }
					}
					//Repeat until no characters left, the last character is a blank, or the last character is a newline
					while(!(text_mesh.text.Length == 0 || text_mesh.text[text_mesh.text.Length-1] == ' ' || text_mesh.text[text_mesh.text.Length-1] == '\n'));
				}
				else
				{
					if(debug) { 
					Debug.Log("Split word is false, splitting by letter\n");
					}
					
					last_removed_char = text_mesh.text[text_mesh.text.Length-1];
					text_mesh.text = text_mesh.text.Remove(text_mesh.text.Length-1);
					new_segment = last_removed_char + new_segment;
					
					if(debug) { 
					Debug.Log("\tRemoved " + last_removed_char);
					Debug.Log("\tnew_segment is now " + new_segment);
					}
				}
			}
			
			if(new_segment != "")
			{
				if(debug) { 
				Debug.Log("Adding to text the segment: " + new_segment);
				}
				text_mesh.text += "\n" + new_segment;
				if(debug) { 
				Debug.Log("\ttext_mesh is now " + text_mesh.text);
				}
			}
			
		}
		while(new_segment != "" && loop2 <= inf_loop_amt && loop < inf_loop_amt);
		
	outer:{}
		
		if(loop >= inf_loop_amt || loop2 >= inf_loop_amt)
		{
			Debug.Log ("warning: infinite loop!");
			Debug.Log (name + " caused the infinite loop.");
			Debug.Log ("It's string is\n" + orig_text);
			Debug.Log ("It's max length is " + orig_max_width);
			Debug.Break();
		}
		
	//	Debug.Break();
		
		
	}
	
	//Set/update the text of the text mesh.
	public void SetText(string atext)
	{
		if(text_mesh)
		{
			if(text_mesh.text == atext) { return; }
			text_mesh.text = atext;
		}
		else
		{
			text_mesh = GetComponentInChildren<TextMesh>() as TextMesh;	
			text_mesh.text = atext;
		}
		
		if(use_word_wrap){ WordWrap(); }
		
		ScaleBackgroundToText();
		
	}	

	//Add to the text of the text pesh.
	public void AppendText(string atext)
	{
		if(text_mesh)
			text_mesh.text += atext;
		else
		{
			text_mesh = GetComponentInChildren<TextMesh>() as TextMesh;	
			text_mesh.text += atext;
		}
	}

	//Used for sorting
	public int CompareTo(Widget that)
    {
        return (String.Compare(name,that.name));
    }
	
	void Awake()
	{
		//Guard against bad max_width value (can't be 0)
		if(max_width == 0) { max_width = 10; }
		
		text_mesh = GetComponentInChildren<TextMesh>() as TextMesh;	
		
		SetBackgroundGameObjects();
	}
	
	// Use this for initialization
	protected void Start () 
	{
		if(transform.parent)
			menu = transform.parent.gameObject.GetComponent<Menu>();
		
		original_col = background.renderer.material.color;
		
		if(text_mesh != null)
			SetText(text_mesh.text);
		
	}
	
	// Update is called once per frame
	protected void Update () 
	{
		if(disabled)
		{
			background.collider.enabled = false;
			IsHover = false;
			background.renderer.material.color = Color.gray;
			GetComponentInChildren<TextMesh>().renderer.material.color = new Color(0.25f,0.25f,0.25f);
			
		}
		else
		{
			background.collider.enabled = true;
			GetComponentInChildren<TextMesh>().renderer.material.color = Color.black;
			
			//For now, behavior is to turn magenta when hovered over, and green when selected.
			background.renderer.material.color = original_col;
			
			if(IsHover)
			{
				background.renderer.material.color = Color.magenta;
			}
			if(IsSelected)
			{
				background.renderer.material.color = Color.green;
			}
		}
	}
	
	protected virtual void SetWidgetHover()
    {
       OnHoverEvent handler = OnWidgetHover;
       if( handler != null )
           handler(this);
    }
	protected virtual void SetWidgetLeave()
    {
       OnLeaveEvent handler = OnWidgetLeave;
       if( handler != null )
           handler(this);
    }
	protected virtual void SetWidgetClick()
    {
       OnClickEvent handler = OnWidgetClick;
       if( handler != null )
           handler(this);
    }
	
	//What happens when you are hovering over this widget.
	public virtual void Hover()
	{
		//If hovering over, change state to hover
		IsHover = true;
		
		SetWidgetHover();
	}
	
	//What happens when you stop hovering over this widget.
	public virtual void Leave()
	{	
		//If stop hovering over, change state to unselected
		IsHover = false;
		
		SetWidgetLeave ();
	}
	
	//What happens when you click on this widget.
	public virtual void Click()
	{
		if(disabled){ return; }
		
		//On click: The state is selected if it is not, and unselected if it is
		if(!IsSelected)
		{
			IsSelected = true;
		}
		else
		{
			IsSelected = false;
		}
		SetWidgetClick();
	}
	
	//Disables this widget by turning off its collider 
	public void Disable()
	{
		disabled = true;
	}
	
	//Enables this widget by turning on its collider 
	public void Enable()
	{	
		disabled = false;
	}
	
	public void Unselect()
	{
		IsSelected = false;
	}
	
	public void Select()
	{
		IsSelected = true;
	}
	
	//Given a gameobject, get the parent's widget component (if it exists).
	public static Widget GetRootWidget(GameObject hit)
	{
		if(hit.transform.parent)
			if(hit.transform.parent.GetComponent<Widget>())
				return (hit.transform.parent.GetComponent<Widget>());
		return null;
	}

}
