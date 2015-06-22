using UnityEngine;
using System.Collections;

public class WandScript2 : MonoBehaviour {

	public Transform pointer;
	public Vector3 positionOffset = new Vector3 (0f,0f,.5f);
	// Use this for initialization
	void Start () {
		pointer = GameObject.Find("PointerRoot").transform;

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 wandPos = pointer.localPosition;
		Vector3 wandRot = pointer.localEulerAngles;

		transform.localPosition = wandPos + positionOffset;

		float rx = wandRot.x;
		float ry = wandRot.y;
		float rz = wandRot.z;

		//rx=-rx;
		//ry=-ry;
		//rz=-rz;
		transform.localEulerAngles = new Vector3 (rx,ry,rz);
	}
}
