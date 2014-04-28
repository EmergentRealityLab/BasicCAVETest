using UnityEngine;
using System.Collections;

public class TeaScoopScript : MonoBehaviour {

	private bool teaCollected;
	private bool inRange;
	private bool lockedIn;
	private TeapotScript teapot;
	private PickupObject pickupObject;
	private Renderer teaClump;
	private Vector3 initialPosition;
	private Quaternion initialRotation;

	// Use this for initialization
	private void Start () 
	{
		initialPosition = transform.position;
		initialRotation = transform.rotation;
		teaCollected = false;
		inRange = false;
		lockedIn = false;
		teapot = GameObject.FindGameObjectWithTag("Teapot").GetComponent<TeapotScript>();
		pickupObject = GameObject.Find("MainPlayer").GetComponent<PickupObject>();
		teaClump = GameObject.Find("TeaClump").renderer;
	}
	
	// Update is called once per frame
	private void Update () 
	{
		if(Controls.ResetObjects())
		{
			transform.position = initialPosition;
			transform.rotation = initialRotation;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		//collecting tea
		if(other.name.Contains("TeaLeaves"))
		{
			//print("tea collected");
			teaCollected = true;
			teaClump.enabled = true;
		}
		else if(other.name.Contains("teapotLeafDrop"))
		{
			inRange = true;
		}
		else if(other.name == "scoopPosition" && !lockedIn)
		{
			lockedIn = true;
			if(pickupObject.pickedUpObject == gameObject)
			{
				pickupObject.Drop();
			}
			
			transform.position = other.transform.position;
			transform.rotation = other.transform.rotation;
			transform.parent = other.transform.parent;
			rigidbody.isKinematic = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.name.Contains("teapotLeafDrop"))
		{
			inRange = false;
		}
		else if(other.name == "scoopPosition")
		{
			lockedIn = false;
		}
	}

	public void Drop()
	{
		if(teaCollected && inRange && !teapot.topOn)
		{
			teaCollected = false;
			teapot.teaLeafCount++;
			teaClump.enabled = false;
		}
	}
}
