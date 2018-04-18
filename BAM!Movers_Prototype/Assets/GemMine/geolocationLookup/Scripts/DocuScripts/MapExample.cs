using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapExample : MonoBehaviour {

	public MapManager mapManager;
	public int timeoutInterval=10;
	float latitude, longitude;
	int zoom, mapProvider;

	// Use this for initialization
	void Start () {
		mapManager.getMapData (onMapResult, timeoutInterval, latitude, longitude, zoom, mapProvider);
	}
	
	public void onMapResult(bool result) {
		if (result == false) {
			// something went wrong
			// handle the error here
		}
		else {
			// everything is fine
		}
	}
}
