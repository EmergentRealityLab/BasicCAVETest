using UnityEngine;
using System.Collections;

/*
 * Add this script to the PointerRoot to give it wand controls that are prototypable on a PC setting.
 * (i.e. you can move the wand with the mouse, keyboard, etc.)
 * To aim the wand, use the mouse.
 * To move the wand sideways or up and down, hold shift, then move the mouse sideways or up/down.
 * To move the wand sideways or back and forth (change z position), hold control, then move mouse sideways to move sideways, and up/down to move forward/backward.
 * 
*/
public class Wand : MonoBehaviour 
{
	/*
  	public delegate void OnClickEvent(GameObject g);
  	public event OnClickEvent OnClick;
  	
	public delegate void OnHoverEvent(GameObject g);
  	public event OnHoverEvent OnHover;
  	
	public delegate void OnLeaveEvent();
  	public event OnLeaveEvent OnLeave;
  	*/

	//Only turn this on if you want to use the wand itself to do the raycast, not pointer control.
	//You should only turn this on on the wand prefab.
	//Good if you want to do lightweight testing without having to use the MainPlayer prefab
	public bool use_raycast;

	public Widget current_widget;

	public enum MoveMode
	{
		Rotate,	//Use mouse to aim
		Roll,	//Use mouse to "twist" the cursor
		XY,		//Use mouse to move horizontally and vertically
		XZ		//Use mouse to move horizontally and forward/backward (along z-axis)
	}
	
	MoveMode mode;
	
	// Use this for initialization
	void Start () 
	{
		mode = MoveMode.Rotate;
		if(!Settings.debug)
		{
			enabled = false;
		} 
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Debug.Log (Input.GetAxis ("Vertical"));
		Movement();
		if(use_raycast)
		{
			Settings.HideAndLockCursor();
			PerformRayCast(); 
		}
	}
	
	void Movement()
	{
		//Handle the various modifiers.
		if(Input.GetKey(KeyCode.LeftShift))
		{
			mode = MoveMode.XY;
		}
		else if(Input.GetKey(KeyCode.LeftControl))
		{
			mode = MoveMode.XZ;
		}
		else if(Input.GetKey(KeyCode.R))
		{
			mode = MoveMode.Roll;
		}
		else
		{
			mode = MoveMode.Rotate;
			Vector3 angle = transform.eulerAngles;
			angle.z = 0f;
			transform.eulerAngles = angle;
		}
		
		//Handle the various modes.
		if(mode == MoveMode.Rotate)
		{	
			//transform.Rotate(new Vector3(0f,Input.GetAxis("Mouse X"),0f));
			transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"),0f));	
		}
		else if(mode == MoveMode.Roll)
		{	
			transform.Rotate(new Vector3(0f, 0f, -Input.GetAxis("Mouse X")));	
		}
		else if(mode == MoveMode.XY)
		{
			transform.Translate (new Vector3(Input.GetAxis("Mouse X")*0.25f,0f,0f));
			transform.Translate (new Vector3(0f,Input.GetAxis("Mouse Y")*0.25f,0f));
		}
		else if(mode == MoveMode.XZ)
		{
			transform.Translate (new Vector3(Input.GetAxis("Mouse X")*0.25f,0f,0f));
			transform.Translate (new Vector3(0f,0f,Input.GetAxis("Mouse Y")*0.25f));
		}

		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - Input.GetAxis("Mouse ScrollWheel"));
		
		//This is just to keep the wand's z angle component from changing (used to stabilize its rotation)
		/*Vector3 angle = transform.eulerAngles;
		angle.z = 0f;
		transform.eulerAngles = angle;*/
		
	}

	void PerformRayCast()
	{
		//Get the world position of the tip of the wand
		Vector3 wand_tip_pos = transform.position + (transform.forward * 2f);
		
		RaycastHit hit;
		
		bool success_hit = Physics.Raycast (wand_tip_pos,transform.forward,out hit,Mathf.Infinity);

		if(success_hit)
		{
			//Debug.Log (hit.collider.name);
		}

		//If we hit a widget via raycast
		if(success_hit && hit.collider.transform.parent.GetComponent<Widget>())
		{
			//The current widget we're hovering over, if not null, should have on leave called
			if(current_widget != null)
			{
				current_widget.SendMessage ("Leave");
			}
			
			//Make the current widget what we just hit
			current_widget = hit.collider.transform.parent.GetComponent<Widget>();
			
			//Send the on hover message to the widget.
			current_widget.SendMessage ("Hover");
			
			//If left mouse clicked: Send the on click message to the widget.
			if(Input.GetMouseButtonDown(0))
			{
				current_widget.SendMessage("Click");
			}
		}
		//If we didn't hit a widget via raycast
		else if(current_widget != null)
		{
			//The current widget's on leave is called and made null.
			current_widget.SendMessage ("Leave");
			current_widget = null;
		}
		
		//For debugging purposes
		Debug.DrawRay(wand_tip_pos,transform.forward*100f,Color.green);
		
	}

	
	public bool IsPointingLeft()
	{
		if(transform.rotation.eulerAngles.y > 180 && transform.rotation.eulerAngles.y <= 318)
		{
			return true;
		}
		return false;
	}
	
	public bool IsPointingRight()
	{
		if(transform.rotation.eulerAngles.y >= 42 && transform.rotation.eulerAngles.y < 180)
		{
			return true;
		}
		return false;
	}
	
	public bool IsPointingUp()
	{
		if(transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x <= 318)
		{
			return true;
		}
		return false;
	}
	
	public bool IsPointingDown()
	{
		if(transform.rotation.eulerAngles.x >= 8 && transform.rotation.eulerAngles.x < 180)
		{
			return true;
		}
		return false;
	}
	
	public bool IsHoldingClick()
	{
		return Input.GetMouseButton(0);
	}
	
	public bool IsMovingUp()
	{
		float vert = Input.GetAxis ("Mouse Y");
		
		if(vert > 0.75f)
		{
			return true;
		}
		return false;
	}
	
	public bool IsMovingDown()
	{
		float vert = Input.GetAxis ("Mouse Y");
		
		if(vert < -0.75f)
		{
			return true;
		}
		return false;
	}
	
}
