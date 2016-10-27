using UnityEngine;
using System.Collections;

public class Spawmer : MonoBehaviour {

    public GameObject zombie;               // The zombie prefab to be spawned.
    public float spawnTime = 3f;            // How long untill first zombie is spawned.
    public float repeatTime = 1f;           // How long between each spawn.
    public GameObject[] spawnPoints;        // An array of spawn points this enemy can spawn at
    private int spawnPointIndex = 0;        // Use this for initialization
    public bool start;

    void Start ()
    {
        
    }
	
    void Update ()
    {
        if (start == true)
        {
            spawnPointIndex = 0;
            start = false;
            InvokeRepeating("Spawn", spawnTime, repeatTime);
        }
    }

	void Spawn ()
    { 
            Instantiate(spawnPoints[spawnPointIndex].GetComponent<ZombieSelector>().zombieType, spawnPoints[spawnPointIndex].transform.position, spawnPoints[spawnPointIndex].transform.rotation);

        if (spawnPointIndex == spawnPoints.Length - 1)
            CancelInvoke("Spawn");
        else
            spawnPointIndex += 1;
    }


}
