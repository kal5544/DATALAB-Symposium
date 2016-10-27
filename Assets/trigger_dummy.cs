using UnityEngine;
using System.Collections;

public class trigger_dummy : MonoBehaviour {
    private Animator am;
    public bool start = false;

	// Use this for initialization
	void Start () {
        am = GetComponent<Animator>();
        am.SetTrigger("dummy");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Begin()
    {
        start = true;
    }
}
