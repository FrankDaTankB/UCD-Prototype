using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GPSManager : MonoBehaviour {

	// store the current latitude and longitude

	float latitude;
	float longitude;


	// Property for easy latitude access

	public float Latitude {
		get { return latitude; }
		set { latitude = value; }
	}

	// Property for easy longitude access

	public float Longitude{
		get { return longitude; }
		set { longitude = value; }
	}

	string errorCode;

	public string ErrorCode {
		get { return errorCode; }
	}

	// states the GPS may be in

	public enum GPSstate {
		undetermined,
		disabled,
		timedOut,
		failed,
		valid,
	};

	public GPSstate gpsState = GPSstate.undetermined;


	//
	// public void clearErrorLog() 
	//
	// Before starting a new try,
	// the last connection's event logs
	// have to be reset
	//

	public void clearErrorLog() {
		gpsState = GPSstate.undetermined;
		errorCode = "";
	}


	//
	// public IEnumerator initGPS()
	//
	// This method initializes the GPS module
	// it is called from the outside Manager script

	public IEnumerator initGPS(Action<bool> onResult, int _timeOut) {

		clearErrorLog ();

		// check if GPS is enabled by user
		if (!Input.location.isEnabledByUser) {
			gpsState = GPSstate.failed;
			errorCode = "GPS disabled|GPS must be enabled in order to use this application.";
			onResult (false);
			yield break;
		}

		// start up the location service
		// without started location service, there will be no GPS
		Input.location.Start ();

		// now try to get GPS data, but do not try longer 
		// than the set timeout interval
		int timeOut = _timeOut;
		while (Input.location.status == LocationServiceStatus.Initializing && timeOut > 0) {
			yield return new WaitForSeconds (1);
			timeOut--;
		}

		// did we consume the whole timeout interval?
		if (timeOut < 1) {
			gpsState = GPSstate.timedOut;
			errorCode = "GPS timed out|Connection timed out. Try again later.";
			onResult (false);
			yield break;
		} 
		// GPS did not return a value in the timeout interval
		if (Input.location.status == LocationServiceStatus.Failed) {
			gpsState = GPSstate.failed;
			errorCode = "GPS could not be initialized|Failed to initialize location service.\nTry again later or increase timeout interval.";
			onResult (false);
			yield break;
		} 
		// success!
		// now get the data, store it in the GPS class and return
		else {
			latitude = Input.location.lastData.latitude;
			longitude = Input.location.lastData.longitude;
			gpsState = GPSstate.valid;
			Input.location.Stop ();
			onResult (true);
			yield break;
		}
	}


}
