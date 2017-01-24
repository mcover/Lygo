using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
	private GameObject worldGem;
	private RobotBatteryManager batteryManager;
	private RobotCableManager cableManager;
	private bool batteryActive;
	public GameObject unplugHolder;
	public List<GameObject> gemHolderList;
	private RobotInputHandler inputHandler;
	public bool socketActive=false;

	private void Awake(){
		Toolbox.RegisterAsTool (this);
	}

	private void OnDestroy(){
		Toolbox.UnregisterTool (this);
	}

	private void Start () {
		batteryActive = false;
		batteryManager = Toolbox.GetTool<RobotBatteryManager> ();
		cableManager = Toolbox.GetTool<RobotCableManager> ();
		inputHandler = Toolbox.GetTool<RobotInputHandler> ();

		foreach (GameObject gemHolder in gemHolderList) {
			gemHolder.SetActive(false);
			LeanTween.scale(gemHolder, new Vector3 (0.1f, 0.1f, 1),0.1f); //this is necessary so that when the gem is picked up the holder can scale up
		}

		Button unplugButton = unplugHolder.GetComponentInChildren<Button> ();
		if (inputHandler.mouseBlocker != unplugButton.transform as RectTransform&& inputHandler.mouseBlocker!=null) {
			Debug.LogError("Error: multiple UI controllers found (mouseBlocker))");
		}
		inputHandler.mouseBlocker = unplugButton.transform as RectTransform; //prevents pressing the unplug button from moving the robot
		RegisterActions ();
		unplugHolder.SetActive (false);
	}

	private void RegisterActions(){// the the static actions happen somewhere the corresponding function is called
		Socket.GemAdded += ActivateGem;
		GemPickup.PickedUpGem += ShowGemUI;
	}
	private void OnDisable(){
		Socket.GemAdded -= ActivateGem;
		GemPickup.PickedUpGem -= ShowGemUI;
	}

	private void Update () { //controls the unplug ui activation and deactivation the 0.0001 keeps the player from dying on start
		if (socketActive == false) {
			if (batteryActive == false && batteryManager.currentBatteryPower > 0.0001) {
				batteryActive = true;
				ActivateUnplugUI (); 
			}
			if (cableManager.cableEndGo == null) {
				DeactivateUnplugUI ();
			}
			if (batteryManager.currentBatteryPower > 0.0001 && cableManager.cableEndGo != null) {
				ActivateUnplugUI ();
			}
		}
	}

	private void ActivateUnplugUI(){
		unplugHolder.SetActive (true);
	}
	private void DeactivateUnplugUI(){
		unplugHolder.SetActive (false);
	}

	private void ShowGemUI(int gemNum, GameObject sender){
		StartCoroutine(ShowGem (gemNum, sender));
	}

	private IEnumerator ShowGem(int gemNum, GameObject sender){
		//starts the movement of the pickup towards the ui gem
		foreach (GameObject gemHolder in gemHolderList) {
			int gemNumber = gemHolder.GetComponent<GemUIScript>().getGemNumber();
			if (gemNum==gemNumber){
				MoveTo mover = sender.AddComponent<MoveTo>();
				mover.destination = gemHolder.transform;
				yield return new WaitForSeconds(.3F);
				gemHolder.SetActive(true);
				LeanTween.scale(gemHolder,new Vector3 (1f, 1f, 1),.5f);
			}
		}

	}
	private void ActivateGem(int gemNum){
		//caused the ui glow to start
		foreach (GameObject gemHolder in gemHolderList) {
			int gemNumber = gemHolder.GetComponent<GemUIScript>().getGemNumber();
			if (gemNum==gemNumber){
				GameObject glowHolder = gemHolder.GetComponent<GemUIScript>().glowHolder;
				glowHolder.SetActive(true);

			}
		}
	}
	
	public Sprite getGemUISprite(int gemNum){
		//return the sprite used by the ui for the corresponding number
		foreach (GameObject gemHolder in gemHolderList) {
			int gemNumber = gemHolder.GetComponent<GemUIScript> ().getGemNumber ();
			if (gemNum == gemNumber) {
				Image newImage =  gemHolder.GetComponentInChildren<CanvasRenderer> ().GetComponent<Image>();
				return newImage.sprite;
			}

		}
		throw new ArgumentOutOfRangeException("bad gem number");
	}

	public Vector3 getGemUIWorldPosition(int gemNum){
		//called by SocketFeedbackController, gives the point of origin for the shape that moves from the gem UI to the socket
		foreach (GameObject gemHolder in gemHolderList) {
			int gemNumber = gemHolder.GetComponent<GemUIScript>().getGemNumber();
			if (gemNum == gemNumber){
				
				RectTransform screenUITransform = gemHolder.transform as RectTransform;
				Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint (null, screenUITransform.position);
				
				Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
				//Debug.Log ("world point: " + worldPoint + "screen point :" + screenPoint+" camera: " +Camera.main.transform.position);
				worldPoint.z = 0;
				return worldPoint;
				}
			}
		throw new ArgumentOutOfRangeException("bad gem number");
	}


}
