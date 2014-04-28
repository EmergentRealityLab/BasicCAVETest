using UnityEngine;
using System.Collections;

public class ScreenFade : MonoBehaviour {
	
	public float FadeInTime = 2.0f;
	public float FadeOutTime = 2.0f;
	public Texture FadeInTexture;
	public float AlphaFadeValue = 0.0f;
	bool fadingIn = false;
	bool fadingOut = false;
	
	/*void Update(){
		if (Input.GetKeyDown(KeyCode.I))
			FadeIn();
		if (Input.GetKeyDown(KeyCode.O))
			FadeOut ();
	}*/
	

	public void FadeIn(){
		fadingOut = false;
		fadingIn = true;
	}
	
	public void FadeOut(){
		fadingOut = true;
		fadingIn = false;
	}
	
	void OnGUI(){	
		if(AlphaFadeValue > 0.0f && fadingIn){
  			AlphaFadeValue -= Mathf.Clamp01(Time.deltaTime / FadeInTime);
			if(AlphaFadeValue < 0.0f){
				AlphaFadeValue = 0.0f;	
				fadingIn = false;
			}
		}
		else if (AlphaFadeValue < 1.0f && fadingOut){
			AlphaFadeValue += Mathf.Clamp01(Time.deltaTime / FadeOutTime);
			if(AlphaFadeValue > 1.0f){
				AlphaFadeValue = 1.0f;
				fadingOut = false;
			}
		}
		GUI.color = new Color(0, 0, 0, AlphaFadeValue);
  		GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height ), FadeInTexture ); 
		return;
	}
}
