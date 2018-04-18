using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class animateSpinner : MonoBehaviour {

	public Image spinner;
	float timer;
	float timerInterval = 0.0833333f;

	// Use this for initialization
	void Start () {
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		timer += 2*Time.deltaTime;
		if (timer > timerInterval) {
			spinner.transform.Rotate (new Vector3 (0, 0, -24));
			timer -= timerInterval;
		}
	}
}
