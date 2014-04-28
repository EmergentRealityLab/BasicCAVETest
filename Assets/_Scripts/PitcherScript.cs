using UnityEngine;
using System.Collections;

public class PitcherScript : MonoBehaviour {
	
	public GameObject water;
	public GameObject waterParticles;
	public ParticleSystem waterPouring;
	public GameObject pourRoot;
	
	private float pourInterval = 0.1f;
	private float dropInterval = 1.0f;
	private float pourTimer = 0.0f;
	private float dropTimer = 0.0f;
	private bool pouring = false;
	private Vector3 initialPosition;
	private Quaternion initialRotation;
	
	// Use this for initialization
	void Start ()
	{
		GameObject.Find("MainPlayer").GetComponent<PlayerTeaCeremony>().pitcher = gameObject;
		pourRoot.transform.eulerAngles = new Vector3(0,0,0);
		initialPosition = transform.position;
		initialRotation = transform.rotation;
		waterPouring.Play();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Controls.ResetObjects())
		{
			transform.position = initialPosition;
			transform.rotation = initialRotation;
		}

		pourRoot.transform.eulerAngles = new Vector3(0,0,0);
		if(pouring)
		{
			pourTimer -= Time.deltaTime;
			dropTimer -= Time.deltaTime;
			if(pourTimer <= 0.0f)
			{
				pourTimer = pourInterval;
				DropParticles();
			}
			if(dropTimer <= 0.0f)
			{
				dropTimer = dropInterval;
				DropContents();
			}
		}
		else
		{
			waterPouring.enableEmission = false;
		}
	}
	
	private void DropContents()
	{
		//GameObject.Instantiate(water, transform.position, Quaternion.identity);
	}
	
	private void DropParticles()
	{
		GameObject.Instantiate(waterParticles, transform.position, Quaternion.identity);
		waterPouring.enableEmission = true;	
	}
	
	public void Pour(bool shouldPour)
	{
		pouring = shouldPour;
	}
}