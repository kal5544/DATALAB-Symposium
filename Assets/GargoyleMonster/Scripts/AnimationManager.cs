using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour {
    public static AnimationManager currentAnimationManager;
    public bool startcalled = false;
	void Awake()
    {
        if (currentAnimationManager == null)
        {
            currentAnimationManager = this;
        }
        else if (currentAnimationManager != this)
        {
            Destroy(gameObject);
        }
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
