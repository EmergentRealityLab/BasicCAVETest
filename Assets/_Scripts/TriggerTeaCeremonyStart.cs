using UnityEngine;
using System.Collections;

public class TriggerTeaCeremonyStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter (Collider other)
	{
		GameObject obj=other.gameObject;
		
		if (obj.tag == "Player")
		{	Debug.Log ("trigger");
			obj.SendMessage ("StartTeaCeremony",SendMessageOptions.DontRequireReceiver);	
		}
	}
	
	
}
