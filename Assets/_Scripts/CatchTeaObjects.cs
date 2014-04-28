using UnityEngine;
using System.Collections;

public class CatchTeaObjects : MonoBehaviour {

	private Vector3 respawnPosition;
	private PickupObject pickupObject;
	void Awake()
	{
		respawnPosition = GameObject.Find ("TeaObjectRespawn").transform.position;
		pickupObject = GameObject.Find("MainPlayer").GetComponent<PickupObject>();
	}

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "OtherPickup" || other.tag == "Teacup" || other.tag == "Teapot" || other.tag == "Pitcher" || other.tag == "Scoop" || other.tag == "Tongs")
		{
			if(other.gameObject == pickupObject.pickedUpObject)
			{
				pickupObject.Drop();
			}

			other.transform.position = respawnPosition;
		}
	}
}
