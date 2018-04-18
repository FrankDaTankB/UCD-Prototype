using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationExample : MonoBehaviour {

	public LocationManager locationManager;

	int timeoutInterval = 10;
	float latitude, longitude;
	string address;

	void Start () {
		// get the info by providing latitude and longitude
		locationManager.getLocationData (onLocationResult, timeoutInterval, latitude, longitude);
		// get the information by providing an address string
		locationManager.getPositionData (onLocationResult, timeoutInterval, address);
	}
	
	public void onLocationResult(bool result) {
		if (result == false) {
			// something went wrong
			// handle the error here
		}
		else {
			// parse the JSON object google delivered
			locationManager.parseLatLong(locationManager.wwwManager.Result);
			// get access to the properties by
			// locationManager.Latitude.ToString ();
			// locationManager.Longitude.ToString ();
			// locationManager.Address.ToString ();
		} 
	}
}
