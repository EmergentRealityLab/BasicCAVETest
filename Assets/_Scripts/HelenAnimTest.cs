using UnityEngine;
using System.Collections;

public class HelenAnimTest : MonoBehaviour {

	public Animator anim;
	public AudioSource audio;
	public string[] animationClips;
	public AudioClip[] audioClips;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.A))
		{
			anim.Play(animationClips[0]);
			audio.PlayOneShot (audioClips[0]);
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			anim.Play(animationClips[1]);
			audio.PlayOneShot (audioClips[1]);
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			anim.Play(animationClips[2]);
			audio.PlayOneShot (audioClips[2]);
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			anim.Play(animationClips[3]);
			audio.PlayOneShot (audioClips[3]);
		}

	}
}
