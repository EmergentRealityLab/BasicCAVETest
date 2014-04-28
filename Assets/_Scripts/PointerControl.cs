using UnityEngine;
using System.Collections;

//This script controls the pointer. It also performs the raycast used to interact with menus, etc.
public class PointerControl : MonoBehaviour 
{
	public Transform pointer;
	GameObject pointer_stick;
	
	LaserControl laser;
	
	public GameObject current_hit;	//The gameobject that the raycast is currently "hitting".
	public Vector3 current_hit_point;
	
	public Player player;

	public PickupObject pickup_object_script;
	LineRenderer line;
	public float texScaleModifier = 10;
	public float texScrollSpeed = 1;
	Transform pbox;

	//Get the current widget we are hitting.
	public Widget current_widget()
	{
		if(current_hit)
			return Widget.GetRootWidget(current_hit);
		else
			return null;
	}
	
	// Use this for initialization
	void Start () {
		
		player = transform.root.GetComponentInChildren<Player>();
		pickup_object_script = player.GetComponent<PickupObject>();

		pointer = GameObject.Find("PointerRoot").transform;
		pointer_stick = GameObject.Find("Pointer_stick").gameObject;
		line = pointer_stick.GetComponent<LineRenderer>();

		if(!Settings.debug)
		{
			GameObject.Find("Pointer").renderer.enabled = false;
			pointer_stick.renderer.enabled = false;
		}
		
		laser = GameObject.Find ("laserDot").gameObject.GetComponent<LaserControl>(); 
		pbox = GameObject.Find("Pointer").transform;
	}
	
	public void Update()
	{
		//Debug.Log (current_hit);
	}
	
	// Update is called once per frame
	public void UpdatePointer (float x, float y, float z, float rotx, float roty, float rotz) 
	{
		pointer.localEulerAngles = new Vector3(rotx, roty, rotz);
		pointer.localPosition = new Vector3(x,y-0.8f,z);
		//Debug.Log (pointer.localPosition);
	}

	const float max_laser_dist = 5f;

	void LateUpdate()
	{
		//Perform the main raycast, then move the laser dot to the hit.
		RaycastHit hit;

		//The raycast for detecting menu selection and tea objects
		PerformRayCast(out hit);

		RaycastHit hit2;

		//The raycast for positioning the laser pointer
		if(Physics.Raycast(pointer.transform.position,pointer.transform.forward,out hit2,max_laser_dist))
		{
			//Debug.Log (hit.collider.name);
      	    laser.transform.position = hit2.point;	
		}
		else
		{
			laser.transform.position = pointer.transform.position + (pointer.transform.forward * max_laser_dist);
		}

		/**/
		//Get the distance from the laser to the pointer
		Vector3 diff = laser.transform.position - pointer.transform.position;

		if(diff.magnitude >= 10f) {
			diff = diff.normalized * 10;
		}

		Vector3 norm = diff.normalized * 0.1f;

		if((diff - norm).magnitude >= 0) {
			laser.transform.position = pointer.transform.position + diff - norm;
		} else {
			laser.transform.position = pointer.transform.position;
		}

		laser.Align();

		//adjust dashed line
		//if (pbox)
			line.SetPosition(0, pbox.position);
		//else
		//	line.SetPosition(0, transform.root.position);
		line.SetPosition(1, laser.transform.position);
		//line.renderer.material.mainTextureScale = new Vector2((float)diff.magnitude/texScaleModifier, line.renderer.material.mainTextureScale.y);
		line.renderer.material.mainTextureOffset -= new Vector2(Time.deltaTime * texScrollSpeed, line.renderer.material.mainTextureOffset.y);
		line.material.SetTextureScale("_MainTex", new Vector2((float)diff.magnitude/texScaleModifier, line.material.GetTextureScale("_MainTex").y));
		line.material.SetTextureOffset("_MainTex", new Vector2(line.material.GetTextureOffset("_MainTex").x - Time.deltaTime * texScrollSpeed, 0f));
	}
	
	public LayerMask SelectionMask()
	{
		LayerMask m = include_bits(new int[]{2,8,9,19,11,31});
		return m;
	}
	
	//Creates a layermask that does not include the layers provided in the arguments
	public static LayerMask remove_bits(int[] bits)
	{
		LayerMask mask = 0;
		mask = ~mask;
		
		for(int i=0;i<bits.Length;i++)
		{
			LayerMask bitmask = 1 << bits[i];
			mask = bitmask ^ mask;
		}
		
		return mask;
	}
	
	//Creates a layermask that only includes layers provided in teh argument
	public static LayerMask include_bits(int[] bits)
	{
		LayerMask inverse = remove_bits (bits);
		return ~inverse;
	}

	public static bool IsTeaObject(GameObject go)
	{
		if(go.tag.Contains("Teacup") || go.tag.Contains("Pitcher") || 
			go.tag.Contains("Teapot") || go.tag.Contains("Tongs")   ||
				go.tag.Contains("Scoop")  || go.tag.Contains("OtherPickup"))
		{
			return true;
		}
		return false;
	}

	bool PerformRayCast(out RaycastHit hit)
	{
		bool success_hit = Physics.Raycast (pointer.transform.position,pointer.transform.forward,out hit,Mathf.Infinity);
		
		if(success_hit)
		{
			NPC npc = NPC.GetNPC(hit.collider.gameObject);
		
			if(npc != null && Controls.MainButtonDown() && player.subtitle_menu.IsClipDone())
			{
				Debug.Log ("We've hit a NPC");
				player.subtitle_menu.SetDialogClip(npc.clip,npc.audio_source,"",null);
				player.subtitle_menu.TurnOn();
			}
		}
		
		//If we hit a widget via raycast
		if(success_hit && Widget.GetRootWidget(hit.collider.gameObject))
		{
			//The current widget we're hovering over, if not null, should have on leave called
			if(current_widget() != null)
			{
				current_widget().Leave();
			}
			
			//Make the current widget what we just hit
			current_hit = hit.collider.gameObject;
			//current_hit_point = hit.point;
			
			//Send the on hover message to the widget.
			current_widget().Hover();
			
			//If main button clicked: Send the on click message to the widget.
			if(Controls.MainButtonDown())
			{
				current_widget().Click();
			}
		}
		//If we hit something that isn't a widget:
		else if(success_hit)
		{
			if(current_hit && IsTeaObject(current_hit))
			{
				foreach(Renderer r in current_hit.GetComponentsInChildren<Renderer>())
					r.material.SetColor("_RimColor", Color.black);
			}

			//The current widget's on leave is called
			if(current_widget() != null)
				current_widget().Leave ();
			
			//Set the current hit to whatever we hit.
			current_hit = hit.collider.gameObject;
			current_hit_point = hit.point;

			if(IsTeaObject(current_hit) && pickup_object_script.enabled)
			{
				foreach(Renderer r in hit.collider.GetComponentsInChildren<Renderer>())
					r.material.SetColor("_RimColor", Color.yellow);
			}
		}
		
		//If we didn't hit anything:
		else if(current_widget() != null)
		{
			if(current_hit && IsTeaObject(current_hit))
			{
				foreach(Renderer r in current_hit.GetComponentsInChildren<Renderer>())
					r.material.SetColor("_RimColor", Color.black);
			}

			//The current widget's on leave is called.
			current_widget().Leave ();
			
			//The current hit is set to null.
			current_hit = null;
			current_hit_point = Vector3.zero;
		}
		
		//For debugging purposes
		//Debug.DrawRay(wand_tip_pos,transform.forward*100f,Color.green);
		
		return success_hit;
		
	}
}
