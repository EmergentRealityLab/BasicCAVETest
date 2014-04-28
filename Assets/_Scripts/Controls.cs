using UnityEngine;
using System.Collections;

//Put all controls in this center class of reference
public class Controls : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static bool MainButton()
	{
		return Input.GetMouseButton(0) || Input.GetButton("Fire1");
	}	
	
	public static bool SecondButton()
	{
		return Input.GetMouseButton(1) || Input.GetButton("Fire2");
	}

	public static bool MainButtonDown()
	{
		return Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1");
	}	

	public static bool SecondButtonDown()
	{
		return Input.GetMouseButtonDown(1) || Input.GetButtonDown("Fire2");
	}
	
	public static bool FastForwardButton()
	{
		if(Settings.deploy_debug)
			return (Input.GetKey(KeyCode.BackQuote) || (Input.GetButton("Fire1") && Input.GetButton("Fire2")));
		else
			return false;
	}
	
	public static bool CheatButton()
	{
		return Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2");
	}

	public static bool Reset()
	{
		return (Settings.debug && Input.GetKeyDown(KeyCode.F10)) || (!Settings.debug && Input.GetKeyDown(KeyCode.Escape));
	}

	public static bool ResetObjects()
	{
		return ((Input.GetAxis("Trigger") > 0.5) && (Input.GetButtonDown("Shoulder")));
	}
}
