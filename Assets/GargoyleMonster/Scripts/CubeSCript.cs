using UnityEngine;
using System.Collections;

public class CubeSCript : MonoBehaviour {

    private Animator a;
	// Use this for initialization
	void Start () {
        a = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if (AnimationManager.currentAnimationManager.startcalled == true)
        {
            a.SetTrigger("cubetrigger");
            AnimationManager.currentAnimationManager.startcalled = false;
        }
	}

    
}
