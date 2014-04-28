using UnityEngine;
using System.Collections;


public class ViewUpdate : MonoBehaviour {
	
	//public Vector3 trackerPosition = new Vector3(0f,1.5f,0f);
	public Transform trackerPosition;
	public bool stereo = false;
	public float interpupillaryDistance = 0.1f;
	MouseLook ml; 
	bool menuToggle = false;
	private string ip = "129.161.12.207";
	public int port = 12345;
	
	public enum viewType{center, left, right};
	public viewType whichCameraToViewFrom = viewType.center;
	
	void Start(){
		ml = transform.parent.GetComponent<MouseLook>() as MouseLook;
	}
	
	void Update(){
		if (ml){
			if (Input.GetKeyDown(KeyCode.M)){
				if (menuToggle){
					menuToggle = false;
					//ml.enabled = true;
				}
				else {
					menuToggle = true;
					ml.enabled = false;
				}
			}
		}
	}
	
	public void UpdateTrackerPosition(float x, float y, float z){
		trackerPosition.localPosition = new Vector3(x, y, z);
	}
	
	public void OnGUI(){
		if (menuToggle){
			ip = GUI.TextField(new Rect(20, 60, 150, 20), ip, 15);
			if (GUI.Button(new Rect(105,10,150,20), "Start server as center")){
				//Debug.Log("Not actually starting server yet");
				Debug.Log ("SET CENTER TO TRUE");
				Settings.position = Settings.Position.Center;
				transform.Find("Main Camera-centerL").gameObject.SetActive(true);
				transform.Find("Main Camera-centerR").gameObject.SetActive(true);
				transform.Find("Main Camera-leftL").gameObject.SetActive(false);
				transform.Find("Main Camera-leftR").gameObject.SetActive(false);
				transform.Find("Main Camera-rightL").gameObject.SetActive(false);
				transform.Find("Main Camera-rightR").gameObject.SetActive(false);
				whichCameraToViewFrom = viewType.center;
				Settings.HideAndLockCursor();
				Network.InitializeServer(4, 12345, false);
			}
			if (GUI.Button(new Rect(180, 50, 150, 20), "Listen as left")){
				//Debug.Log("Not left yet");
				Settings.position = Settings.Position.Left;
				transform.Find("Main Camera-centerL").gameObject.SetActive(false);
				transform.Find("Main Camera-centerR").gameObject.SetActive(false);
				transform.Find("Main Camera-leftL").gameObject.SetActive(true);
				transform.Find("Main Camera-leftR").gameObject.SetActive(true);
				transform.Find("Main Camera-rightL").gameObject.SetActive(false);
				transform.Find("Main Camera-rightR").gameObject.SetActive(false);
				whichCameraToViewFrom = viewType.left;
				Settings.HideAndLockCursor();
				Network.Connect(ip, port);
			}
			if (GUI.Button(new Rect(180,75,150,20), "Listen as right")){
				//Debug.Log("Not right yet");
				Settings.position = Settings.Position.Right;
				transform.Find("Main Camera-centerL").gameObject.SetActive(false);
				transform.Find("Main Camera-centerR").gameObject.SetActive(false);
				transform.Find("Main Camera-leftL").gameObject.SetActive(false);
				transform.Find("Main Camera-leftR").gameObject.SetActive(false);
				transform.Find("Main Camera-rightL").gameObject.SetActive(true);
				transform.Find("Main Camera-rightR").gameObject.SetActive(true);
				whichCameraToViewFrom = viewType.right;
				Settings.HideAndLockCursor();
				Network.Connect(ip, port);
			}
		}
	}
}
