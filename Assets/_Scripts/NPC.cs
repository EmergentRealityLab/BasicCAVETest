using UnityEngine;
using System.Collections;

//Attach this script to the root of any NPC for them to get clicked and say a voice line.
public class NPC : MonoBehaviour {
	
	public AudioClip clip;
	public AudioSource audio_source;
	public string english_subtitles;
	
	// Use this for initialization
	void Start () {
		audio_source = GetComponentInChildren<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static NPC GetNPC(GameObject obj)
	{
		GameObject root = obj.transform.root.gameObject;
		return root.GetComponentInChildren<NPC>();
	}
}
