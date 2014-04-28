using UnityEngine;
using System.Collections;

public class DisableRendererOnAwake : MonoBehaviour {
	
	void Awake()
	{
		if(renderer)
			renderer.enabled = false;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
