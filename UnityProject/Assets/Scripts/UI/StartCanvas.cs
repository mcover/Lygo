using UnityEngine;
using System.Collections;

public class StartCanvas : MonoBehaviour {

	public Canvas guiCanvas;
	public Canvas startCanvas;
	public Canvas winCanvas;

	void Start() {
		Toolbox.GetTool<RobotInputHandler> ().socketActivating = true;//should stop input control
		int start = PlayerPrefs.GetInt ("startScreen?");
		if (start == 1) {
			guiCanvas.enabled = false;
			winCanvas.enabled = false;
			startCanvas.enabled = true;
			startCanvas.GetComponent<CanvasGroup> ().alpha = 1;
			PlayerPrefs.SetInt("startScreen?", 0);
		} else {
			StartGame();
		}
	
	}

	

	public void StartGame(){

		guiCanvas.enabled = true;
		winCanvas.enabled = true;
		startCanvas.enabled = false;
		Toolbox.GetTool<RobotInputHandler> ().socketActivating = false;//should start input control
        Toolbox.GetTool<CameraManager>().StartingZoom();
	}
}
