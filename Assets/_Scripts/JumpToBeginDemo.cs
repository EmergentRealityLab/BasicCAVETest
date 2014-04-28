using UnityEngine;
using System.Collections;

public class JumpToBeginDemo : MonoBehaviour {
	
	public static bool once = false;
	
	void Awake () 
	{
		if(!once && !BeginDemo.started_from_begin)
		{	
			once = true;
			Application.LoadLevel("startDemo");
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
