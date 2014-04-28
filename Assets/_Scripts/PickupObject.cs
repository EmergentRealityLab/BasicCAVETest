using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickupObject : MonoBehaviour {

	public GameObject pickedUp;

	public GameObject pickedUpObject
	{
		get
		{
			if(pickedUp != null && pickedUp.GetComponent<TeaTongs>() != null && pickedUp.GetComponent<TeaTongs>().child != null)
			{
				return pickedUp.GetComponent<TeaTongs>().child;
			}
			else
			{
				return pickedUp;
			}
		}
		set
		{
			if(pickedUp != null && pickedUp.GetComponent<TeaTongs>() != null && pickedUp.GetComponent<TeaTongs>().child != null)
			{
				pickedUp.GetComponent<TeaTongs>().child = value;
			}
			else
			{
				pickedUp = value;
			}
		}
	}

	public GameObject tea;
	public GameObject water;

	private float dropTimer;
	private float totalRotationZ;
	private bool holding;
	private Transform priorParent;
	private PointerControl pointerControl;
	private PlayerTeaCeremony playerTeaCeremony;

	public void Reset()
	{

	}

	// Use this for initialization
	private void Start () 
	{
		holding = false;
		totalRotationZ = 0.0f;
		pointerControl = GameObject.Find("PointerRoot").GetComponent<PointerControl>();
		playerTeaCeremony = GameObject.Find("MainPlayer").GetComponent<PlayerTeaCeremony>();

		if(!Settings.start_allow_actions)
		{
			this.enabled = false;
		}
	}
	
	// Update is called once per frame
	private void Update () 
	{		
		PlayerInput();

		if(!holding)
		{
			LaserControl.Laser.gameObject.SetActive(true);
		}
		else
		{
			LaserControl.Laser.gameObject.SetActive(false);
			if(totalRotationZ > 30.0f && totalRotationZ < 180f)
			{
				if(pickedUpObject == null)
				{
					return;
				}
				if(!playerTeaCeremony.CanPour(pickedUpObject.transform.position.y))
				{
					return;
				}

				if(pickedUpObject.transform.tag.Contains("Teacup"))
				{
					pickedUpObject.GetComponent<TeacupScript>().SetRootLeft();
					pickedUpObject.GetComponent<TeacupScript>().Pour(true);
				}
				else if(pickedUpObject.transform.tag.Contains("Pitcher"))
				{
					pickedUpObject.GetComponent<PitcherScript>().Pour(true);
				}
				else if(pickedUpObject.transform.tag.Contains("Teapot"))
				{
					pickedUpObject.GetComponent<TeapotScript>().Pour(true);
				}
				else if(pickedUpObject.transform.tag.Contains("Scoop"))
				{
					pickedUpObject.GetComponent<TeaScoopScript>().Drop();
				}
			}
			else if(totalRotationZ >= 180 && totalRotationZ < 330 && pickedUpObject.transform.tag.Contains("Teacup"))
			{
				pickedUpObject.GetComponent<TeacupScript>().SetRootRight();
				pickedUpObject.GetComponent<TeacupScript>().Pour(true);
			}
			else
			{
				if(pickedUpObject == null)
				{
					return;
				}
				if(pickedUpObject.transform.tag.Contains("Teacup"))
				{
					pickedUpObject.GetComponent<TeacupScript>().SetRootRight();
					pickedUpObject.GetComponent<TeacupScript>().Pour(false);
				}
				else if(pickedUpObject.transform.tag.Contains("Pitcher"))
				{
					pickedUpObject.GetComponent<PitcherScript>().Pour(false);
				}
				else if(pickedUpObject.transform.tag.Contains("Teapot"))
				{
					pickedUpObject.GetComponent<TeapotScript>().Pour(false);
				}
			}
		}
	}
	
	private void PlayerInput()
	{		
		if(Input.GetKeyDown (KeyCode.F1))
		{
			Drop ();
		}

		if(Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1"))
		{
			if (!holding)
				Pickup();
			else if(pickedUpObject.tag != "Tongs")
				Drop();
		}
		
		if(holding)
		{
			totalRotationZ = pointerControl.pointer.eulerAngles.z;
		}
	}

	public void Pickup()
	{
		if(pointerControl.current_hit != null)
		{
			if(pointerControl.current_hit.tag.Contains("Teacup") || pointerControl.current_hit.tag.Contains("Pitcher") || 
			   pointerControl.current_hit.tag.Contains("Teapot") || pointerControl.current_hit.tag.Contains("Tongs")   ||
			   pointerControl.current_hit.tag.Contains("Scoop")  || pointerControl.current_hit.tag.Contains("OtherPickup"))
			{

				if(pointerControl.current_hit.tag.Contains("OtherPickup"))
				{
					priorParent = null;
				}
				else
				{
					priorParent = pointerControl.current_hit.transform.parent;
				}
				
				pickedUpObject = pointerControl.current_hit;

				if(pointerControl.current_hit.tag.Contains("Tongs"))
				{
					pickedUpObject.transform.eulerAngles = new Vector3(90,0,0);
				}
				else if(pointerControl.current_hit.tag.Contains("Scoop"))
				{
					//value to be adjusted
					pickedUpObject.transform.eulerAngles = new Vector3(0,90,0);
				}
				else
				{
					pickedUpObject.transform.localRotation = Quaternion.identity;
				}
				

				pickedUpObject.transform.parent = pointerControl.gameObject.transform;
				pickedUpObject.rigidbody.isKinematic = true;
				totalRotationZ = 0.0f;
				holding = true;
			}
		}
	}

	public void Drop()
	{
		if(pickedUp == null && pickedUpObject == null)
		{
			return;
		}

		if(pickedUp.GetComponent<TeaTongs>() && pickedUp.GetComponent<TeaTongs>().drop_lock)
		{
			return;
		}

		pickedUpObject.rigidbody.isKinematic = false;

		//if (priorParent)
		pickedUpObject.transform.parent = null;//priorParent;

		if(pickedUpObject.tag.Contains("Teapot"))
		{
			pickedUpObject.GetComponent<TeapotScript>().Pour(false);
		}
		else if(pickedUpObject.tag.Contains("Teacup"))
		{
			pickedUpObject.GetComponent<TeacupScript>().Pour(false);
		}
		else if(pickedUpObject.tag.Contains("Pitcher"))
		{
			pickedUpObject.GetComponent<PitcherScript>().Pour(false);
		}

		pickedUpObject = null;

		if(pickedUpObject && pickedUpObject.GetComponent<TeaTongs>())
		{
			pickedUpObject.GetComponent<TeaTongs>().RemoveChild();
		}
		else
		{
			holding = false;
		}

	}
}
