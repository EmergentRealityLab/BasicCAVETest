using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OSCEventListener : MonoBehaviour {
	
	PointerControl pointerRoot;
	ViewUpdate projectionRoot;

	private static OSCEventListener osc;
	public static OSCEventListener OSC
	{
		get
		{
			if(osc == null)
				osc = GameObject.FindObjectOfType<OSCEventListener>();
			return osc;
		}
	}

	// Use this for initialization
	void Start () 
	{
		if(Settings.debug)
		{
			gameObject.SetActive(false);
			return;
		}
		OSCHandler.Instance.Init(); //init OSC
		projectionRoot = GameObject.Find("ProjectionRoot").GetComponent<ViewUpdate>();
		pointerRoot = GameObject.Find("PointerRoot").GetComponent<PointerControl>();
		DontDestroyOnLoad(gameObject);
	}

	public void ResetReferences()
	{
		projectionRoot = GameObject.Find("ProjectionRoot").GetComponent<ViewUpdate>();
		pointerRoot = GameObject.Find("PointerRoot").GetComponent<PointerControl>();
	}

	// Update is called once per frame
	void Update () {

		if (projectionRoot.whichCameraToViewFrom == ViewUpdate.viewType.center){
			/*
			OSCHandler.Instance.UpdateLogs();

			List<UnityOSC.OSCPacket> packets=OSCHandler.Instance.Servers["HeadTracker"].packets;
			
			if (packets.Count>0)
			Debug.Log (packets[0].Address);
			
			*/
			OSCHandler.Instance.UpdateLogs();
			List<string> server_messages = OSCHandler.Instance.Servers["HeadTracker"].log;
			foreach (string msg in server_messages){
				//Debug.Log (msg);
				//parse message and update tracker position
				string[] words = msg.Split(' ');
				
				
				//convert Vicon coordinates to Unity coordinates
				projectionRoot.UpdateTrackerPosition(float.Parse(words[5]),float.Parse(words[7]), float.Parse(words[6]));

				//14, 15, 16 = H, P, R
				// rotate X = pitch
				// rotate Y = heading
				// rotate Z = roll
				if (pointerRoot)
					pointerRoot.UpdatePointer (float.Parse(words[11]), float.Parse(words[13]), float.Parse(words[12]), float.Parse(words[15]), float.Parse(words[14]), float.Parse(words[16]));
			}
			
			
			
		}
	}
}
 