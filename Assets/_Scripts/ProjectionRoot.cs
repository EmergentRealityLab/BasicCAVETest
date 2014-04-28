using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ProjectionRoot : MonoBehaviour {
	
	//public Vector3 trackerPosition = new Vector3(0f,1.5f,0f);
	public Transform trackerPosition;
	public enum numberOfScreensType{one, three};
	public numberOfScreensType numberOfScreens = numberOfScreensType.one;
	public bool stereo = false;
	public float interpupillaryDistance = 0.063f;
	public bool player2 = false;
	public string ip = "129.161.12.174";
	public int port = 12345;
	
	public enum viewType{center, left, right};
	[HideInInspector]
	public viewType whichCameraToViewFrom = viewType.center;
	bool menuToggle = true;
	
	void Awake(){
		TurnOnCenter();
	}
	
	void Update(){
		if (Input.GetKeyDown(KeyCode.M) && player2){
			if (menuToggle)
				menuToggle = false;
			else 
				menuToggle = true;
		}
	}
	
	public void UpdateTrackerPosition(float x, float y, float z){
		trackerPosition.localPosition = new Vector3(x, y, z);

	}
	
	public void OnGUI(){
		if (menuToggle){
			if (GUI.Button(new Rect(105,10,150,20), "Start server as center")){
				//Debug.Log("Not actually starting server yet");
				Settings.position = Settings.Position.Center;
				Debug.Log ("SET CENTER TO TRUE");
				TurnOnCenter();
				Network.InitializeServer(4, 12345, false);
			}
			ip = GUI.TextField(new Rect(20, 60, 150, 20), ip, 15);
			if (GUI.Button(new Rect(180,50,150,20), "Listen as left")){
				//Debug.Log("Not left yet");
				Settings.position = Settings.Position.Left;
				TurnOnLeft();
				Network.Connect(ip, port);
			}
			if (GUI.Button(new Rect(180,75,150,20), "Listen as right")){
				//Debug.Log("Not right yet");
				Settings.position = Settings.Position.Right;
				TurnOnRight();
				Network.Connect(ip, port);
			}
			interpupillaryDistance = GUI.HorizontalSlider(new Rect(120, 110, 150, 25), interpupillaryDistance, 0.05f, 0.08f);
			GUI.Label(new Rect(20, 110, 100, 25), "IPD: " + interpupillaryDistance);
		}
	}
	
	public void SplitScreen(){
		stereo = false;
		if (player2){
			switch(whichCameraToViewFrom){
			case viewType.center:
				transform.Find("Cameras/Main Camera-centerL").gameObject.SetActive(false);
				break;
			case viewType.left:
				transform.Find("Cameras/Main Camera-leftL").gameObject.SetActive(false);
				break;
			case viewType.right:
				transform.Find("Cameras/Main Camera-rightL").gameObject.SetActive(false);
				break;
			}
		}
		else{
			switch(whichCameraToViewFrom){
			case viewType.center:
				transform.Find("Cameras/Main Camera-centerR").gameObject.SetActive(false);
				break;
			case viewType.left:
				transform.Find("Cameras/Main Camera-leftR").gameObject.SetActive(false);
				break;
			case viewType.right:
				transform.Find("Cameras/Main Camera-rightR").gameObject.SetActive(false);
				break;
			}
		}	
	}
	
	public void TurnOnCenter(){
		transform.Find("Cameras/Main Camera-centerL").gameObject.SetActive(true);
		transform.Find("Cameras/Main Camera-centerR").gameObject.SetActive(true);
		transform.Find("Cameras/Main Camera-leftL").gameObject.SetActive(false);
		transform.Find("Cameras/Main Camera-leftR").gameObject.SetActive(false);
		transform.Find("Cameras/Main Camera-rightL").gameObject.SetActive(false);
		transform.Find("Cameras/Main Camera-rightR").gameObject.SetActive(false);
		whichCameraToViewFrom = viewType.center;
	}
	
	public void TurnOnLeft(){
		transform.Find("Cameras/Main Camera-centerL").gameObject.SetActive(false);
		transform.Find("Cameras/Main Camera-centerR").gameObject.SetActive(false);
		transform.Find("Cameras/Main Camera-leftL").gameObject.SetActive(true);
		transform.Find("Cameras/Main Camera-leftR").gameObject.SetActive(true);
		transform.Find("Cameras/Main Camera-rightL").gameObject.SetActive(false);
		transform.Find("Cameras/Main Camera-rightR").gameObject.SetActive(false);
		whichCameraToViewFrom = viewType.left;
	}
	
	public void TurnOnRight(){
		transform.Find("Cameras/Main Camera-centerL").gameObject.SetActive(false);
		transform.Find("Cameras/Main Camera-centerR").gameObject.SetActive(false);
		transform.Find("Cameras/Main Camera-leftL").gameObject.SetActive(false);
		transform.Find("Cameras/Main Camera-leftR").gameObject.SetActive(false);
		transform.Find("Cameras/Main Camera-rightL").gameObject.SetActive(true);
		transform.Find("Cameras/Main Camera-rightR").gameObject.SetActive(true);
		whichCameraToViewFrom = viewType.right;
	}
}
