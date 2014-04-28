using UnityEngine;
using System.Collections;

/*
 * SubtitleMenu was a menu originally intended for use by NPCs; when you would click them and they would say a line.
 * However, this never got implemented, so SubtitleMenu currently isn't being used anywhere.
 * 
 * The idea is to contain a DialogClip so that it can animate the NPC and play its voice clip as well.
*/
public class SubtitleMenu : Menu 
{
	public bool on;
	public string subtitle_text;
	public DialogClip subtitle_clip;
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(on)
		{
			subtitle.gameObject.SetActive(true);
			
			if(subtitle_clip != null) 
			{
				subtitle_clip.StartClip();
				
				if(subtitle_clip.IsClipDone())
				{
					on = false;
				}
			}
		}
		else
		{
			if(subtitle_clip != null)
				subtitle_clip.Reset();
			
			subtitle.gameObject.SetActive(false);
		}
	}
	
	public bool IsClipDone()
	{
		return subtitle_clip == null || subtitle_clip.IsClipDone();
	}
	
	//Set the dialog clip.
	public void SetDialogClip(AudioClip aclip,AudioSource source,string animationClip,string animatedCharacterName)
	{
		if(aclip == null)
		{
			Debug.LogError("A dialog clip entered was null!");
			return;
		}
		else if(source == null)
		{
			Debug.LogError("Source is null!");
			return;
		}
		if(subtitle_clip == null)
		{
			subtitle_clip = new DialogClip(aclip,source,animationClip, animatedCharacterName);
		}
		else
		{
			subtitle_clip.Reset();
			subtitle_clip.clip = aclip;
		}
	}
	
	public void TurnOn()
	{
		on = true;
	}
	
	public SubtitleWidget subtitle
	{
		get
		{
			return widgets[0] as SubtitleWidget;
		}
	}
}
