using UnityEngine;
using System.Collections;

public class PointerTestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1"))
		{
			Debug.Log ("fire 1");
		}
				if (Input.GetButtonDown("Fire2"))
		{
			Debug.Log ("fire 2");
		}
	}
}
