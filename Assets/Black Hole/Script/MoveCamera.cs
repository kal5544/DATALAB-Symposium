using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour 
{
	public bool Move;

	private Animator Anim;

	void Start()
	{
		Anim = GetComponent<Animator>();
	}

	void Update () 
	{
		if(Move)
		{
			if(Anim.enabled)
				Anim.enabled = false;
			transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0));
			transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
		}
		else
		{
			if(!Anim.enabled)
				Anim.enabled = true;
		}
	}
}
