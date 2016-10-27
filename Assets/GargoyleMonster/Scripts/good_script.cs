using UnityEngine;
using System.Collections;

public class good_script : MonoBehaviour
{
    private Animator am;
    public bool start = false;
    public bool beginAttack = false;

    // Use this for initialization
    void Start()
    {
        am = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (beginAttack == true)
        {
            am.SetTrigger("dummy");
            beginAttack = false;
        } 
    }

    public void Begin()
    {
        AnimationManager.currentAnimationManager.startcalled = true;
        start = true;
    }
}
