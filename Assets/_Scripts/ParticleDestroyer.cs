using UnityEngine;
using System.Collections;

public class ParticleDestroyer : MonoBehaviour {

	private ParticleSystem thisParticleSystem;

	void Start() {

		thisParticleSystem = GetComponent<ParticleSystem>();

		if (!thisParticleSystem.loop) {	
			Destroy(this.gameObject, thisParticleSystem.duration);
		}
	}

}
