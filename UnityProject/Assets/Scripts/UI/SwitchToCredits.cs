using UnityEngine;
using System.Collections;

public class SwitchToCredits : MonoBehaviour {

	public Canvas creditsCanvas;

	private void Start () {
		creditsCanvas.enabled = false;
	}
	
	public void OpenCredits(){//used by credits button
		creditsCanvas.enabled = true;
	}

	public void BackToGame(){
		creditsCanvas.enabled = false;//used by back button
	}
}
