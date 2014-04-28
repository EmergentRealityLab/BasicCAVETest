using UnityEngine;
using System.Collections;

public class HeadLookController : MonoBehaviour {

	public GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("tracker");
	}

	//Usage: <something>.rotation = RotateTowards()
	public Quaternion RotateLerpTowards(GameObject rotater,Vector3 Target,float RotationSpeed)
	{
		Quaternion _lookRotation;
		Vector3 _direction;
		
		//find the vector pointing from our position to the target
		_direction = (Target - rotater.transform.position).normalized;
		
		//create the rotation we need to be in to look at the target
		_lookRotation = Quaternion.LookRotation(_direction);
		
		//rotate us over time according to speed until we are in the required rotation
		return Quaternion.Lerp(rotater.transform.rotation,_lookRotation, RotationSpeed * Time.deltaTime);//Time.deltaTime * RotationSpeed);
	}

	// Update is called once per frame
	void Update () {

		transform.rotation = RotateLerpTowards(gameObject,player.transform.position,10f);
		
		float saveY = transform.localEulerAngles.y;

		if(transform.localEulerAngles.y > 90 && transform.localEulerAngles.y <= 180)
		{
			saveY = 90;
		}
		else if(transform.localEulerAngles.y > 180 && transform.localEulerAngles.y <= 270)
		{
			saveY = 270;
		}

		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,saveY,transform.localEulerAngles.z);
	}
}
