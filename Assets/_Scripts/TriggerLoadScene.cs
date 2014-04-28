using UnityEngine;
using System.Collections;

public class TriggerLoadScene : MonoBehaviour {
	GameObject gob;
	
	void Start(){
		gob = GameObject.Find("First Person Controller");	
	}

	void OnTriggerEnter(Collider coll){
		networkView.RPC("RPCgo", RPCMode.All);
	}
	
	[RPC]
	void RPCgo(){
		StartCoroutine("go");
	}
	
	[RPC]
	void RPCReset(){
		StartCoroutine("Reset");
	}
	
	void Update(){
		if (Input.GetAxis("Fire3")	> .5f)
			networkView.RPC("RPCgo", RPCMode.All);

		if (Input.GetButtonDown("Reset")){
			networkView.RPC("RPCReset", RPCMode.All);
		}
	}
	
	IEnumerator go(){
		gob.BroadcastMessage("FadeOut");
		yield return new WaitForSeconds(2f);
		gob.transform.position = new Vector3(410f, 0.85f, 50f);
		Application.LoadLevel("Teahousev3_3ScreenStereoCenterTracked");
	}
	
	IEnumerator Reset(){
		gob.BroadcastMessage("FadeOut");
		yield return new WaitForSeconds(2f);	
		gob.transform.position = new Vector3(419.1f, 3.21f, -3.46f);
		gob.BroadcastMessage("FadeIn");

	}
}
