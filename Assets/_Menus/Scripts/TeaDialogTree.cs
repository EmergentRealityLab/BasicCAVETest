using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.IO;

/*
 * The TeaDialogTree class represents Mrs. Ling's dialog tree in the game.
 * It's underlying data structure is a list of DialogMenus whose
 * widgets lead to other DialogMenus. (These widgets' branches are set
 * in this class at runtime.)
 * 
 * Unfortunately this doesn't inherit from anything, so it's hard-coded to Mrs. Ling's dialogue and
 * the tea ceremony script, and the tea XML file. I recommend that someone create a generic superclass for a dialogue tree
 * if you plan on using different dialog trees in the future. Have different dialoge menus then inherit 
 * from that base class. That way, common code can be reused, creating DRY code.
 * 
 * Note that it DOESN'T inherit from menu; it's not a menu, but a "manager" of sorts.
 */
public class TeaDialogTree : MonoBehaviour
{
	public AudioSource test_audio_source;
	public string animatedCharacterName;

	//GameObject that shows when you are doing an action
	public GameObject action_indicator;

	public PickupObject pickup_object_script;
	
	//All the menus in this dialog tree.
	public List<DialogMenu> menus;
	//The current menu we are on in this dialog tree.
	public DialogMenu current_menu;	
	
	public DialogMenu choice_menu_prefab;
	public DialogWidget dialog_widget_prefab;
	
	public bool generate_static_tree;
	
	public SubtitleWidget subtitle;
	
	public Vector3 menu_scale;
	
	//A list of the names of all dialog widgets to not show
	public List<string> excluded_choices;

	public string last_response;

	[System.Serializable]
	public class ActionResponse
	{
		public string english_text;
		public string chinese_text;
	}
	
	public List<ActionResponse> good_responses;
	public List<ActionResponse> bad_responses;
	
	public int good_response_index;
	public int bad_response_index;
	
	public List<AudioClip> clips;
	public List<AudioClip> good_response_clips;
	public List<AudioClip> bad_response_clips;
	
	//Phraise or criticism of if you did an action in the tea ceremony correctly
	public DialogClip action_response_clip;
	
	//Timer that dictates how long to wait before failing the action
	public float action_timer;	
	public const float MAX_ACTION_TIME = 120;
	
	public Vector3 menu_position;

	//When this is set to true, it means that a widget was selected that needs a player action afterwards.
	public bool selected_action_widget = false;
	
	public Vector3 get_menu_position()
	{
		Transform t = transform.Find("MenuPosition");
		if(t != null)
		{
			t.renderer.enabled = false;
			return t.position;
		}
		else
		{
			return Vector3.zero;
		}
	}

	private PlayerTeaCeremony playerTeaCeremony;

	public void Reset()
	{
		test_audio_source.clip = null;

		current_menu = null;	

		set_visible_once = false;

		last_response = "";

		good_response_index = 0;
		bad_response_index = 0;

		action_response_clip = null;
		
		selected_action_widget = false;

		foreach(Menu m in menus)
		{
			if(m.GetComponent<SubtitleWidget>() || (m.transform.parent && m.transform.parent.GetComponent<SubtitleWidget>()) || m.name == "MenuPosition" || m.GetComponent<TeaDialogTree>()){}
			else
			{
				m.gameObject.SetActive(true);
				Destroy (m.gameObject);
			}
		}
		foreach(Transform go in GetComponentsInChildren<Transform>())
		{
			if(go.GetComponent<SubtitleWidget>() || (go.transform.parent && go.transform.parent.GetComponent<SubtitleWidget>()) || go.name == "MenuPosition" || go.GetComponent<TeaDialogTree>()){}
			else
				Destroy (go.gameObject);
		}
		Start ();
	}

	void Awake()
	{
		if(Application.loadedLevelName != "MenuTest") {
			playerTeaCeremony = GameObject.Find("MainPlayer").GetComponent<PlayerTeaCeremony>();
			playerTeaCeremony.dialogTree = this; 
			pickup_object_script = GameObject.Find("MainPlayer").GetComponent<PickupObject>();
			if(!Settings.start_allow_actions)
				pickup_object_script.enabled = false;
		}
	}

	// Use this for initialization
	void Start () 
	{
		menu_position = get_menu_position();
		
		//Guard against default bad menu scale
		if(menu_scale == Vector3.zero){ menu_scale = Vector3.one; }
		
		//Load all tea dialog clips
		Object[] objs = Resources.LoadAll("AudioClips/TeaDialog/DialogClips");
		clips = new List<AudioClip>();
		foreach(Object obj in objs)
		{
			AudioClip clip = obj as AudioClip;
			clips.Add(clip);
		}
		
		//Record all good responses clips
		objs = Resources.LoadAll("AudioClips/TeaDialog/GoodResponses");
		good_response_clips = new List<AudioClip>();
		foreach(Object obj in objs)
			good_response_clips.Add(obj as AudioClip);
		
		//Record all bad responses clips
		objs = Resources.LoadAll("AudioClips/TeaDialog/BadResponses");
		bad_response_clips = new List<AudioClip>();
		foreach(Object obj in objs)
			bad_response_clips.Add(obj as AudioClip);

		excluded_choices = new List<string>();
		
		SetResponsesFromXML();
		
		subtitle = GetComponentInChildren<SubtitleWidget>();
		
		if(!subtitle)
		{
			Debug.Log("Error: no subtitle");
			Destroy(gameObject);
		}
		
		if(generate_static_tree)
		{
			//Get all the menus attached. (Don't attach any if you want to dynamically create the tree;
			//this is probably always the case except when debugging.
			DialogMenu[] amenus = GetComponentsInChildren<DialogMenu>();
			foreach(DialogMenu menu in amenus)
			{
				menus.Add(menu);
				
				foreach(Widget w in menu.widgets)
				{
					//Debug.Log ("added widget " + w.name + " of menu " + menu.name);
					w.OnWidgetClick += OnWidgetClick;
				}
			}
		}
		else
		{
			CreateTreeFromXML();
			
			for(int i=0;i<menus.Count;i++)
			{
				menus[i].tree = this;
				
				//Neatly arrange the menu's widgets
				menus[i].ArrangeWidgets();
				
				foreach(Widget w in menus[i].widgets)
				{
					w.OnWidgetClick += OnWidgetClick;
					((DialogWidget)w).DialogMenu = GetChoiceMenuFromName(((DialogWidget)w).choicemenu_name);
				}
				
				if(menus[i].dialog_tree_state == DialogTreeState.Wait)
				{
					menus[i].OnEndCond += OnEndCond;
				}

				//All menu's next menus are the ones that come after them in the XML
				if(i < menus.Count - 1)
				{
					menus[i].next_menu = menus[i+1];
					menus[i].orig_next_menu = menus[i+1];
				}

			}

			if(Settings.debug && Settings.menu_to_jump_to != "")
				current_menu = GetChoiceMenuFromName(Settings.menu_to_jump_to);
			else
				current_menu = menus[0];
		}
		
		subtitle.SetText(current_menu.original_prompt);

	}
	
	public DialogMenu GetChoiceMenuFromName(string aname)
	{
		foreach(DialogMenu menu in menus)
		{
			if(menu.name == aname)
				return menu;
		}
		return null;
	}
	
	void SetResponsesFromXML()
	{
		good_responses = new List<ActionResponse>();
		bad_responses = new List<ActionResponse>();
		
		TextAsset textAsset = (TextAsset) Resources.Load("XMLs/action_responses");
		
		XmlReader reader = XmlReader.Create(new StringReader(textAsset.text));
		
		bool positive = true;
		ActionResponse current_response = new ActionResponse();
		string current_node = "";
		
		while(reader.Read())
		{
			if(reader.NodeType == XmlNodeType.Element)
			{
				if(reader.Name == "Negative")
				{
					positive = false;
				}
				if(reader.Name == "Response")
				{
					current_response = new ActionResponse();
				}
				if(reader.Name == "EnglishText")
				{
					current_node = "EnglishText";
				}
				else if(reader.Name == "ChineseText")
				{
					current_node = "ChineseText";
				}
			}
			else if(reader.NodeType == XmlNodeType.Text)
			{
				if(current_node == "ChineseText")
				{
					string val = StringUtils.RemoveNewlineAndIndents(reader.Value);
					current_response.chinese_text = val;
					if(positive)
					{
						good_responses.Add(current_response);
					}
					else
					{
						bad_responses.Add(current_response);
					}
				}
				else if(current_node == "EnglishText")
				{
					string val = StringUtils.RemoveNewlineAndIndents(reader.Value);
					current_response.english_text = val;
				}
				
			}
		}
	}
	
	void CreateTreeFromXML()
	{
		TextAsset textAsset = (TextAsset) Resources.Load("XMLs/tea_dialog");  
		
		XmlReader reader = XmlReader.Create(new StringReader(textAsset.text));
		
		DialogMenu current_dialog_menu = null;
		DialogWidget current_widget = null;
		
		string last_element = "";
		string last_attr_name = "";
		string last_attr_val = "";
		
		while(reader.Read())
		{
			if(reader.NodeType == XmlNodeType.Element)
			{
				last_element = reader.Name;
				
				if(last_element == "Prompt")
				{
					current_dialog_menu = Instantiate(choice_menu_prefab,menu_position,Quaternion.identity) as DialogMenu;
					
					//Set the submenus to the speficied submenu scale, and set their rotation to the same rotation as this.
					current_dialog_menu.transform.localScale = menu_scale;
					
					current_dialog_menu.transform.parent = transform;
					current_dialog_menu.transform.localRotation = Quaternion.identity;

					menus.Add(current_dialog_menu);
					
				}
				else if(last_element == "Choice")
				{
					current_widget = Instantiate(dialog_widget_prefab,Vector3.zero,current_dialog_menu.background.transform.rotation) as DialogWidget;
					
					current_widget.transform.localScale = menu_scale;
					
					current_dialog_menu.AddWidget(current_widget);
				}
				while (reader.MoveToNextAttribute()) // Read the attributes.
				{
					last_attr_name = reader.Name;
					last_attr_val = reader.Value;
					
					if(last_element == "Prompt")
					{
						if(last_attr_name == "id")
						{
							AudioClip next_clip;
							current_dialog_menu.name = last_attr_val;
							if(last_attr_val == "Preamble1")
								next_clip = clips.Find(clip => clip.name == "Welcome(mandarin)");
							else if(last_attr_val == "Preamble2")
								next_clip = clips.Find(clip => clip.name == "TodayIWillTeachYou");
							else
								next_clip = clips.Find(clip => clip.name == last_attr_val);
							
							if(next_clip == null)
								Debug.Log(last_attr_val + " failed to generate a clip.");
						
							// note - name the animations Preamble1 and Preamble2
							current_dialog_menu.SetDialogClip(next_clip,test_audio_source,last_attr_val,animatedCharacterName);
						}
						else if(last_attr_name == "progression")
						{
							//Set the current menu's progression here
							if(last_attr_val == "wait")
							{
								current_dialog_menu.dialog_tree_state = DialogTreeState.Wait;
							}
							else if(last_attr_val == "choice")
							{
								current_dialog_menu.dialog_tree_state = DialogTreeState.Choice;
							}
						}
						else if(last_attr_name == "haltUntilGoodAction")
						{
							current_dialog_menu.halt_until_good_action = true;
						}
						else if(last_attr_name == "haltAtEnd")
						{
							current_dialog_menu.halt_at_end = true;
						}
						else if(last_attr_name == "displayResponseForAction")
						{
							current_dialog_menu.display_response_for_action = true;
						}
					}
					else if(last_element == "Choice")
					{
						//Create a dialog widget 
						if(last_attr_name == "id")
						{
							current_widget.name = last_attr_val;
							current_widget.animationClip = last_attr_val;
							AudioClip next_clip = clips.Find(clip => clip.name == last_attr_val);
							
							if(next_clip == null)
								Debug.Log(last_attr_val + " failed to generate a clip.");
							
							current_widget.audio_clip = next_clip;
							
						}
						else if(last_attr_name == "gotoPromptId")
						{
							current_widget.choicemenu_name = last_attr_val;
						}
						else if(last_attr_name == "haltUntilGoodAction")
						{
							current_widget.halt_until_good_action = true;
						}
						else if(last_attr_name == "excludeChoice")
						{
							string[] excluded = last_attr_val.Split(',');
							current_widget.excluded_choices = excluded;
						}
					}
				}
			}
			else if(reader.NodeType == XmlNodeType.Text)
			{
				if(last_element == "EnglishSubtitles")
				{
					string val = reader.Value;
					
					//Remove all newlines and whitespace following newlines from val
					val = StringUtils.RemoveNewlineAndIndents(val);
					
					current_dialog_menu.original_prompt = val;
				}
				else if(last_element == "ChineseText")
				{
					//current_widget.SetText(reader.Value);
					current_widget.chinese_text = reader.Value;
				}
				else if(last_element == "EnglishText")
				{
					//current_widget.AppendText("\n(" + reader.Value + ")");
					current_widget.english_text = reader.Value;
				}
				else if(last_element == "Pinyin")
				{
					current_widget.pinyin = reader.Value;
				}
				else if(last_element == "EnglishResponse")
				{
					//Remove all newlines and whitespace following newlines from val
					string val = StringUtils.RemoveNewlineAndIndents(reader.Value);
					
					current_widget.response = val;
				}
			}
		}
	}

	//Take a widget, and extract its excluded choices, into excluded_choices list
	public void AppendExcluded(Widget w)
	{
		foreach(string s in (w as DialogWidget).excluded_choices)
		{
			excluded_choices.Add(s);
		}
	}

	//What happens when a widget was clicked?
	public void OnWidgetClick(Widget widget)
	{
		widget.Disable();
		
		AppendExcluded(widget);
		
		DialogMenu next_menu;

		//Widget requires a halt? Then wait for action
		if((widget as DialogWidget).halt_until_good_action)
		{
			current_menu.dialog_tree_state = DialogTreeState.Action;
		}

		//If we exhausted all choices, jump straight to the next menu
		if(current_menu.AllWidgetsDisabled())
		{
			next_menu = current_menu.orig_next_menu;

			//Halt at end? Wait for action
			if(current_menu.halt_at_end)
			{
				current_menu.dialog_tree_state = DialogTreeState.Action;
			}
		}
		//Otherwise:
		else
		{
			//Go to the selected widget's destination
			next_menu = (widget as DialogWidget).DialogMenu;
			//If that destination is a new menu: disable all current widgets.
			if(next_menu != current_menu)
			{
				current_menu.DisableAllWidgets();
			}
		}
	
		//Set the current menu's subtitle to the widget's response
		subtitle.SetText((widget as DialogWidget).response);

		last_response = (widget as DialogWidget).response;

		//Change it's audio clip to the new one
		current_menu.SetDialogClip((widget as DialogWidget).audio_clip,test_audio_source,(widget as DialogWidget).animationClip,animatedCharacterName);
		
		PrepareChangeMenu(next_menu);
	}

	bool set_visible_once = false;

	// Update is called once per frame
	void Update () 
	{
		if(action_indicator == null)
			action_indicator = GameObject.Find("ActionIndicator").gameObject;

		if(!set_visible_once)
		{
			foreach(Menu menu in menus)
			{
				menu.SetVisible(false);
			}
			set_visible_once = true;
		}

		DisableAllNonCurrentMenus();

		if(Settings.debug)
		{
			Settings.HideAndLockCursor();
		}
		/*
		if(Input.GetKeyDown(KeyCode.Equals) && Settings.debug)
		{
			OnPlayerActionSuccess();
		}
		*/
		if(Input.GetKeyDown (KeyCode.Minus))
		{
			OnPlayerActionFail();
		}

		//We use the state variable to only prepare the menu change once
		if(current_menu.dialog_tree_state == DialogTreeState.Wait)
		{
			current_menu.dialog_tree_state = DialogTreeState.Halt;

			PrepareChangeMenu(current_menu.next_menu);
		}

		//If the current menu was preset to be a choice state:
		else if(current_menu.dialog_tree_state == DialogTreeState.Choice)
		{
			//Make the current menu invisible
			current_menu.SetVisible(false);
			
			//If the audio is finished, make the menu visible
			if(current_menu.dialog_clip == null || current_menu.dialog_clip.IsClipDone())
			{
				//The menu is scheduled to halt for action after initial speech? Set state to action, don't enable choices yet
				if(current_menu.halt_until_good_action && !current_menu.halt_at_end)
				{
					current_menu.dialog_tree_state = DialogTreeState.Action;
					StartAllowAction();
				}
				else
				{
					current_menu.gameObject.SetActive(true);
					current_menu.SetVisible(true);
					current_menu.dialog_tree_state = DialogTreeState.Halt;
				}
			}
			else
			{
				//Start playing the audio
				current_menu.dialog_clip.StartClip();
			}
		}
		//If we're in an action state: 
		else if(current_menu.dialog_tree_state == DialogTreeState.Action && !current_menu.start_check_for_end_cond)
		{
			action_timer += Time.deltaTime;
			if(action_timer >= MAX_ACTION_TIME)
			{
				action_timer = 0;
				OnPlayerActionFail();
			}
		}
		
	}
	
	public void StartAllowAction()
	{
		Debug.Log ("startallowaction() called");

		//If we selected a response that then leads to an action, and this variable is set to true
		//on the current menu, then display the response's text when she finishes talking
		if(current_menu.display_response_for_action)
			subtitle.SetText(last_response);
		else
			subtitle.SetText(current_menu.original_prompt);

		current_menu.start_check_for_end_cond = false;
		current_menu.dialog_tree_state = DialogTreeState.Action;
		current_menu.SetVisible(false);
		
		EnablePlayerAction();
	}
	
	public void EnablePlayerAction() 
	{
		action_indicator.renderer.enabled = true;
        playerTeaCeremony.enabled = true;
		//pickup_object_script.enabled = true;
	}
	
	public void DisablePlayerAction()
	{
		action_indicator.renderer.enabled = false;
        playerTeaCeremony.enabled = false;
		//pickup_object_script.enabled = false;
	}

	//Helper function used by the two callbacks OnPlayerActionSuccess() and OnPlayerActionFail()
	private void ActionAdvance()
	{
		action_timer = 0;

		//We're only supposed to halt for action at beginning? Don't move on to next menu
		if(current_menu.halt_until_good_action && !current_menu.halt_at_end)
		{
			current_menu.halt_until_good_action = false;

			if(!current_menu.AllWidgetsDisabled()) {
				current_menu.dialog_tree_state = DialogTreeState.Choice; 
			}
			else {
				current_menu.dialog_tree_state = DialogTreeState.Wait;
			}
		}
		//We're halting at the end. Move on to next menu
		else
		{
			current_menu.halt_until_good_action = false;
			current_menu.start_check_for_end_cond = true;
			current_menu.dialog_tree_state = DialogTreeState.Halt;
			
			//If we aren't checking for an end condition: Reset to current menu
			PrepareChangeMenu(current_menu.next_menu);

		}

		DisablePlayerAction();
	}

	//Callback when player's action has succeeded.
	//This shouldn't be called when the current menu is already checking for an end condition. An error would be thrown then.
	public void OnPlayerActionSuccess()
	{
		//If dialog is speaking, or dialog tree state is not action state: Don't do this function
		if(current_menu.start_check_for_end_cond || current_menu.dialog_tree_state != DialogTreeState.Action)
		{
			return;
		}

		//Set the current menu's subtitle to the widget's response
		subtitle.SetText(good_responses[good_response_index].english_text);
		current_menu.SetDialogClip(good_response_clips[good_response_index],test_audio_source,"",animatedCharacterName);

		good_response_index++;
		if(good_response_index > good_responses.Count - 1){ good_response_index = 0; }

		ActionAdvance();

	}
	
	//Callback when player's action has failed.
	//This shouldn't be called when the current menu is already checking for an end condition. An error would be thrown then.
	public void OnPlayerActionFail()
	{
		if(current_menu.start_check_for_end_cond || current_menu.dialog_tree_state != DialogTreeState.Action)
		{
			return;
		}
		//Set the current menu's subtitle to the widget's response
		subtitle.SetText(bad_responses[bad_response_index].english_text);
		current_menu.SetDialogClip(bad_response_clips[bad_response_index],test_audio_source,"",animatedCharacterName);

		bad_response_index++;
		if(bad_response_index > bad_responses.Count - 1){ bad_response_index = 0; }

		ActionAdvance();

		// automatically advance to next step if player runs out of time
		playerTeaCeremony.EndCheck();
	}

	//Gear up the tree to prepare to change to a new menu
	public void PrepareChangeMenu(DialogMenu next_menu)
	{
		current_menu.StartEndCond();
		current_menu.next_menu = next_menu;
		current_menu.OnEndCond += OnEndCond;
	}
	
	//What happens when the end condition is actually met
	public void OnEndCond(DialogMenu firer, DialogMenu next_menu)
	{
		//Disregard events fired by menus that aren't the current menu
		if(firer != current_menu){ return; }

		//If we're in an action state, don't move on. Just start the allowing of the actio
		if(current_menu.dialog_tree_state == DialogTreeState.Action)
		{
			StartAllowAction();
			return;
		}
		//If the menu is scheduled to halt: start the allowed action. Don't do this if the next menu is the same, as it means
		//we are returning to the same menu, and we don't do checks there.
		if(current_menu.halt_until_good_action && current_menu != next_menu)
		{
			StartAllowAction();
		}
		else
		{
			ChangeMenu(next_menu);
		}
	}
	
	//Change to the next menu.
	public void ChangeMenu(DialogMenu next_menu)
	{
		//If we are advancing to a new menu: Advance
		if(current_menu != next_menu)
		{
			if(next_menu)
				next_menu.SetVisible(true);

			current_menu = next_menu;

			if(current_menu)
				subtitle.SetText(current_menu.original_prompt);
			else
				OnFinish();
		}
		//If we are staying at the same menu: Don't advance and reset timers and original prompt
		else
		{
			current_menu.DeselectAllWidgets();
			current_menu.ResetEndCond();
						
			subtitle.SetText(current_menu.original_prompt);
		}
		
		subtitle = GetComponentInChildren<SubtitleWidget>();
		
		if(!subtitle)
		{
			Debug.Log ("Error: no subtitle");
			Destroy(gameObject);
		}
	}
	
	//Disable all menus that are not the current, focused menu.
	void DisableAllNonCurrentMenus()
	{
		foreach(DialogMenu menu in menus)
		{
			if(menu != current_menu)
			{
				menu.gameObject.SetActive(false);
			}
		}
		if(current_menu)
		{
			current_menu.gameObject.SetActive(true);
			current_menu.RemoveExcludedWidgets();
		}
	}
	
	//Code to call when the dialog tree is finished
	void OnFinish()
	{
		Destroy(gameObject);
	}
	
}
