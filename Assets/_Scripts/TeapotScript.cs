using UnityEngine;
using System.Collections;

public class TeapotScript : MonoBehaviour {
	
	public int waterInside = 0;
	public int waterOutside = 0;
	public int teaCount = 0;
	public int teaLeafCount = 0;
	public bool topOn = true;
	public ParticleSystem waterPouring;
	public GameObject pourRoot;
	
	public GameObject water;
	public GameObject tea;
	public GameObject teaParticles;

	private float pourInterval = 0.1f;
	private float dropInterval = 1.0f;
	private float pourTimer = 0.0f;
	private float dropTimer = 0.0f;
	private bool pouring = false;
	private Vector3 initialPosition;
	private Quaternion initialRotation;

	Color start_color;
	public ParticleSystem overflow;

	public void OverflowWater()
	{
		overflow.startColor = start_color;
		overflow.enableEmission = true;
	}
	
	public void OverflowTea()
	{
		overflow.startColor = WaterPourHit.tea_color;
		overflow.enableEmission = true;
	}

	public void LateUpdate()
	{
		overflow.enableEmission = false;
	}

	// Use this for initialization
	void Start () 
	{
		waterInside = 0;
		waterOutside = 0;
		topOn = false;
		GameObject.Find("MainPlayer").GetComponent<PlayerTeaCeremony>().teapot = gameObject;
		waterPouring.renderer.material.SetColor("_TintColor", new Color(0.65f,0.16f,0.16f,0.5f));
		pourRoot.transform.eulerAngles = new Vector3(0,0,0);
		initialPosition = transform.position;
		initialRotation = transform.rotation;
		waterPouring.Play();
		overflow.Play ();
		overflow.enableEmission = false;
		start_color = overflow.startColor;
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
	
	void OnCollisionEnter(Collision collider)
	{
		if(collider.transform.name.Contains("water(Clone)"))
		{
			if(topOn)
			{
				waterOutside++;
				//print("teapot water outside count = " + waterOutside.ToString());
			}
			else
			{
				waterInside++;
				//print("teapot water inside count = " + waterInside.ToString());
			}
			GameObject.Destroy(collider.gameObject);
			
		}
	}
	
	private void DropContents()
	{
		if(teaCount > 0)
		{
			teaCount--;
			//GameObject.Instantiate(tea, transform.position, Quaternion.identity);
		}
	}
	
	private void DropParticles()
	{
		if(teaCount > 0)
		{
			//GameObject.Instantiate(teaParticles, transform.position, Quaternion.identity);
			waterPouring.enableEmission = true;
			waterPouring.startColor = new Color(212.0f / 255.0f, 155.0f / 255.0f, 117.0f / 255.0f);
		}
		else
		{
			waterPouring.enableEmission = false;	
		}
	}
	
	public void Pour(bool shouldPour)
	{
		pouring = shouldPour;
	}
}
