using UnityEngine;
using System.Collections;

public class HeadLook : MonoBehaviour {

	public HeadLookController controller;

	// Use this for initialization
	void Start () {
		controller = GameObject.FindObjectOfType<HeadLookController>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.eulerAngles = new Vector3(controller.transform.eulerAngles.x, controller.transform.eulerAngles.y, transform.eulerAngles.z);
	}
}
