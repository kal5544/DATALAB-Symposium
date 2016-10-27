using UnityEngine;
using System.Collections;

public class Particlesystem : MonoBehaviour {

	public bool x;
	public ParticleSystem dust;

	void Update () 
	{
		if(x){

			dust.Play();
			ParticleSystem.EmissionModule em = dust.emission;
			em.enabled = true;
		}

		else if(!x){

			dust.Stop();

			//dust.Pause();
			//dust.Clear();
		}
	}
}
