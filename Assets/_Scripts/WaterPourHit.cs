using UnityEngine;
using System.Collections;

//This class represents the particle system used for the water pouring.
public class WaterPourHit : MonoBehaviour {

	public GameObject water_spill;
	public AudioSource splash_sound;
	public AudioSource pour_sound;
	public GameObject water_splash;
	
	public GameObject origin;

	public static Color tea_color = new Color(103/255f,68/255f,36/255f);

	public bool IsOriginTea()
	{
		return origin.tag == "Teapot" || origin.tag == "Teacup";
	}

	// Use this for initialization
	void Start () {
		origin = transform.parent.parent.gameObject;
		if(IsOriginTea())
		{
			particleSystem.startColor = tea_color;
		}
	}

	float splash_time_last_collision = 0f;
	float pour_time_last_collision = 0f;

	// Update is called once per frame
	void Update () {

		splash_time_last_collision += Time.deltaTime;
		pour_time_last_collision += Time.deltaTime;

		if(splash_time_last_collision >= 0.1f)
		{
			splash_sound.Stop();
		}
		if(pour_time_last_collision >= 0.1f)
		{
			pour_sound.Stop();
		}
	}

	private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];

	void OnParticleCollision(GameObject other) {

		int safeLength = particleSystem.safeCollisionEventSize;
		if (collisionEvents.Length < safeLength)
			collisionEvents = new ParticleSystem.CollisionEvent[safeLength];
		
		int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
		int i = 0;
		while (i < numCollisionEvents) {

			Vector3 pos = collisionEvents[i].intersection;

			bool do_pour = false;

			if(other.tag == "Teacup" || other.tag == "Teapot" || other.name == "teapotlid")
			{
				//Increment teacup/teapot water here...
				if(other.tag == "Teacup")
				{
					do_pour = true;
					if(origin.GetComponent<PitcherScript>() != null)
					{
						other.GetComponent<TeacupScript>().waterCount++;
						if(other.GetComponent<TeacupScript>().waterCount > PlayerTeaCeremony.teacupCapacity)
							other.GetComponent<TeacupScript>().OverflowWater();
						//print("water count = " + other.GetComponent<TeacupScript>().waterCount.ToString());
					}
					else if(origin.GetComponent<TeapotScript>() != null)
					{
						other.GetComponent<TeacupScript>().teaCount++;
						if(other.GetComponent<TeacupScript>().teaCount > PlayerTeaCeremony.teacupCapacity)
							other.GetComponent<TeacupScript>().OverflowTea();
						//print("tea count = " + other.GetComponent<TeacupScript>().teaCount.ToString());
					}
				}
				else if(other.tag == "Teapot")
				{
					if(origin.GetComponent<PitcherScript>() != null)
					{
						if(other.GetComponent<TeapotScript>().topOn)
						{
							other.GetComponent<TeapotScript>().waterOutside++;
							//print("water outside = " + other.GetComponent<TeapotScript>().waterOutside.ToString());
						}
						else
						{
							do_pour = true;
							other.GetComponent<TeapotScript>().waterInside++;
							if(other.GetComponent<TeapotScript>().waterInside > PlayerTeaCeremony.teapotCapacity)
							{
								if(other.GetComponent<TeapotScript>().teaCount <= 0)
									other.GetComponent<TeapotScript>().OverflowWater();
								else
									other.GetComponent<TeapotScript>().OverflowTea();
							}
							//print("water inside = " + other.GetComponent<TeapotScript>().waterInside.ToString());;
						}
					}
				}
				else if(other.name == "teapotlid")
				{
					if(origin.GetComponent<PitcherScript>() != null)
					{
						if(other.transform.parent.GetComponent<TeapotScript>().topOn)
						{
							other.transform.parent.GetComponent<TeapotScript>().waterOutside++;
						}
					}
				}


			}

			if(do_pour)
			{
				//To get which script that the origin tea object is, use the "origin variable", i.e.:
				//if(origin.GetComponent<PitcherScript>() != null){ Debug.Log ("Origin is pitcher"); }
				
				pour_time_last_collision = 0f;
				
				if(!pour_sound.isPlaying)
				{
					pour_sound.Play();
				}
				else
				{
					if(pour_sound.time >= 3.052381f)
						pour_sound.time = 1.504988f;
				}
			}
			else
			{
				splash_time_last_collision = 0f;
				GameObject spill = Instantiate(water_spill,pos,Quaternion.identity) as GameObject;
				GameObject splash = Instantiate(water_splash,pos,Quaternion.identity) as GameObject;

				if(IsOriginTea())
				{
					spill.renderer.material.SetColor ("_TintColor", tea_color);
					splash.GetComponentInChildren<ParticleSystem>().startColor = tea_color;
				}

				if(!splash_sound.isPlaying)
				{
					splash_sound.Play();
				}
			}
			i++;
		}
	}
}
