using UnityEngine;
using System.Collections;

public class TongClampCollider : MonoBehaviour {

	public TeaTongs parent;

	// Use this for initialization
	void Start () {
	
		parent = transform.parent.gameObject.GetComponent<TeaTongs>();
	
	}
	
	// Update is called once per frame
	void Update () {
	

	}

	void OnTriggerStay(Collider other)
	{
		if(parent.IsHeld() && other.tag == "Teacup")
		{
			Debug.Log ("In position to clamp");
		}
		if(parent.child == null && parent.IsHeld() && other.tag == "Teacup" && Controls.MainButtonDown())
		{
			Debug.Log ("It was clamped");
			other.transform.localRotation = Quaternion.identity;
			parent.SetChild(other.gameObject);
			parent.drop_lock = true;
		}
	}
}
