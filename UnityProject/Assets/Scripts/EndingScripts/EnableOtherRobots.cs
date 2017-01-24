using UnityEngine;
using System.Collections;

public class EnableOtherRobots : MonoBehaviour {


	public GameObject robotContainer;
	// Use this for initialization
	private void Start () {
		robotContainer.SetActive (false);
		RegisterActions ();
	}
	private void OnDisable(){
		if (Toolbox.GetTool<GameManager> () != null) {
			Toolbox.GetTool<GameManager> ().PlayerHasWon -= enableOthers;
		}
	}

	
	private void RegisterActions(){
		Toolbox.GetTool<GameManager> ().PlayerHasWon += enableOthers;
	}
	private void enableOthers(){
		robotContainer.SetActive (true);
	}
}
