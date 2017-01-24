using UnityEngine;

public class RadarStation : Pickup
{
    private bool _stationActive;

	private void Start () 
	{
        activateImmediately = true;
	    _stationActive = false;
	}

    public override void ActivatePickup(GameObject robotGo)
    {
        if (_stationActive)
        {
            return;
        }

        _stationActive = true;

        RobotRadar radar = GetComponentInChildren<RobotRadar>();

        radar.DisablePoI(gameObject);
        radar.EnterRadarStation();
        radar.RunRadarStation();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _stationActive = false;
        GetComponentInChildren<RobotRadar>().ExitRadarStation();
    }

    public override void CleanupPickup()
    {

    }
}
