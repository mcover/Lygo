using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GemUIScript : MonoBehaviour {

	public int gemIdentifier;
	public GameObject glowHolder;

	public int getGemNumber(){
		return gemIdentifier;
	}

}
