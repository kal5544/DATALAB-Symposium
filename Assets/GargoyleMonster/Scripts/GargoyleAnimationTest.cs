using UnityEngine;
using System.Collections;

public class GargoyleAnimationTest : MonoBehaviour {
	float speed, height;
	bool flying, sleep, eating;
	
	Animator animator;
	
	
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	
	void OnGUI() {
		GUILayout.BeginArea(new Rect(10, 10, 128, 512));
		
		GUILayout.Label("Speed");
		speed = GUILayout.HorizontalSlider(speed, 0, 1);
		flying = GUILayout.Toggle(flying, "Fly");
		GUILayout.Label("Flying Height");
		height = GUILayout.HorizontalSlider(height, 0, 1);
		sleep = GUILayout.Toggle(sleep, "Sleep");
		eating = GUILayout.Toggle(eating, "Eat");
		GUILayout.EndArea();
		if(GUI.changed) {
			animator.SetFloat("speed", speed);
			animator.SetFloat("height", height);
			animator.SetBool("flying", flying);
			animator.SetBool("sleep", sleep);
			animator.SetBool("eating", eating);
		}
	}
	
	
}
