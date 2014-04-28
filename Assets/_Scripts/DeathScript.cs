using UnityEngine;
using System.Collections;

public class DeathScript : MonoBehaviour {
	
	public float deathTimer = 1.0f;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		deathTimer -= Time.deltaTime;
		if(deathTimer < 0)
		{
			Destroy(gameObject);
		}
	}
}
