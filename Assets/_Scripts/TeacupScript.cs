using UnityEngine;
using System.Collections;

public class TeacupScript : MonoBehaviour {
	
	public int teaCount;
	public int waterCount;
	
	public GameObject water;
	public GameObject tea;
	
	public GameObject waterParticles;
	public GameObject teaParticles;
	public ParticleSystem waterPouring;
	public GameObject pourRootLeft;
	public GameObject pourRootRight;
	
	private float pourInterval = 0.1f;
	private float dropInterval = 1.0f;
	private float pourTimer = 0.0f;
	private float dropTimer = 0.0f;
	private bool pouring = false;
	private Vector3 initialPosition;
	private Quaternion initialRotation;

	public ParticleSystem pourLeft;
	public ParticleSystem pourRight;
	
	Color start_color;
	public ParticleSystem overflow;

	public void OverflowWater()
	{
		overflow.startColor = new Color();
		overflow.enableEmission = true;
	}

	public void OverflowTea()
	{
		overflow.startColor = WaterPourHit.tea_color;
		overflow.enableEmission = true;
	}

	public void SetRootLeft()
	{
		if(waterPouring != pourLeft)
		{
			waterPouring = pourLeft;
			pourLeft.enableEmission = false;
			pourRight.enableEmission = false;
		}
	}

	public void LateUpdate()
	{
		overflow.enableEmission = false;
	}

	public void SetRootRight()
	{
		if(waterPouring != pourRight)
		{
			waterPouring = pourRight;
			pourLeft.enableEmission = false;
			pourRight.enableEmission = false;
		}
	}

	// Use this for initialization
	void Start () 
	{
		teaCount = 0;
		waterCount = 0;
		GameObject.Find("MainPlayer").GetComponent<PlayerTeaCeremony>().teacups.Add(gameObject);
		pourLeft.Play();
		pourRight.Play();
		overflow.Play();
		pourLeft.enableEmission = false;
		pourRight.enableEmission = false;
		overflow.enableEmission = false;
		start_color = overflow.startColor;

		initialPosition = transform.position;
		initialRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Controls.ResetObjects())
		{
			transform.position = initialPosition;
			transform.rotation = initialRotation;
		}

		if(teaCount > 500)
		{
			teaCount = 500;
		}

		pourRootLeft.transform.eulerAngles = new Vector3(0,0,0);
		pourRootRight.transform.eulerAngles = new Vector3(0,0,0);

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
		if(collider.transform.name.Contains("teaTeapot(Clone)"))
		{
			teaCount++;
			//print(teaCount);
			GameObject.Destroy(collider.gameObject);
			//print("tea count = " + teaCount.ToString() + " " + Time.time.ToString());
		}
		else if(collider.transform.name.Contains("water(Clone)"))
		{
			waterCount++;
			//print(waterCount);
			GameObject.Destroy(collider.gameObject);
			//print("water count = " + waterCount.ToString());
		}
	}
	
	public void DropContents()
	{
		if(waterCount > 0)
		{
			waterCount -= 100;
			//GameObject.Instantiate(water, transform.position , Quaternion.identity);
		}
		else if(teaCount > 0)
		{
			teaCount -= 100;
			//GameObject.Instantiate(tea, transform.position, Quaternion.identity);
		}
	}
	
	private void DropParticles()
	{
		if(waterCount > 0)
		{
			waterPouring.enableEmission = true;
			//GameObject.Instantiate(waterParticles, transform.position , Quaternion.identity);
		}
		else if(teaCount > 0)
		{
			waterPouring.enableEmission = true;
			//GameObject.Instantiate(teaParticles, transform.position , Quaternion.identity);
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
