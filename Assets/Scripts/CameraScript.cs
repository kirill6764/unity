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
		/*rawImage.material.mainTexture = webcamTexture;
		*/
	}
	
	// Update is called once per frame
	void Update () {
		if (!camAvailable)
			return;

		float ratio = (float)webcamTexture.width / (float)webcamTexture.height;
		fit.aspectRatio = ratio;

		float scaleY = webcamTexture.videoVerticallyMirrored ? -1f : 1f;
		rawImage.rectTransform.localScale = new Vector3 (1f, scaleY, 1f);

		int orient = -webcamTexture.videoRotationAngle;
		rawImage.rectTransform.localEulerAngles = new Vector3 (0, 0, orient);

	}

	public void SavePhoto(){
		string _SavePath = Application.persistentDataPath + "/";
		Texture2D snap = new Texture2D(webcamTexture.width, webcamTexture.height);
		snap.SetPixels(webcamTexture.GetPixels());
		snap.Apply();

		System.IO.File.WriteAllBytes(_SavePath + "avatar.png", snap.EncodeToPNG());
		SceneManager.LoadScene("Main");
	}
}
