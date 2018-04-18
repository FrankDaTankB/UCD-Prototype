using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;

public class LocationManager : MonoBehaviour {

	public WWWManager wwwManager;

	public int timeOutInterval=10;

	public enum LocationState
	{
		undetermined,
		disabled,
		timedOut,
		failed,
		valid,
		userinput,
	};

	public LocationState locationState = LocationState.undetermined;

	string errorCode;

	public string ErrorCode {
		get {return errorCode;}
	}

	string formatted_address;
	float fLat;
	float fLong;
	int dstOffset;
	int rawOffset;
	string daylightSaving;
	string timeZone;

	public string Address {
		get { return formatted_address; }
		set { formatted_address = value; }
	}

	public float Latitude {
		get { return fLat; }
		set { fLat = value; }
	}

	public float Longitude{
		get { return fLong; }
		set { fLong = value; }
	}

	public int DayLightSavingSeconds {
		get { return dstOffset; }
		set { dstOffset = value; }
	}

	public string DayLightSaving {
		get { return daylightSaving; }
		set { daylightSaving = value; }
	}

	public bool IsDaylightSaving {
		get {
			if (dstOffset != 0)
				return true;
			else
				return false;
		}
	}

	public int TimeZoneOffset {
		get { return rawOffset; }
		set { rawOffset = value; }
	}

	public int TimeZoneOffsetHour {
		get { return rawOffset/3600; }
		set { rawOffset = value; }
	}

	public string TimeZoneOffsetString {
		get { return timeZone; }
		set { timeZone = value; }
	}
		

	Action<bool> returnToSender;


	public void clearErrorLog() {
		wwwManager.clearErrorLog ();
		locationState = LocationState.undetermined;
		errorCode = "";
	}

	//
	// public void getLocationData()
	// 
	// this method fetches the location data from google
	// to do that, it needs the latitude and longitude
	//

	public void getLocationData(Action<bool> onResult, int timeOutInterval, float latitude = -1, float longitude = -1) {
		returnToSender = onResult;
		clearErrorLog();
		if (latitude == -1 && longitude == -1) {
			// we have no valid GSP or other lat/long data, 
			// so we need user input to proceed
			errorCode = "No valid geocoordinates available|No valid geo data for location lookup provided. Please enter your address";
			locationState = LocationState.userinput;
			onResult (false);
		} else {
			// GPS data present, get location
			errorCode = "";
			locationState = LocationState.valid;
			getCityFromLatLong (latitude, longitude, timeOutInterval);
		}
	}


	public void getPositionData(Action<bool> onResult, int timeOutInterval, string address) {
		returnToSender = onResult;
		clearErrorLog();
		if (address == "") {
			errorCode = "No address entered|No valid address data for location lookup provided. Please enter your address";
			locationState = LocationState.userinput;
			onResult (false);
		} else {
			// address data present, get location
			locationState = LocationState.valid;
			getLatLongFromCity(address, timeOutInterval);
		}
	}

	public void getTimezoneData(Action<bool> onResult, int timeOutInterval, float latitude = -1, float longitude = -1) {
		returnToSender = onResult;
		clearErrorLog();
		if (latitude == -1 && longitude == -1) {
			// we have no valid GSP or other lat/long data, 
			// so we need user input to proceed
			errorCode = "No valid geocoordinates available|No valid geo data for location lookup provided. Please enter your address";
			locationState = LocationState.failed;
			onResult (false);
		} else {
			// GPS data present, get location
			errorCode = "";
			locationState = LocationState.valid;
			getTimeZone (latitude, longitude, timeOutInterval);
		}
	}

	public void getCityFromLatLong(float fLat, float fLong, int timeOutInterval) {
		// get adress data from google maps for given lat+long
		StartCoroutine (wwwManager.getDataFromURL(onEvaluateWWWRequest, timeOutInterval, "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + fLat.ToString() + "," + fLong.ToString()));
	}


	public void getLatLongFromCity(string city, int timeOutInterval) {
		// get coordinates from google maps for given address data
		StartCoroutine (wwwManager.getDataFromURL(onEvaluateWWWRequest, timeOutInterval, "https://maps.googleapis.com/maps/api/geocode/json?address="+WWW.EscapeURL(city)));
	}

	public void getTimeZone(float flat, float flong, int timeOutInterval) {
		// get the timezone info for the given lat+long
		StartCoroutine (wwwManager.getDataFromURL(onEvaluateWWWRequest, timeOutInterval, 
			"https://maps.googleapis.com/maps/api/timezone/json?location=" + flat.ToString() + "," + flong.ToString() + "&timestamp=" + UnixTimeNow ()));
	}

	public void onEvaluateWWWRequest(bool result) {
		if (result) {
			// everything went fine so far
			errorCode = "LM|LM";
			returnToSender(true);
		} else {
			// we have an error
			errorCode = wwwManager.ErrorCode;
			returnToSender(false);
		}
	}

	//
	// public void parseLatLong(string geoInfo)
	//
	// analyse the adress information provided by google
	// and fill in the variables
	//

	public void parseLatLong(string geoInfo) {
		var Node = JSONNode.Parse (geoInfo);

		string status = Node ["status"];
		if (status == "OK") {
			// get the latitude
			fLat = Node ["results"][0]["geometry"]["location"]["lat"].AsFloat;
			// get the longitude
			fLong = Node ["results"][0]["geometry"]["location"]["lng"].AsFloat;
			// get the address information
			formatted_address = Node ["results"] [0] ["formatted_address"];
		}
		else {	
			formatted_address = "---";
		}
	}


	public void parseTimeZone(string timeInfo, Action<string> onResult) {
		var Node = JSONNode.Parse (timeInfo);
		dstOffset = Node ["dstOffset"].AsInt;
		rawOffset = Node ["rawOffset"].AsInt;
		string timeZoneId = Node ["timeZoneId"];
		string status = Node ["status"];
		if (status == "OK") {
			timeZone = "(UTC ";
			if (rawOffset > 0)
				timeZone += "+ ";
			else
				timeZone += "- ";
			timeZone += (Mathf.Abs(rawOffset) / 3600).ToString ("D2") + ":00) " + timeZoneId;
			if (dstOffset != 0)
				daylightSaving = "yes";
			else
				daylightSaving = "no";
			onResult ("");
		} else {
			onResult (status);
		}
	}



	public long UnixTimeNow()
	{
		var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
		return (long)timeSpan.TotalSeconds;
	}




	//
	// public string getLatLngDegree() 
	//
	// Formats lat+long in degrees
	//

	public string getLatLngDegree() {
		string NS, EW;
		if (fLat > 0)
			NS = "N";
		else
			NS = "S";
		if (fLong > 0)
			EW = "E";
		else
			EW = "W";

		float G1 = Mathf.Abs (fLat);
		float G = Mathf.Floor(G1);
		G1 = (G1 - G) * 60.0f;
		float M = Mathf.Floor(G1);
		float S = (G1 - M) * 60.0f;
		NS = G.ToString () + "°" + NS + " " + (int)M + "' " + (int)S + "''";

		G1 = Mathf.Abs (fLong);
		G = Mathf.Floor (G1);
		G1 = (G1 - G) * 60.0f;
		M = Mathf.Floor(G1);
		S = (G1 - M) * 60.0f;
		EW = G.ToString () + "°" + EW + " " + (int)M + "' " + (int)S + "''";

		return NS + "   " + EW;
	}

}
