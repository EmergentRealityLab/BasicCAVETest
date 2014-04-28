using UnityEngine;
using System.Collections;

public class TeaContainerLidScript : MonoBehaviour 
{
	
	private PickupObject pickupObject;
	private bool lockedIn;

	private void Start () 
	{
		pickupObject = GameObject.Find("MainPlayer").GetComponent<PickupObject>();	
		lockedIn = false;
	}
	
	private void Update () 
	{
		
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if(other.name == "containerLidPosition" && !lockedIn)
		{
			if(pickupObject.pickedUpObject == gameObject)
			{
				pickupObject.Drop();
			}
			
			transform.position = other.transform.position;
			transform.parent = other.transform.parent;
			rigidbody.isKinematic = true;
			lockedIn = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.name == "containerLidPosition")
		{
			lockedIn = false;
		}
	}
}