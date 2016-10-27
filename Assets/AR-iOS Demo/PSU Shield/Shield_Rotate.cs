using UnityEngine;
using System.Collections;



public class Shield_Rotate : MonoBehaviour {

	public float rotationsPerMinute = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
		
	// Update is called once per frame
	void Update () {
		transform.Rotate(0, 0, 6.0f*rotationsPerMinute*Time.deltaTime);
	}
}
