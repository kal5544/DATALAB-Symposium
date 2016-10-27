using UnityEngine;
using System.Collections;

public class Zombie : CustomMono {
    private Animator animController;
    private Transform spawnOffset;

    public bool bh = false;
    public bool climb = false;
    public bool t1 = false;
    public bool t2 = false;
    public bool t3 = false;
    public bool t4 = false;
    public float offset = 0.36f;
    // Use this for initialization
    void Start () {
        animController = GetComponent<Animator>();
        TriggerBracedHang();
        GameManager.currentGameManager.zombieList.Add(gameObject);
        bh = false;
           climb = false;
          t1 = false;
          t2 = false;
          t3 = false;
          t4 = false;
        inIdle = false;
        finishedSpawn = false;
    }
    private bool inIdle = false;
    private bool finishedSpawn = false;
    // Update is called once per frame
    void Update () {
	   
	}

    void FixedUpdate()
    {
        if (finishedSpawn && inIdle)
        {
            finishedSpawn = false;

            GameManager.currentGameManager.zombieReady++;
        }
        //this is for testing purpose delete later
        if (bh)
        {
            bh = false;
            TriggerBracedHang();
        }
        if (climb)
        {
            climb = false;
            TriggerClimb();
        }
        if (t1)
        {
            t1 = false;
            TriggerThriller1();
        }
        if (t2)
        {
            t2 = false;
            TriggerThriller2();
        }
        if (t3)
        {
            t3 = false;
            TriggerThriller3();
        }
        if (t4)
        {
            t4 = false;
            TriggerThriller4();
        }
    }

    public void TriggerBracedHang()
    {
        animController.SetTrigger("Braced Hang");
    }
    public void TriggerClimb()
    {
        animController.SetTrigger("Climb");
    }
    public void TriggerThriller1()
    {
        animController.SetTrigger("Thriller 1");
    }
    public void TriggerThriller2()
    {
        animController.SetTrigger("Thriller 2");
    }
    public void TriggerThriller3()
    {
        animController.SetTrigger("Thriller 3");
    }
    public void TriggerThriller4()
    {
        animController.SetTrigger("Thriller 4");
    }
    public void FinishedSpawning()
    {
        inIdle = false;
        finishedSpawn = true;
    }
    public void InIdle()
    {
        inIdle = true;
    }
    public void addOffset()
    {
        transform.Translate(0, offset, 0);

    }
    public void StopAudioHelper()
    {
        GameManager.currentGameManager.TempStopAudio();
    }
    
}
