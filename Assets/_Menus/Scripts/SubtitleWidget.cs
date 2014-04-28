using UnityEngine;
using System.Collections;

/*A widget dedicated to outputting subtitles.
* More functionality should be added depending on the specifications of how subtitles work (i.e. should words fill one at a time?
* should the subtitle text scroll?)
* Unlike most other widgets, does not need a parent menu.
* Now uses an easy text mesh instead of a normal one, so that a black shadow effect is possible.
* This class overrides a lot in the regular widget because it uses a different text mesh type (EasyFontTextMesh).
*/
public class SubtitleWidget : Widget 
{
	//The audio clip for the subtitle
	//public DialogClip audio_clip;

	public new EasyFontTextMesh easy_text_mesh;
	public GameObject easy_go;

	public MeshFilter my_text_mesh;

	// Use this for initialization
	new void Start () 
	{
		base.Start();
		easy_go = GetComponentInChildren<EasyFontTextMesh>().gameObject;
		my_text_mesh = easy_go.GetComponentInChildren<MeshFilter>();
	}
	
	// Update is called once per frame
	new void Update () 
	{
	}

	//Set/update the text of the text mesh.
	public new void SetText(string atext)
	{
		//if(this is DialogWidget)
		//	Debug.Log("LossyScale of dialog widget: " + transform.lossyScale);
		
		if(easy_text_mesh)
		{
			if(easy_text_mesh.Text == atext) { return; }
			easy_text_mesh.Text = atext;
		}
		else
		{
			easy_text_mesh = GetComponentInChildren<EasyFontTextMesh>() as EasyFontTextMesh;	
			easy_text_mesh.Text = atext;
		}
		
		if(use_word_wrap){ WordWrap(); }
		
		ScaleBackgroundToText();
		
	}	
	
	public new void AppendText(string atext)
	{
		if(easy_text_mesh)
			easy_text_mesh.Text += atext;
		else
		{
			easy_text_mesh = GetComponentInChildren<EasyFontTextMesh>() as EasyFontTextMesh;	
			easy_text_mesh.Text += atext;
		}
	}

	
	//Update and scale the background gameobject according to the text
	//This will also scale the widget background as such.
	public new void ScaleBackgroundToText()
	{
		float new_width = 0;
		float new_height = 0;

		new_width = easy_text_mesh.renderer.bounds.size.x;
		new_height = easy_text_mesh.renderer.bounds.size.y;

		background.transform.localScale = new Vector3(new_width,new_height,0);
		
	}

	//Format the text so that it's width does not go beyond max_width
	//Note: Be sure than if the text has newlines in it, there are no spaces directly after the newline(s), otherwise
	//the stretching won't work properly.
	//This is already handled by the XML parser so you only need to worry about this if you're entering in text from non-XML source
	public new void WordWrap()
	{	
		if(easy_text_mesh.Text == "")
		{
			return;
		}

		easy_text_mesh.Text = easy_text_mesh.Text.TrimEnd('\r', '\n');
		
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

			//my_text_mesh.mesh.RecalculateBounds();
			easy_text_mesh.RefreshMesh(true);

			if(loop > inf_loop_amt){ Debug.Log ("Warning: An infinite loop has occured!"); goto outer; }
			
			char last_removed_char = '\0';
			
			new_segment = "";
			
			split_word = true;
			
			last_char_in_line = false;
			
			//Keep removing characters until the bounds size is less than or equal to the maximum
			while(easy_text_mesh.renderer.bounds.size.x > max_width && loop2 <= inf_loop_amt && loop < inf_loop_amt)
			{
				loop2++;

				//my_text_mesh.mesh.RecalculateBounds();
				easy_text_mesh.RefreshMesh(true);

				if(loop2 > inf_loop_amt){ Debug.Log ("Warning: An infinite loop has occured!"); goto outer; }
				
				if(debug) { 
					Debug.Log("Current text: " + easy_text_mesh.Text);
					Debug.Log("Width: " + easy_text_mesh.renderer.bounds.size.x);
				}
				split_word = true;
				
				//If we are on the last word in the line:
				if(StringUtils.IsLastLineOneWord(easy_text_mesh.Text))
				{
					split_word = false;
					if(debug) { 
						Debug.Log("The last line is one word. split_word set to " + split_word.ToString());
					}
				}
				
				if(StringUtils.IsLastLineOneLetter(easy_text_mesh.Text))
				{
					//Debug.Log ("Letter: " + easy_text_mesh.Text[easy_text_mesh.Text.Length-1]);
					//if(max_width < easy_text_mesh.renderer.bounds.size.x) 
					max_width = easy_text_mesh.renderer.bounds.size.x;
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
						last_removed_char = easy_text_mesh.Text[easy_text_mesh.Text.Length-1];
						easy_text_mesh.Text = easy_text_mesh.Text.Remove(easy_text_mesh.Text.Length-1);
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
					while(!(easy_text_mesh.Text.Length == 0 || easy_text_mesh.Text[easy_text_mesh.Text.Length-1] == ' ' || easy_text_mesh.Text[easy_text_mesh.Text.Length-1] == '\n'));
				}
				else
				{
					if(debug) { 
						Debug.Log("Split word is false, splitting by letter\n");
					}
					
					last_removed_char = easy_text_mesh.Text[easy_text_mesh.Text.Length-1];
					easy_text_mesh.Text = easy_text_mesh.Text.Remove(easy_text_mesh.Text.Length-1);
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
				easy_text_mesh.Text += "\n" + new_segment;
				if(debug) { 
					Debug.Log("\teasy_text_mesh is now " + easy_text_mesh.Text);
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

	//What happens when you are hovering over this widget.
	public override void Hover(){}
	//What happens when you stop hovering over this widget.
	public override void Leave(){}
	//What happens when you click on this widget.
	public override void Click(){}
}
