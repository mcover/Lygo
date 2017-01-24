using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public Color unpluggedColor;
    public List<Color> socketColors;

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
        Toolbox.GetTool<RobotCableManager>().RobotPlugged += SocketPluggedInto;
        Toolbox.GetTool<RobotCableManager>().RobotUnplugged += SocketUnplugged;
    }

    private void OnDisable()
    {
        if (Toolbox.GetTool<RobotCableManager>() != null)
        {
            Toolbox.GetTool<RobotCableManager>().RobotPlugged -= SocketPluggedInto;
            Toolbox.GetTool<RobotCableManager>().RobotUnplugged -= SocketUnplugged;
        }
    }

    private RobotCableManager _robotCableManager;
    private RobotAnimationManager _robotAnimationManager;
    private CameraManager _cameraManager;

    private void Start () 
	{
        RegisterActions();

        _robotCableManager = Toolbox.GetTool<RobotCableManager>();
        _robotAnimationManager = Toolbox.GetTool<RobotAnimationManager>();
        _cameraManager = Toolbox.GetTool<CameraManager>();
	}



    private void SocketPluggedInto()
    {
        Socket socket = _robotCableManager.cableEndGo.GetComponent<CableEnd>().socket.GetComponent<Socket>();

        _cameraManager.ChangeCameraColor(socketColors[socket.gemNumber]);
        _robotAnimationManager.ChangeParticleColors(socketColors[socket.gemNumber]);
    }

    private void SocketUnplugged()
    {
        //_cameraManager.ChangeCameraColor(unpluggedColor);
        //_robotAnimationManager.ChangeParticleColors(socketColors[socket.gemNumber]);

    }
}
