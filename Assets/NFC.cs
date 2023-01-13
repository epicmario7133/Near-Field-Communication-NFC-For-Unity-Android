using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class NFC : MonoBehaviour {

	public string tagID;
	public Text tag_output_text;
	public bool tagFound = false;

	private AndroidJavaObject mActivity;
	private AndroidJavaObject mIntent;
	private string sAction;


	void Start() {
		tag_output_text.text = "Scan a NFC tag to make the cube disappear...";
	}

	void Update() {
		if (Application.platform == RuntimePlatform.Android) {
			if (!tagFound) {
				try {
					// Create new NFC Android object
					mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"); // Activities open apps
					mIntent = mActivity.Call<AndroidJavaObject>("getIntent");
					sAction = mIntent.Call<String>("getAction"); // resulte are returned in the Intent object
					if (sAction == "android.nfc.action.NDEF_DISCOVERED")
					{
						Debug.Log("Tag of type NDEF");
						AndroidJavaObject[] rawMsg = mIntent.Call<AndroidJavaObject[]>("getParcelableArrayExtra", "android.nfc.extra.NDEF_MESSAGES");
						AndroidJavaObject[] records = rawMsg[0].Call<AndroidJavaObject[]>("getRecords");
						byte[] payLoad = records[0].Call<byte[]>("getPayload");
						string result = System.Text.Encoding.Default.GetString(payLoad); // not sure if it works for all encodings, but it works for me
						result = result.Replace("en", ""); //rimuovi EN
						tag_output_text.text = result; // first few letters are about used language (for english "en..")
					}
					else if (sAction == "android.nfc.action.TAG_DISCOVERED") {
						Debug.Log("This type of tag is not supported !");
					}
					else {
						tag_output_text.text = "Scan a NFC tag to make the cube disappear...";
						return;
					}
				}
				catch (Exception ex) {
					string text = ex.Message;
					tag_output_text.text = text;
				}
			}
		}
	}
}