using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WWWManager : MonoBehaviour {

	string wwwresult;
	string errorCode;
	WWW wwwErg;

	public WWW WWWResult {
		get { return wwwErg; }
	}

	public string Result {
		get {return wwwresult;}
	}

	public string ErrorCode {
		get {return errorCode;}
	}

	public enum WWWState {
		undetermined,
		disabled,
		timedOut,
		error,
		valid,
	}

	public WWWState wwwState = WWWState.undetermined;


	public void clearErrorLog() {
		wwwState = WWWState.undetermined;
		errorCode = "";
	}


	public IEnumerator getDataFromURL(Action<bool> onResult, int _timeOut, string URL) {

		clearErrorLog ();

		// no network at all
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			wwwState = WWWState.disabled;
			errorCode = "Not connected to network|Please check your network and/or wifi settings and try again.";
			onResult (false);
			yield break;
		}

		// network, but no acecss to internet
		// if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) {
		//	wwwState = WWWState.disabled;
		//	errorCode = "Not connected to internet. Please check your network and/or wifi settings and try again.";
		//	onResult (false);
		//	yield break;
		//}

		// we should have internet access
		WWW www = new WWW (URL);

		// we use a timeout to circumvent the 30 seconds of standard timeout
		int timeOut = _timeOut;
		while (!www.isDone && timeOut > 0) {
			yield return new WaitForSeconds (1);
			timeOut--;
		}

		// connection timed out
		if (timeOut < 1 && !www.isDone) {
			wwwState = WWWState.timedOut;
			errorCode = "Connection timeout|Connection timed out after " + _timeOut + " seconds.\n\nPlease check your internet connection and/or increase the timeout interval.";
			onResult(false);
			yield break;
		}

		// we had a connection, but received an error
		if (www.error != null) {
			wwwState = WWWState.error;
			errorCode = "Unknown error|" + www.error;
			onResult (false);
			yield break;
		}

		// if we catched every possible error before, 
		// everything should be fine now
		wwwresult = www.text;
		wwwState = WWWState.valid;
		wwwErg = www;
		onResult (true);
		yield return www;
	}
		
}
