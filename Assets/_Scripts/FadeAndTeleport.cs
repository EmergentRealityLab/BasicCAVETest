using UnityEngine;
using System.Collections;

public class FadeAndTeleport : MonoBehaviour {
	
	AudioSource bkgd;
	AudioSource air;
	AudioSource tea;

	// Use this for initialization
	void Start () {
		bkgd = GameObject.Find("_AmbientSound").GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if (Input.GetKeyDown(KeyCode.O) || (Input.GetAxis("Fire3")	> .5f)){
			networkView.RPC("RPCTeleport", RPCMode.All);
		}
		*/
		if (Controls.Reset()){

			if(Settings.debug)
				StartCoroutine("Reset");
			else
				networkView.RPC("RPCReset", RPCMode.All);
		}
	}
	
	[RPC]
	void RPCReset(){
		StartCoroutine("Reset");
	}
	
	[RPC]
	void RPCTeleport(){
		StartCoroutine("Teleport");	
	}
	
	IEnumerator Teleport(){
		BroadcastMessage("FadeOut");
		StartCoroutine("FadeAir");
		yield return new WaitForSeconds(2.0f);
		transform.parent.position = new Vector3(0f, 0.8f, 7f);
		bkgd = GameObject.Find("_AmbientSound").GetComponent<AudioSource>();
		bkgd.Play();
		BroadcastMessage("FadeIn");
	}
	
	IEnumerator FadeAir(){
		air = GameObject.Find("_AirportSound").GetComponent<AudioSource>();
		while (air.volume > 0.05f){
			air.volume -= 0.25f * Time.deltaTime;
			yield return null;
		}
		air.Stop();
	}
	
	IEnumerator FadeTea(){
		tea = GameObject.Find("_AmbientSound").GetComponent<AudioSource>();
		while (tea.volume > 0.05f){
			tea.volume -= 0.25f * Time.deltaTime;
			yield return null;
		}
		tea.Stop();
	}
	
	IEnumerator Reset(){
		BroadcastMessage("FadeOut");
		//StartCoroutine("FadeAir");
		//StartCoroutine("FadeTea");
		yield return new WaitForSeconds(2.0f);
		transform.parent.position = new Vector3(419.1f, 3.21f, -3.46f);
		transform.parent.Rotate(0f, 180f, 0f);

		if(Player.player)
			Destroy(Player.player.gameObject);

		Application.LoadLevel("Transition");

		//bkgd = GameObject.Find("_AirportSound").GetComponent<AudioSource>();
		//bkgd.Play();
		BroadcastMessage("FadeIn");
		this.enabled = false;

	}
}
