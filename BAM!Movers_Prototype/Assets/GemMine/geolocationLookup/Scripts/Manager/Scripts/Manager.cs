using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class Manager : MonoBehaviour {

	// slots for the package prefabs
	public GPSManager gpsManager;
	public LocationManager locationManager;
	public MapManager mapManager;

	//GPS and WWW connection timeout after 10 seconds
	public int timeoutInterval=10;

	public bool useGPSToGetLocationInfo;
	public bool fallbackToGoogleLookup;

	// reference to the ui objects/dialogs
	public GameObject panelError;
	public GameObject panelLoading;
	public GameObject panelPosition;

	// set zoom level
	public Slider zoomSlider;

	// status fields
	public Text txtLatitude;
	public Text txtLongitude;
	public Text txtAddress;

	public Dropdown mapProviderDD;

	// startup values for GPS coordinates
	float latitude = -1;
	float longitude = -1;

	// we need a zoom value to start with
	int mapScale=16;

	// Use this for initialization
	void Start () {
		mapScale = (int)zoomSlider.value;
		// deactivate the status panels
		panelError.SetActive (false);
		panelLoading.SetActive (false);
		panelPosition.SetActive (false);

	}

	//
	// 	public void getGeo() {
	//
	// User pressed "Get Address" Button on the UI
	//

	public void getGeo() {
		getLocationInformation ();
	}



	//
	// 	public void getGPS() {
	//
	// User pressed "Get GPS Data" Button on the UI
	//

	public void getGPS() {
		getGPSInformation ();
	}



	//
	// 	public void getMap() {
	//
	// User pressed "Get Map Data" Button on the UI
	//

	public void getMap() {
		getMapInformation (latitude, longitude, mapProviderDD.value, mapScale);
	}



	//
	// public void setMapScale()
	//
	// User dragged the slider button on the zoom slider
	//

	public void setMapScale() {
		mapScale = (int)zoomSlider.value;
	}


	public void zoomSliderChanged() {
		setMapScale ();
	}


	//
	// public void zoomInMap() 
	//
	// User pressed the "+" Button on the slider
	//

	public void zoomInMap() {
		if (zoomSlider.value < zoomSlider.maxValue) {
			zoomSlider.value++;
			setMapScale();
		}
	}



	//
	// public void zoomOutMap() 
	//
	// User pressed the "-" Button on the slider
	//

	public void zoomOutMap() {
		if (zoomSlider.value > zoomSlider.minValue) {
			zoomSlider.value--;
			setMapScale();
		}
	}

	// (1) getGPSInformation gets called and tries to deliver GPS information
	//     The result of the operation will be transmitted to onGPSResult,
	//     which is called next (as an event function) and evaluates

	//
	// public void getGPSInformation() 
	//
	// this method starts up the GPS receiver
	// and tries to get the coordinates
	//

	public void getGPSInformation() {
		// show the loading status/animation
		showLoader("Locating position","Connecting to GPS receiver.\nTrying to retrieve GPS position data.");
		// try to get the GPS position
		//after returning from the Coroutine, onGPSResult is called
		StartCoroutine (gpsManager.initGPS (onGPSResult, timeoutInterval));
	}



	// (2) calling the GPSManager will deliver a result (true/false)
	//     if the result is true, we have valid GPS information
	//     if the result is false, we do not have valid GPS information

	//
	// public void onGPSResult(bool result)
	//
	// This method evaluates the result delivered 
	// from the GPS method
	//

	public void onGPSResult(bool result) {
		// we have a result, so let's hide the loader animation
		hideLoader ();

		// the call produced an error
		// now show the error and call the Location Manager afterwards (getLocationInformation())
		// calling the Location Manager without any parameters will cause him to prompt user to enter a
		// valid address he can lookup information for

		if (result == false) {
			showError (gpsManager.ErrorCode + "\n\nIf your device has no GPS installed, please provide your location in the following dialog.", () => {
				getLocationInformation ();
			});
		}
		// everything is fine
		// use the lat/long to get more info
		else {
			txtLatitude.text = "Latitude: " + gpsManager.Latitude.ToString();
			txtLongitude.text = "Longitude: " + gpsManager.Longitude.ToString();

			latitude = gpsManager.Latitude;
			longitude = gpsManager.Longitude;

			// if user wants to use the GPS data to get more information,
			// start up the Location Manager with the provided latitude/longitude data
			if (useGPSToGetLocationInfo) {
				getLocationInformation (gpsManager.Latitude, gpsManager.Longitude);
			}
		}
	}



	// (3) this calls up the Location Manager
	//     The result of the operation will be transmitted to onLocationResult,
	//     which is called next (as an event function) and evaluates

	//
	// public void getLocationInformation(float latitude=-1, float longitude=-1)
	//
	// this method calls the Location Manager. It can be called with or without parameters
	// if called without parameters, it will set latitude/longitude to default -1, causing an error,
	// calling onLocationResult with result false
	//

	public void getLocationInformation(float latitude=-1, float longitude=-1) {
		// show the loading status/animation
		showLoader("Retrieving position info","Connecting to gogle map to get address data for GPS coordinates.\n\nNow retrieving location info.");
		locationManager.getLocationData (onLocationResult, timeoutInterval, latitude, longitude);
	}


	public void getPositionInformation(string address) {
		// show the loading status/animation
		showLoader("Retrieving position info","Connecting to gogle map to get address data for GPS coordinates.\n\nNow retrieving location info.");
		locationManager.getPositionData (onLocationResult, timeoutInterval, address);
	}



	// (4) calling the Location Manager will deliver a result (true/false)
	//     if the result is true, we have valid Location information
	//     if the result is false, we do not have valid Location information


	public void onLocationResult(bool result) {
		// we have a result, so let's hide the loader animation
		hideLoader ();

		// the result is true, so the web request returned a result
		if (result) {
			locationManager.parseLatLong(locationManager.wwwManager.Result);
			txtLatitude.text = "Latitude: " + locationManager.Latitude.ToString ();
			txtLongitude.text = "Longitude: " + locationManager.Longitude.ToString ();
			txtAddress.text = locationManager.Address.ToString ();

			if (latitude == -1 && longitude == -1) {
				latitude = locationManager.Latitude;
				longitude = locationManager.Longitude;
			}
		} 

		// we have an error
		// let's find out, which one
		else {
			if (locationManager.wwwManager.wwwState == WWWManager.WWWState.error) {
				showError (locationManager.ErrorCode, () => {
					hideError ();
				});
			
			}
			else if (locationManager.wwwManager.wwwState == WWWManager.WWWState.timedOut) {
				showError (locationManager.ErrorCode, () => {
					hideError ();
				});
			}
			else if (locationManager.wwwManager.wwwState == WWWManager.WWWState.disabled) {
				showError (locationManager.ErrorCode, () => {
					hideError ();
				});
			}
			else if (locationManager.locationState == LocationManager.LocationState.userinput) {
				// we have no valid geo coordinates,
				// so the user has to fill in his address to lookup his position
				showLocation(() => {hideLocation();});
			} 
		}
	}


	// (5) this calls up the Map Manager
	//     The result of the operation will be transmitted to onMapResult,
	//     which is called next (as an event function) and evaluates

	//
	// public void getMap(float latitude, float longitude)
	//
	// this method calls the Map Manager.
	//

	public void getMapInformation(float latitude, float longitude, int mapProvider, int zoom=13) {
		// show the loading status/animation
		if (latitude != -1 && longitude != -1) {
			showLoader ("Retrieving google map", "Connecting to gogle map to get map data for GPS coordinates.");
			mapManager.getMapData (onMapResult, timeoutInterval, latitude, longitude, zoom, mapProvider);
		} else {
			showError ("No map retrieved|Could not determine latitude and longitude. Please get GPS or address data first.", () => {
				hideError ();
			});
			//mapManager.getMapData (onMapResult, timeoutInterval, 51.65968f, 8.352985f, zoom, mapProvider);
		}
	}


	public void onMapResult(bool result) {
		// we have a result, so let's hide the loader animation
		hideLoader ();

		// the result is true, so the web request returned a result
		if (result) {
			// everything is fine
		}else {
			// we have an error
			showError (mapManager.ErrorCode, () => {
				hideError ();
			});
		}
	}


	//
	// public void showError(...)
	//
	// show the error screen
	// this is one of the UI dialog components
	//

	public void showError(string errorText, UnityAction methodAction ) {
		string[] message = errorText.Split ('|');
		panelError.transform.Find("pnlTitle").Find("txtTitle").GetComponent<Text> ().text = message[0];
		panelError.transform.Find ("txtError").GetComponent<Text> ().text = message [1];
		panelError.transform.Find ("Button").GetComponent<Button> ().onClick.AddListener(methodAction);
		panelError.SetActive (true);
	}
		
	//
	// public void hideError()
	//
	// hide the error screen again
	//

	public void hideError() {
		panelError.transform.Find ("Button").GetComponent<Button> ().onClick.RemoveAllListeners ();
		panelError.SetActive (false);
	}



	//
	// public void showLoader(...)
	//
	// show the loading screen
	// this is one of the UI dialogs showing the preloading animation
	//

	public void showLoader(string title, string errorText) {
		panelLoading.transform.Find("pnlTitle").Find("txtTitle").GetComponent<Text> ().text = title;
		panelLoading.transform.Find("txtError").GetComponent<Text> ().text = errorText;
		panelLoading.SetActive (true);
	}

	//
	// public void hideLoader()
	//
	// hide the loading screen
	//

	public void hideLoader() {
		panelLoading.SetActive (false);
	}



	//
	// public void showLocation(UnityAction methodAction )
	//
	// This method shows the dialog prompting the user for address input
	//

	public void showLocation(UnityAction methodAction ) {
		panelError.SetActive (false);
		panelPosition.SetActive (true);
		panelPosition.transform.Find ("Button").GetComponent<Button> ().onClick.AddListener(methodAction);
	}

	public void hideLocation() {
		string address;
		address = panelPosition.transform.Find ("TxtCity").Find("InputField").Find ("Text").GetComponent<Text> ().text;
		panelPosition.transform.Find ("Button").GetComponent<Button> ().onClick.RemoveAllListeners ();
		panelPosition.SetActive (false);
		if (address == "") {
			showError("No valid address provided|Please enter a valid address in the dialog.", () => {
				hideError();
			});
		} else {
			getPositionInformation (address);
		}
	}


}
