using System;
using System.Collections.Generic;
using UnityEngine;

public class RobotAnimationManager : MonoBehaviour
{
    public float robotDeathAnimationTime = 2;
    public GameObject robotLightGo;
    public GameObject robotParticlesGo;
	public GameObject endParticles;

    private RobotPickupManager _robotPickupManager;
    private RobotCableManager _robotCableManager;
    private Animator _animator;

    private int _robotDeathId;
    private int _unpluggingId;

    public const string c_alwaysVisibleSortingLayer = "AlwaysVisible";

    private void Awake()
    {
        Toolbox.RegisterAsTool(this);
    }

    private void OnDestroy()
    {
        Toolbox.UnregisterTool(this);
    }

    private void RegisterActions()
    {
        Toolbox.GetTool<RobotCableManager>().RobotPlugged += CheckForGems;
        Toolbox.GetTool<RobotCableManager>().RobotUnplugged += RobotUnplugging;
        Toolbox.GetTool<RobotBatteryManager>().BatteryRanOut += RobotDeath;
		Toolbox.GetTool<GameManager> ().PlayerHasWon += addParticlesOnWin;
    }

    private void OnDisable()
    {
        if (Toolbox.GetTool<RobotCableManager>() != null)
        {
            Toolbox.GetTool<RobotCableManager>().RobotPlugged -= CheckForGems;
            Toolbox.GetTool<RobotCableManager>().RobotUnplugged -= RobotUnplugging;
        }

        if (Toolbox.GetTool<RobotBatteryManager>() != null)
        {
            Toolbox.GetTool<RobotBatteryManager>().BatteryRanOut -= RobotDeath;
        }
		if (Toolbox.GetTool<GameManager> () != null) {
			Toolbox.GetTool<GameManager>().PlayerHasWon-=addParticlesOnWin;
		}
    }

    private void Start()
    {
        RegisterActions();

        _robotPickupManager = Toolbox.GetTool<RobotPickupManager>();
        _robotCableManager = Toolbox.GetTool<RobotCableManager>();
        _animator = GetComponent<Animator>();

        _robotDeathId = Animator.StringToHash("RobotDeath");
        _unpluggingId = Animator.StringToHash("Unplugging");
    }
	

    public void CheckForGems()
    {
        // No cable = nothing to check.
        if (_robotCableManager.cableEndGo == null)
        {
            return;
        }

        Socket socket = _robotCableManager.cableEndGo.GetComponent<CableEnd>().socket.GetComponent<Socket>();
        List<GemPickup> gems = _robotPickupManager.GetPickupsOfType<GemPickup>();

        for (int i = 0; i < gems.Count; i++)
        {
            // If it's the gem
            if (socket.GemFitsSocket(gems[i].gemNumber))
            {
                //gems[i].ActivatePickup(gameObject);
				StartCoroutine(Toolbox.GetTool<SocketFeedbackController>().startSocketFeedback (gems[i], socket));

            }
            else
            {
                // Add it back to pickups
                _robotPickupManager.AddPickup(gems[i]);
            }
        }
    }

    public Action<float> RobotIsDieing = (f) => { };

    public void RobotDeath()
    {
        _animator.SetBool(_robotDeathId, true);

        RobotIsDieing(robotDeathAnimationTime);
    }

    public void ChangeParticleColors(Color newColor)
    {
        foreach (ParticleSystem particleSystem in robotParticlesGo.GetComponentsInChildren<ParticleSystem>())
        {
            particleSystem.startColor = newColor;
        }
    }

    private void RobotUnplugging()
    {
        _animator.SetTrigger(_unpluggingId);
    }

	private void addParticlesOnWin(){
		GameObject newParticles = Instantiate (endParticles, transform.position, transform.rotation) as GameObject;

		newParticles.transform.parent = transform;
	}

}
