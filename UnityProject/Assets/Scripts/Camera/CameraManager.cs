using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Public Vars

    public float zoomMaxSize = 10;
    public float zoomTime = 3;
    public float cameraLag = 0.1f;
	public bool socketFeedbackActive = false;
    #endregion

    #region PrivateVars

    private bool _isConnected;
    private Camera _mainCamera;
    private Transform _robotControllerTransform;

    private RobotCableManager _robotCableManager;

    private float _zoomNormalSize;
    private int _zoomDirection;
    private float _zoomSpeed;
    private bool _winningZoom;

    private bool _zoomNormalHandle;

    #endregion

    #region Properties

    public float GetCurrentMinZoom
    {
        get { return _zoomNormalSize; }
    }

    #endregion

    #region Setup

    private void Awake()
    {
        Toolbox.RegisterAsTool(this);
    }

    private void Start()
    {
        _isConnected = false;
        _mainCamera = Camera.main;

        _robotControllerTransform = Toolbox.GetTool<RobotController>().transform;

        RegisterActions();

        _robotCableManager = Toolbox.GetTool<RobotCableManager>();

        _zoomDirection = -1;

        _zoomNormalSize = _mainCamera.orthographicSize;

        _zoomSpeed = (zoomMaxSize - _zoomNormalSize)/zoomTime;

        _zoomNormalHandle = true;
    }

    private void RegisterActions()
    {
        Toolbox.GetTool<RobotCableManager>().RobotPlugged += CameraPlugged;
        Toolbox.GetTool<RobotCableManager>().RobotUnplugged += CameraUnplugged;

        Toolbox.GetTool<GameManager>().PlayerHasWon += StartWinCameraZoom;
    }

    private void OnDestroy()
    {
        Toolbox.UnregisterTool(this);
    }

    private void OnDisable()
    {
        if (Toolbox.GetTool<RobotCableManager>() != null)
        {
            Toolbox.GetTool<RobotCableManager>().RobotPlugged -= CameraPlugged;
            Toolbox.GetTool<RobotCableManager>().RobotUnplugged -= CameraUnplugged;
        }

        if (Toolbox.GetTool<GameManager>() != null)
        {
            Toolbox.GetTool<GameManager>().PlayerHasWon -= StartWinCameraZoom;
        }
    }

    #endregion

    #region Update

    private void Update()
    {
        if (!socketFeedbackActive && !_winningZoom) 
        {
			MoveCamera ();
		}
        if (_zoomNormalHandle)
        {
            HandleCameraZoom();
        }
    }

    private void MoveCamera()
    {
        Vector3 targetPos = _robotControllerTransform.position;

        if (_isConnected)
        {//when the robot is plugged in the camera is centered on a weighted average of the spaceship and socket position. The player is 
			//weighted twice as much as the socket to help with the player being able to not see on the screen what his light should reveal
            Vector3 socketPosition = _robotCableManager.cableStartGo.transform.position;
            targetPos = (socketPosition + 2 * _robotControllerTransform.position) / 3f;
        }

        Vector3 newPos = Vector3.Lerp(_mainCamera.transform.position, targetPos, cameraLag);
        newPos.z = -10f;
        _mainCamera.transform.position = newPos;
    }

    private void CameraPlugged()
    {
        _isConnected = true;
    }

    private void CameraUnplugged()
    {
        _isConnected = false;
    }

    #endregion

    #region Camera Zoom

    private void HandleCameraZoom()
    {
        float currCameraZoom = _mainCamera.orthographicSize + _zoomDirection*_zoomSpeed*Time.deltaTime;

        if (currCameraZoom < _zoomNormalSize)
        {
            currCameraZoom = _zoomNormalSize;
        }

        if (currCameraZoom >= zoomMaxSize)
        {
            currCameraZoom = zoomMaxSize;
        }

        _mainCamera.orthographicSize = currCameraZoom;
    }

    public void FlipZoomDirection()
    {
        if (_winningZoom)
            return;

        _zoomDirection *= -1;
    }

    public void ChangeZoomTime(float newZoomTime)
    {
        zoomTime = newZoomTime;
        _zoomSpeed = (zoomMaxSize - _zoomNormalSize)/zoomTime;
    }

    public void ChangeMinimumZoom(float newMinZoom)
    {
        if (_winningZoom)
            return;

        _zoomNormalHandle = false;

        LeanTween.value(gameObject, _zoomNormalSize, newMinZoom, 1)
            .setOnUpdate(OnCameraSizeChange)
            .setOnComplete(() => _zoomNormalHandle = true);

        _zoomNormalSize = newMinZoom;
    }

    public void StartingZoom()
    {
        _zoomNormalHandle = false;

        LeanTween.value(gameObject, 30, 5.5f, 0.8f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnUpdate(OnCameraSizeChange)
            .setOnComplete(() => _zoomNormalHandle = true);
    }

    private void OnCameraSizeChange(float cameraSize)
    {
        _mainCamera.orthographicSize = cameraSize;
    }

    private void StartWinCameraZoom()
    {
        _winningZoom = true;

        _zoomDirection = 1;
        zoomMaxSize = 50;
    }

    #endregion

    public void ChangeCameraColor(Color newColor)
    {
        _mainCamera.backgroundColor = newColor;
    }
}
