using UnityEngine;
using System.Collections;

public class SetPlayerPref : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PlayerPrefs.SetInt ("startScreen?", 1);
		PlayerPrefs.Save ();
		Application.LoadLevel (1);
	
	}
	

}
