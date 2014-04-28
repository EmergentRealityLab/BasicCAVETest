using UnityEngine;
using System.Collections.Generic;
using System;

//The current dialog menu of the dialog tree can be in one of several states
public enum DialogTreeState
{
	Choice,	//Waiting for player to choose an action
	Wait,	//Wait for Mrs. Ling's dialog to finish
	Action,	//Wait for player to successfully complete an action
	Halt	//A state where none of the above are true. Used often in transitions between states.
}

//DialogClip encapsulates the state and behavior of a dialog clip in its own class.
//It also contains animation data now, so it's name could be refactored to something like "AnimClip".

// starts the audio, and should start the animation playing as well. 
// so needs the animatorcontroller for the character model, and the name of the clip to play.  string, same as name of audio.
public class DialogClip
{
	public AudioClip clip;
	public AudioSource audio_player;
	public bool one_frame_passed;
	public string animationClip;
	public string animatedCharacterName;
	public Animator anim; 

	public DialogClip(AudioClip aclip,AudioSource aaudio_player, string _animationClip, string _animatedCharacterName)
	{
		//Debug.Log("Set clip " + aclip + " onto audio source " + aaudio_player);
		if(aaudio_player == null)
		{
			Debug.LogError("Audio player is null! Errors will happen!");
			return;
		}
		audio_player = aaudio_player;
		clip = aclip;
		animationClip=_animationClip;
		animatedCharacterName = _animatedCharacterName;
		anim=null;

	}
	public void StartClip()
	{
		//While playing, calling this will have no effect
		if(audio_player.isPlaying)
		{
			return;
		}
		audio_player.clip = clip;
		audio_player.Stop();
		audio_player.Play();

		// FIXME
		// the animated character may not exist at the time this object is created (might be in a different scene, etc)
		// so, for a temporary workaround, at this point in time we will search the scene for the GameObject we want by name,
		// get the Animator component from it, and play the clip.

		if (anim==null)
		{
			GameObject obj=GameObject.Find(animatedCharacterName);
			if (obj!=null)
			{
				anim=obj.GetComponent<Animator>();

			}
		}
		
		if (anim != null) {
			GameObject obj=GameObject.Find(animatedCharacterName);
			Debug.Log("sending message from DialogMenu to call playAnimation on Mrs. Ling");
			obj.SendMessage("playAnimation", animationClip);
			Debug.Log("sent message from DialogMenu to call playAnimation on Mrs. Ling");

			//anim.Play (animationClip);

			// turn off the head look controller if I'm clip "L"

			if (animationClip == "L")
			{
				GameObject headCtrl = GameObject.Find("HeadController");
				if (headCtrl!=null)
				{
					headCtrl.SetActive(false);
				}
				//Player.player.playFinalAnim();
			}

			/*
			for(int i = 1; i < AnimationIndices.animIndices.Count; i++){
				if(animationClip == AnimationIndices.animIndices[i].ToString ()){
					Debug.Log (i + " : " + animationClip);
					anim.SetFloat ("whichAnimation", i - 0.5f);
				}
			}*/
			
		}

					       
	}

	//Is the clip done playing?
	//Note: if you also want the animation to be done playing for this method to return true, you can add an && boolean operator
	//to the if(!audio_player.isPlaying) specificying that the animation is not playing. That way the clip
	//will only be done if the animation is done as well.
	public bool IsClipDone()
	{
		if(Controls.FastForwardButton())
			audio_player.pitch = 100;
		else
			audio_player.pitch = 1;
		
		
		if(!one_frame_passed)
		{
			one_frame_passed = true;
			return false;
		}
		if(!audio_player.isPlaying)
		{
			anim.SetFloat ("whichAnimation", 0);
			one_frame_passed = false;
			return true;
		}
		return false;
	}

	//Reset the dialog clip, stopping it.
	public void Reset()
	{
		one_frame_passed = false;
		audio_player.Stop();
	}
}

/*
 * This is a menu that is designed to be used in dialog trees.
 * It gives a list of options. You then select an option.
 * You can configure what happens next, i.e. what dialog menu to jump to next.
 */
public class DialogMenu : Menu 
{
	public TeaDialogTree tree;
	
	public DialogTreeState dialog_tree_state;
	
	public string original_prompt;	//The original string that this menu had.
	
	public delegate void OnEndCondEvent(DialogMenu firer, DialogMenu next_menu);
  	public event OnEndCondEvent OnEndCond;	//Event that's fired when the current dialog is finished, and we are ready to advance to the next menu.

	public DialogMenu next_menu;		//Next menu to jump to, can be set.
	public DialogMenu orig_next_menu;	//The next menu to go to eventually, when all choices are exausted.
	
	public bool start_check_for_end_cond;	//Starts checking for the end condition of this menu, once the player has selected a choice.
	
	public float end_cond_timer;	//If a simple timer is used for the end condition, then this is the end condition timer.
	private static float MAX_WAIT_TIME = 1000f;
	
	public float delay; //The delay when the audio clip finishes, and the menu's end condition happens.
	float delay_timer;
	
	public DialogClip dialog_clip;

	public bool halt_until_good_action;
	
	public bool halt_at_end;

	public bool display_response_for_action;

	//Set the dialog clip.
	// give it the audio clip, the AudioSource to play the clip, the name of the animation clip, and the Animator component 
	public void SetDialogClip(AudioClip aclip,AudioSource source, string animationClip, string animatedCharacterName)
	{
		if(aclip == null)
		{
			Debug.LogWarning("A dialog clip entered was null!");
			return;
		}
		else if(source == null)
		{
			Debug.LogError("Source is null!");
			return;
		}
		if(dialog_clip == null)
		{
			dialog_clip = new DialogClip(aclip,source,animationClip,animatedCharacterName);
		}
		else
		{
			dialog_clip.Reset();
			dialog_clip.clip = aclip;
			dialog_clip.animationClip=animationClip;
			dialog_clip.animatedCharacterName=animatedCharacterName;
		}
	}
	
	//Check for the condition that will cause the next dialog menu to fire.
	//Depending on the menu's name (i.e. what stage we are in the ceremony) different things will cause it. 
	//I.e. when the audio file stops playing, when Mrs. Ling's animation stops, etc.
	public void CheckForEndCondition()
	{
		if(dialog_clip == null) { return; }
		
		if(dialog_clip.IsClipDone() || (Settings.debug && Input.GetKeyDown(KeyCode.Mouse1)))
		{
			delay_timer += Time.deltaTime;
			if(delay_timer >= delay)
			{
				delay_timer = 0;
				dialog_clip.Reset();
				start_check_for_end_cond = false;
				OnEndCond(this,next_menu);
			}
		}
	}

	//Start the end condition trigger. This is only called once.
	public void StartEndCond()
	{
		end_cond_timer = 0f;
		start_check_for_end_cond = true;
		
		if(dialog_clip != null)
		{
			dialog_clip.StartClip();
		}
	}
	
	//Reset the end condition
	public void ResetEndCond()
	{
		end_cond_timer = 0f;
		start_check_for_end_cond = false;
	}
	
	new void Awake()
	{
		base.Awake();
	}
	
	// Use this for initialization
	new void Start () 
	{
		if(dialog_tree_state == DialogTreeState.Wait)
			start_check_for_end_cond = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(dialog_tree_state == DialogTreeState.Wait)
		{
			start_check_for_end_cond = true;
		}
		
		if(start_check_for_end_cond)
		{
			CheckForEndCondition();
		}
		
		if(Controls.CheatButton())
		{
			if(!AllWidgetsDisabled() && !hidden)
				ToggleHint();
		}
	}

	//Remove all widgets that were specified to be removed in the XML depending on certain paths in the dialog tree (see script.)
	public void RemoveExcludedWidgets()
	{
		bool removed = false;
		foreach(string widget_name in tree.excluded_choices)
		{
			for(int i=widgets.Count-1;i>=0;i--)
			{
				if(widgets[i].name == widget_name)
				{
					RemoveWidget(widgets[i]);
					removed = true;
				}
			}
		}
		if(removed)
			ArrangeWidgets();
	}
	
	public void ToggleHint()
	{
		for(int i=0;i<widgets.Count;i++)
		{
			(widgets[i] as DialogWidget).ToggleHint();
		}
	}
	
	//Event that's fired when submit button is clicked.
	public override void OnSubmit(SubmitButton submitButton)
	{
	}
	
	//Event that's fired when a widget is selected
	public override void OnWidgetClick(Widget widget)
	{
	}
	
}
