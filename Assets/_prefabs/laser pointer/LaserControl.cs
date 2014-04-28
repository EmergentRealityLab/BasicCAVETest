using UnityEngine;
using System.Collections;
 
public class LaserControl : MonoBehaviour
{
    Vector3 betweenCameraPos;
	GameObject debug_camera;
	Transform pointer;

	private static LaserControl laser;

	public static LaserControl Laser
	{
		get
		{
			if(laser == null)
				laser = GameObject.FindObjectOfType<LaserControl>();
			return laser;
		}
	}
	
	void Start(){
		betweenCameraPos = Vector3.Lerp(GameObject.Find("Main Camera-centerL").transform.position, GameObject.Find("Main Camera-centerR").transform.position, 0.5f);
		pointer = GameObject.Find("Pointer").transform;
		debug_camera = GameObject.Find("Main Camera-Preview");

		if(Settings.debug) {
			GameObject laserQuad = GameObject.Find ("laserQuad") as GameObject;
			laserQuad.transform.localEulerAngles = new Vector3(0,180,0);
		}
	}
 
    void LateUpdate()
    {
		/*
		//Moved to PointerControl
		if (pointer)
		{
			
			RaycastHit hit;
      	    if(Physics.Raycast(pointer.position, pointer.forward, out hit))
			{
      	       transform.position = hit.point;
			}
			
			transform.LookAt(betweenCameraPos);
			transform.Rotate(90,0,0);
			
		}
		*/
    }
	
	public void Align()
	{
		if(!Settings.debug) {
			transform.LookAt(betweenCameraPos);
		}
		else {
			transform.LookAt(debug_camera.transform.position);
		}
	}
}
