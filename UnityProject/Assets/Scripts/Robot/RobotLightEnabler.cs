using UnityEngine;

public class RobotLightEnabler : MonoBehaviour
{
    public GameObject lightOverlay;

	private void Start () 
	{
	    lightOverlay.SetActive(true);
	}
}
