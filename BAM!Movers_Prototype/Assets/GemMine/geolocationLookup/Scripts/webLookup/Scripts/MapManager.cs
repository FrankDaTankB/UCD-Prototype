using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class MapManager : MonoBehaviour {

	public WWWManager wwwManager;
	public Image mapImage;

	public string googleAPIKey;
	public string mapQuestAPIKey;
	public string bingAPIKey;

	string errorCode;

	public string ErrorCode {
		get {return errorCode;}
	}
		
	// google map server
	string googleServer = "https://maps.googleapis.com/maps/api/staticmap?";
	// mapquest server
	string mapquestServer = "https://beta.mapquestapi.com/staticmap/v5/map?";
	// the map's zoom level
	int zoomLevel;
	// color for marker and line (and code for the app)
	string col = "0x338a56";
	string col2 = "338a56";
	// image width
	int width;
	// image height;
	int height;
	int mapwidth;
	int mapheight;
	int scaleFactor;

	Texture2D mapTexture;

	Action<bool> returnToSender;

	public void clearErrorLog() {
		wwwManager.clearErrorLog ();
		errorCode = "";
	}

	public void getMapData(Action<bool> onResult, int timeOutInterval, float latitude, float longitude, int zoom, int mapProvider) {
		returnToSender = onResult;
		clearErrorLog ();
		width = Screen.width;
		height = Screen.height;

		string url;

		switch (mapProvider) {
		case 0:
			// google maps
			// zoom: 1 bis 21
			// APIkey: AIzaSyBHns83FtBHBrPIJ_pAdrrgRsMrfBdn4IM 
			// calculation for google
			// API key ist nicht zwingend erforderlich

			scaleFactor = (width > height) ? (int) (width / 640) : (int) (height / 640);
			mapwidth = width / scaleFactor;
			mapheight = height / scaleFactor;

			url = googleServer + 
				"center=" + latitude.ToString() + "," + longitude.ToString() +
				"&zoom=" + zoom.ToString() + 
				"&scale=2&size=" + mapwidth.ToString() + "x" + mapheight.ToString() + 
				"&maptype=roadmap&format=png&visual_refresh=true" + 
				"&markers=size:mid%7C" + "color:" + col + 
				"%7Clabel:%7Clippstadt%7C" + latitude.ToString() + "," + longitude.ToString() +
				"&sensor=false";
			
			//+"&key=AIzaSyBHns83FtBHBrPIJ_pAdrrgRsMrfBdn4IM";

			getMap (url, timeOutInterval);
			break;

		case 1:
			// mapquest unterstützt Größen bis 1920x1920
			// zoom: 0 bis 20
			// size=1080,1920@2x liefert Retina-Auflösung von 2160x3840
			// key = S96JFgM5OYGje3ZJbngY1wNBb8Nf6X7x
			// retina wird eigentlich nicht benötigt und verfierfacht die dateigröße auf 1MB
			// defaultMarker=https://developer.mapquest.com/documentation/assets/img/apple-icon-152x152.png

			scaleFactor = (width > height) ? (int)(width / 640) : (int)(height / 640);
			mapwidth = width / scaleFactor;
			mapheight = height / scaleFactor;

			url = mapquestServer +
			"center=" + latitude.ToString () + "," + longitude.ToString () +
			"&locations=" + latitude.ToString () + "," + longitude.ToString () +
			"&defaultMarker=marker-lg-" + col2 + "-22407F" +
			"&zoom=" + zoom.ToString () +
			"&size=" + mapwidth.ToString () + "," + mapheight.ToString () + "@2x" +
			"&key=S96JFgM5OYGje3ZJbngY1wNBb8Nf6X7x";

			getMap (url, timeOutInterval);
			break;

		case 2:
			// Microsoft Bing

			scaleFactor = (width > height) ? (int) (width / 640) : (int) (height / 640);
			mapwidth = width / scaleFactor;
			mapheight = height / scaleFactor;

			url = "http://dev.virtualearth.net/REST/V1/Imagery/Map/Road/" +
				latitude.ToString () + "," + longitude.ToString () +
				"/" + zoom.ToString () + 
				"?mapSize=" + mapwidth.ToString() + "," + mapheight.ToString() + 
				"&format=png" + 
				"&pushpin=" + latitude.ToString() + "," + longitude.ToString() + ";37;" + 
				"&key=AqccaY1x29TVgwUxGzI3ZtbEGTPPISGqZgEcEP5e10tI1OX_IrGcaNkproUsrub1";

			getMap (url, timeOutInterval);
			break;

		default:
			break;
		}
			
	}

	public void getMap(string url, int timeOutInterval) {
		// get adress data from google maps for given lat+long
		StartCoroutine (wwwManager.getDataFromURL(onEvaluateWWWRequest, timeOutInterval, url));
	}

	public void onEvaluateWWWRequest(bool result) {
		if (result) {
			// everything went fine so far
			errorCode = "LM|LM";
			// we have no error and downloaded something
			if (wwwManager.WWWResult.texture != null) {
				// we have not downloaded a valid map				
				if (wwwManager.WWWResult.texture.width == 8 || wwwManager.WWWResult.texture.height == 8) {
					errorCode = "No valid map|The ressource is no valid picture map!\n\nMap picture cannot be loaded.\n\nPlease check for typo error in the url.";
					returnToSender (false);
				}

				// by now, we should have a static map
				// get the texture
				mapTexture = wwwManager.WWWResult.texture;
				// convert it to a sprite
				Sprite sprite = Sprite.Create(mapTexture, new Rect(0,0,mapTexture.width,mapTexture.height),new Vector2(0.5f,0.5f),1.0f);
				mapImage.GetComponent<Image> ().sprite = sprite;
				returnToSender (true);
			}
				
			returnToSender(true);
		} else {
			// we have an error
			errorCode = wwwManager.ErrorCode;
			returnToSender(false);
		}
	}

}

