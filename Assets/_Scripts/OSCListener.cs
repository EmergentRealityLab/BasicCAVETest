using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OSCListener : MonoBehaviour {
	
	PointerControl pointerRoot;
	ProjectionRoot projectionRoot;
	ProjectionRoot pr2;

	// Use this for initialization
	void Awake () {
		OSCHandler.Instance.Init(); //init OSC
		projectionRoot = GameObject.Find("MainPlayer").transform.Find("ProjectionRoot").GetComponent<ProjectionRoot>();
		//pr2 = GameObject.Find("ERL Controller 2").transform.Find("ProjectionRoot").GetComponent<ProjectionRoot>();
		pointerRoot = GameObject.Find("PointerRoot").GetComponent<PointerControl>();
		DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (projectionRoot.whichCameraToViewFrom == ProjectionRoot.viewType.center){
			OSCHandler.Instance.UpdateLogs();
			List<string> server_messages = OSCHandler.Instance.Servers["HeadTracker"].log;
			foreach (string msg in server_messages){
				//Debug.Log (msg);
				//parse message and update tracker position
				string[] words = msg.Split(' ');
				
				//convert Vicon coordinates to Unity coordinates
				projectionRoot.UpdateTrackerPosition(-float.Parse(words[5]),float.Parse(words[7]), -float.Parse(words[6]));

				//pr2.UpdateTrackerPosition(-float.Parse(words[17]),float.Parse(words[19]), -float.Parse(words[18]));

				if (pointerRoot)
					pointerRoot.UpdatePointer (float.Parse(words[11]), float.Parse(words[13]), float.Parse(words[12]), -float.Parse(words[15]), -float.Parse(words[14]), -float.Parse(words[16]));
			}
		}
	}
}
 