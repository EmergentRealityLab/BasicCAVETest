using UnityEngine;
using System.Collections;

//This script simply disables the "Debug" camera when we're running the game on the actual lab.
public class DebugCamera : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		if(!Settings.debug)
		{
			gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
