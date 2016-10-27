using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : CustomMono {
    public static GameManager currentGameManager;
    
    public Spawmer spawner;
    public bool startSpawn;
    public Zombie testZombie;
    public ArrayList zombieList;
    public int zombieReady;
    private bool play;
    private bool danceInProgress;
    AudioSource audio;
    public bool stopAudio = false;
    void Awake()
    {
        if(currentGameManager == null)
        {
            currentGameManager = this;
        }else if(currentGameManager != this)
        {
            Destroy(transform.gameObject);
        }
        zombieList = new ArrayList();
        audio = GetComponent<AudioSource>();
        startSpawn = false;
        play = true;
        danceInProgress = false;
    }
    public void Reset()
    {
        danceInProgress = false;
        PlayPause();
    }
    public void PlayPause()
    {
        if (!danceInProgress)
        {
            danceInProgress = true;
            if (zombieList.Count != 0)
            {
                foreach (GameObject zombie in zombieList)
                {
                    Destroy(zombie);

                }
            }
            audio.Stop();
            startSpawn = true;
        }else
        {
            if (play)
            {
                play = !play;
                if (zombieList.Count != 0)
                {
                    foreach (GameObject zombie in zombieList)
                    {
                        if(zombie)
                            zombie.GetComponent<Animator>().enabled = false;

                    }
                }
                if (audio.isPlaying)
                {
                    audio.Pause();
                }
                

            }
            else
            {
                play = !play;
                foreach (GameObject zombie in zombieList)
                {
                    if (zombie)
                        zombie.GetComponent<Animator>().enabled = true;

                }
                if (!audio.isPlaying)
                {
                    audio.UnPause();
                }
            }
        }
    }



	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	    if(zombieReady != 0 && zombieReady == spawner.spawnPoints.Length)
        {
            zombieReady = 0;
            foreach (GameObject zombie in zombieList)
            {
                if(zombie)
                    zombie.GetComponent<Zombie>().TriggerThriller1();
                
            }
            audio.Play();
        }
        if (startSpawn)
        {
            startSpawn = false;
            spawner.start = true;

        }
        if (stopAudio)
        {
            stopAudio = false;
            audio.Stop();
            Debug.Log(audio.timeSamples);
        }
	}
    public void TempStopAudio()
    {
        if (audio.isPlaying)
        {
            danceInProgress = false;
            audio.Stop();
        }
    }
    
}
