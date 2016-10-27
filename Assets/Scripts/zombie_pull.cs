using UnityEngine;
using System.Collections;

public class zombie_pull : MonoBehaviour {

	//BBB Here, we name our target object (s)
	GameObject zombies;

	public float PullRadius; // Radius to pull
	public float GravitationalPull; // Pull force
	public float MinRadius; // Minimum distance to pull from
	public float DistanceMultiplier; // Factor by which the distance affects force

	public LayerMask LayersToPull;


	//Initialise code:
	void Start () {
		//BBB here, we define our target object by searching for its tag (setup in editor)
		zombies = GameObject.FindGameObjectWithTag("monster");
	}

	// Function that runs on every physics frame
	void FixedUpdate()
	{
		
		Rigidbody rb = zombies.GetComponent<Rigidbody>();
	
		Vector3 direction = transform.position - zombies.transform.position;

		float distance = direction.sqrMagnitude*DistanceMultiplier + 1; // The distance formula

		// Object mass also affects the gravitational pull
		rb.AddForce(direction.normalized * (GravitationalPull / distance) * rb.mass * Time.fixedDeltaTime);
	}
}
