using UnityEngine;
using System.Collections;

public class MrsLingControl : MonoBehaviour {

	Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void playAnimation(string clipName){

		Debug.Log("playAnimation was called from the Dialog Menu");
		anim.Play (clipName);
		if (Network.isServer)
			networkView.RPC ("netPlayAnimation", RPCMode.Others, clipName);
	}

	[RPC]
	void netPlayAnimation(string clipName){
		Debug.Log("starting to play: " + clipName);
		anim.Play(clipName);
		Debug.Log ("called play on: " + clipName);
	}
}
