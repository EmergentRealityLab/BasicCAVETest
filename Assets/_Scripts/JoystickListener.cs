using UnityEngine;
using System.Collections;

public class JoystickListener : MonoBehaviour {
	
	GameObject pointer;
	
	// Use this for initialization
	void Start () {
		pointer = GameObject.Find("Pointer");
	}
	
	// Update is called once per frame
	void Update () {
		pointer.renderer.material.color = new Color(Input.GetAxis("Fire3"), 1 - Input.GetAxis("Fire3"), 0);

	}
}
