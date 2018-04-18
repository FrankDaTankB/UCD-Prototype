using UnityEngine;
using System.Collections;

public class errorPanel : MonoBehaviour {

	//
	// public void closeDialog()
	//
	// User clicks on OK button on the error dialog
	// and closes it
	//

	public void closeDialog() {
		gameObject.SetActive (false);
	}
}
