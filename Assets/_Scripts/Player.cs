using UnityEngine;
using System.Collections;

//This class stores any necessary state involved with a player, such as "Inventory", etc.
//(Empty for now)
public class Player : MonoBehaviour 
{
	private static Player p;

	public Player clone;
	public Player clean_player_prefab;
	Animator anim;

	//Singleton for player
	public static Player player
	{
		get
		{
			if(p == null)
				p = GameObject.FindObjectOfType<Player>();
			return p;
		}
	}

	public TeaDialogTree tea_tree;

	public void Recreate()
	{
		clone.gameObject.SetActive(true);
		p = clone;
		p.name = "MainPlayer";
		Destroy(gameObject);
	}

	/*public void playFinalAnim(){
		networkView.RPC("finalAnim", RPCMode.Others);
	}

	[RPC]
	public void finalAnim(){
		anim.Play("L");
	}*/

	public void Reset()
	{
		/*
		tea_tree.gameObject.SetActive(true);
		tea_tree.Reset();
		GetComponent<PickupObject>().Reset();
		GetComponent<PlayerTeaCeremony>().Reset();
		StopTeaCeremony();
		*/
	}

	// Use this for initialization
	void Start() 
	{
		subtitle_menu = GetComponentInChildren<SubtitleMenu>();

		if(OSCEventListener.OSC)
			OSCEventListener.OSC.ResetReferences();

		//anim = GameObject.Find("MRS_LING").GetComponent<Animator>();

		//clone.gameObject.SetActive(false);
	}
	
	bool fast_forward;
	
	public SubtitleMenu subtitle_menu;
	
	public bool in_tea_ceremony = false;

	// Update is called once per frame
	void Update() 
	{
		if(Settings.debug)
		{
			Settings.HideAndLockCursor();
		}
		
		if(Settings.start_tea_immediately)
		{
			StartTeaCeremony();
		}

		/*
		if(Controls.FastForwardButton())
		{
			Time.timeScale = 10;
		}
		else
		{
			Time.timeScale = 1;
		}
		*/

		//Stop them from moving in tea ceremony
		/*
		if(in_tea_ceremony)
		{
			//GetComponentInChildren<CharacterController>().enabled = false;
			tea_tree.gameObject.SetActive(true);
			//transform.position = new Vector3(0.2f,0.9f,2.0f);				  // there should be a more general purpose way to do this
		}
		*/
	}
	
	// enable or disable movement 
	// 
	public void StartTeaCeremony ()
	{
		CharacterMotor motor = GetComponent<CharacterMotor>();
		//motor.canControl=false;
		
		tea_tree.gameObject.SetActive (true);
		//GetComponent<PlayerTeaCeremony>().BeginCheck();

		// tea_tree is the whole DialogTree
		// Find Mrs. Ling, get her Animator component, set the 
	}

	public void StopTeaCeremony()
	{
		CharacterMotor motor = GetComponent<CharacterMotor>();
		motor.canControl=true;
		
		tea_tree.gameObject.SetActive (false);
	}
	
	void OnGUI()
	{
		//string coords = transform.position.ToString();
		
		//GUI.Label (new Rect(Screen.width/2f,Screen.height/2f,500,500),coords);
		
		//Debug.Log (coords);


	}
	
}
