using UnityEngine;
using System.Collections;

public class TeapotLidScript : MonoBehaviour {

	private PickupObject pickupObject;
	private bool lockedIn;
	private Vector3 initialPosition;
	private Quaternion initialRotation;

	private void Start () 
	{
		initialPosition = transform.position;
		initialRotation = transform.rotation;
		pickupObject = GameObject.Find("MainPlayer").GetComponent<PickupObject>();	
		lockedIn = false;
	}

	private void Update () 
	{
		if(Controls.ResetObjects())
		{
			transform.position = initialPosition;
			transform.rotation = initialRotation;
			rigidbody.isKinematic = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.name == "teapotLidPosition" && !lockedIn)
		{
			if(pickupObject.pickedUpObject == gameObject)
			{
				pickupObject.Drop();
			}

			transform.position = other.transform.position;
			transform.parent = other.transform.parent;
			other.transform.parent.GetComponent<TeapotScript>().topOn = true;
			rigidbody.isKinematic = true;
			lockedIn = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.name == "teapotLidPosition")
		{
			other.transform.parent.GetComponent<TeapotScript>().topOn = false;
			lockedIn = false;
		}
	}
}
