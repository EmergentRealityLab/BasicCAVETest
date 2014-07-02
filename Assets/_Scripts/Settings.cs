using UnityEngine;
using System.Collections;

//This class provides global access to settings variables for the game, such as debug on/off, etc.
//Your checks for whether game is debugging or not should reference variables in this class.
public class Settings : MonoBehaviour 
{
	//This has been changed. No longer need to set this variable to true/false every time you switch from
	//editor to build, which was annoying and error-prone. Now is automatically true in editor, false in build.
	public static bool debug
	{
		get
		{
			return false;
			//return Application.isEditor;
		}
	}

	//Turn this on when you are debugging on the big screen in the lab room.
	public static bool deploy_debug = true;

	public static bool start_tea_immediately = false;

	//You turn this boolean on if you want the ability to manipulate the tea set as soon as the game starts.
	//This should only be used for debugging. In the real version, this ability is only turned on in pre-defined
	//moments in the menu sequence.
	public static bool start_allow_actions = true;

	//Set this to the empty string to not jump to any menu
	public static string menu_to_jump_to = "";

	public enum Position
	{
		None,
		Left,
		Center,
		Right
	}

	public static Position position;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public static void HideAndLockCursor()
	{
		Screen.lockCursor = true;
		Screen.showCursor = false;
	}
}
