using UnityEngine;
using System.Collections;

public class StartS2 : MonoBehaviour {
	GameObject gob;
	
	// Use this for initialization
	void Start () {

		Debug.Log ("START");

		if(Settings.position == Settings.Position.Center)
		{
			Debug.Log ("CENTER");
			GameObject.Find("Main Camera-centerL").gameObject.SetActive(true);
			GameObject.Find("Main Camera-centerR").gameObject.SetActive(true);
			GameObject.Find("Main Camera-leftL").gameObject.SetActive(false);
			GameObject.Find("Main Camera-leftR").gameObject.SetActive(false);
			GameObject.Find("Main Camera-rightL").gameObject.SetActive(false);
			GameObject.Find("Main Camera-rightR").gameObject.SetActive(false);
		}
		if(Settings.position == Settings.Position.Left)
		{
			Debug.Log ("LEFT");
			GameObject.Find("Main Camera-centerL").gameObject.SetActive(false);
			GameObject.Find("Main Camera-centerR").gameObject.SetActive(false);
			GameObject.Find("Main Camera-leftL").gameObject.SetActive(true);
			GameObject.Find("Main Camera-leftR").gameObject.SetActive(true);
			GameObject.Find("Main Camera-rightL").gameObject.SetActive(false);
			GameObject.Find("Main Camera-rightR").gameObject.SetActive(false);
		}
		if(Settings.position == Settings.Position.Right)
		{
			Debug.Log ("RIGHT");
			GameObject.Find("Main Camera-centerL").gameObject.SetActive(false);
			GameObject.Find("Main Camera-centerR").gameObject.SetActive(false);
			GameObject.Find("Main Camera-leftL").gameObject.SetActive(false);
			GameObject.Find("Main Camera-leftR").gameObject.SetActive(false);
			GameObject.Find("Main Camera-rightL").gameObject.SetActive(true);
			GameObject.Find("Main Camera-rightR").gameObject.SetActive(true);
		}

		//OSCHandler.Instance.Init(); //(re?)init OSC
		gob = GameObject.Find("MainPlayer");
		gob.transform.Rotate(0f,180f,0f);
		gob.transform.position = transform.position;
		gob.BroadcastMessage("FadeIn");
		gob.GetComponentInChildren<FadeAndTeleport>().enabled = true;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
