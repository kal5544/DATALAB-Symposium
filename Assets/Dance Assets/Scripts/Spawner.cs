using UnityEngine;
using System.Collections;

public class Spawner : CustomMono {
    private Animator animController;

	// Use this for initialization
	void Start () {
        animController = GetComponent<Animator>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void EnableSpawn()
    {
        animController.SetTrigger("Grow");
    }

    public void SpawnZombie()
    {

    }
}
