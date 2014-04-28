using UnityEngine;
using System.Collections;

public class NoiseFilter : MonoBehaviour {

	public Vector3 prev_pos;
	public float epsilon;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		/*
		if(!Settings.use_experimental_filter)
		{
		    enabled = false;
			return;
		}

		if(Controls.MainButton())
		{
			epsilon += (Time.deltaTime * 0.01f);
		}
		else if(Controls.SecondButton())
		{
			epsilon -= (Time.deltaTime * 0.01f);
			if(epsilon <= 0)
				epsilon = 0;
		}

		float dist = Vector3.Distance(prev_pos,transform.position);

		//Debug.Log (dist);

		if(dist < epsilon)
		{
			transform.position = prev_pos;
		}

		prev_pos = transform.position;
		*/

	}
	
	/*
	void OnGUI()
	{
		if(Settings.use_experimental_filter)
		{
			GUI.Label (new Rect(Screen.width/2,Screen.height/2,200,100), epsilon.ToString());
		}
	}
	*/

}
