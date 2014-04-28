using UnityEngine;
using System.Collections;

public class PlayerEventCollider : MonoBehaviour {
	
	public Player player;
	
	void Awake()
	{
		player = transform.root.GetComponentInChildren<Player>();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/*
	//Enable the tea menu
	void OnTriggerEnter(Collider other)
	{
		if(other.name == "TriggerTeaCeremony")
			player.StartTeaCeremony();
	}
	*/
	
	void StartTeaCeremony()
	{
			player.StartTeaCeremony();
	}
	
}
