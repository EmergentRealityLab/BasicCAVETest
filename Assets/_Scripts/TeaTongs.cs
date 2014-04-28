using UnityEngine;
using System.Collections;

public class TeaTongs : MonoBehaviour {

	public Animator anim;
	public PointerControl pointerControl;
	public GameObject child;

	public bool drop_lock;
	private bool lockedIn;
	private PickupObject pickupObject;
	private Vector3 initialPosition;
	private Quaternion initialRotation;

	// Use this for initialization
	private void Start () 
	{
		lockedIn = false;
		anim = GetComponent<Animator>();
		pointerControl = GameObject.Find("PointerRoot").GetComponent<PointerControl>();
		pickupObject = GameObject.Find("MainPlayer").GetComponent<PickupObject>();
		initialPosition = transform.position;
		initialRotation = transform.rotation;
	}
	
	// Update is called once per frame
	private void Update () {
		if(Controls.ResetObjects())
		{
			transform.position = initialPosition;
			transform.rotation = initialRotation;
		}
		//anim.Play();
	}

	public void LateUpdate()
	{
		drop_lock = false;
	}

	public bool IsHeld()
	{
		return transform.parent == pointerControl.transform;
	}

	public void SetChild(GameObject achild)
	{
		child = achild;
		child.rigidbody.isKinematic = true;
		child.transform.parent = transform.parent;
	}

	public void RemoveChild()
	{
		if(child != null)
		{
			child.transform.parent = null;
			child.rigidbody.isKinematic = false;
			child = null;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.name == "tongsPosition" && !lockedIn)
		{
			lockedIn = true;
			pickupObject.Drop();
			pickupObject.Drop();

			transform.position = other.transform.position;
			transform.rotation = other.transform.rotation;
			transform.parent = other.transform.parent;
			rigidbody.isKinematic = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.name == "tongsPosition")
		{
			lockedIn = false;
		}
	}

}


