using UnityEngine;
using System.Collections;

//This class will launch the demo scene specified in the variable "string".
public class BeginDemo : MonoBehaviour 
{
	public string scene_to_load;
	public static bool started_from_begin;
	
	// Use this for initialization
	void Start () 
	{
		started_from_begin = true;
		
		Application.LoadLevel(scene_to_load);
		//networkView.RPC("DoItDoItDoIt", RPCMode.All);
	}
	
	[RPC]
	void DoItDoItDoIt()
	{
		Application.LoadLevel(scene_to_load);
	}
	
}
