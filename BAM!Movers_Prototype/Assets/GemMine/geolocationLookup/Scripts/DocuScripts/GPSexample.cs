using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSexample : MonoBehaviour {

	// slot for the package prefabs
	public GPSManager gpsManager;
	// If the GPS Manager does not get a result after
	// 10 seconds, it will cancel the process
	int timeoutInterval = 10;

	void Start () {
		// Startup the GPS Manager
		// If the Manager finishes its work, it will call onGPSResult
		StartCoroutine (gpsManager.initGPS (onGPSResult, timeoutInterval));
	}


	public void onGPSResult(bool result) {
		if (result == false) {
			// something went wrong
			// handle the error here
		}
		else {
			// everything is fine
			// get the latitude and longitude from the gpsManager's properties
			//latitude = gpsManager.Latitude;
			//longitude = gpsManager.Longitude;
		}
	}
}
