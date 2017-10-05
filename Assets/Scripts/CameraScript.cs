using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CameraScript : MonoBehaviour {
	bool camAvailable;
	WebCamTexture webcamTexture;
	Texture defaultBackground;
	public RawImage rawImage;
	public AspectRatioFitter fit;
	// Use this for initialization
	void Start () {
		defaultBackground = rawImage.texture;
		WebCamDevice[] devices = WebCamTexture.devices;
		if (devices.Length == 0) {
			Debug.Log ("No camera");
			camAvailable = false;
			return;
		}

		for(int i = 0; i < devices.Length; i++){
			if(devices[i].isFrontFacing){
				webcamTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
			}
		}

		if(webcamTexture == null){
			Debug.Log("Unable to find camera");
			return;
		}
		webcamTexture.Play();
		rawImage.texture = webcamTexture;
		camAvailable = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!camAvailable)
			SceneManager.LoadScene ("Main");
		else {
			float ratio = (float)webcamTexture.width / (float)webcamTexture.height;
			fit.aspectRatio = ratio;

			float scaleY = webcamTexture.videoVerticallyMirrored ? -1f : 1f;
			rawImage.rectTransform.localScale = new Vector3 (1f, scaleY, 1f);

			int orient = -webcamTexture.videoRotationAngle;
			rawImage.rectTransform.localEulerAngles = new Vector3 (0, 0, orient);
		}
	}

	public void SavePhoto(){
		string _SavePath = Application.persistentDataPath + "/";
		Texture2D snap = new Texture2D(rawImage.mainTexture.width, rawImage.mainTexture.height, TextureFormat.ARGB32, false);
		snap.SetPixels(webcamTexture.GetPixels()); 
		snap.Apply();
		snap = PortraitToLandscape(snap);
		System.IO.File.WriteAllBytes(_SavePath + "avatar.png", snap.EncodeToPNG());
		SceneManager.LoadScene("Main");
	}

	Texture2D PortraitToLandscape(Texture2D origTex) {
		var origPix = origTex.GetPixels();
		var newPix = new Color[origPix.Length];

		for (var x = 0; x < origTex.width; x++) {
			for (var y = 0; y < origTex.height; y++) {
				var newX = y;
				var newY = origTex.width - x - 1;
				newPix[y * origTex.width + x] = origPix[newY * origTex.height + newX];
			}
		}
		var newTex = new Texture2D(origTex.height, origTex.width);
		newTex.SetPixels(newPix);
		newTex.Apply();
		return newTex;
	}
}
