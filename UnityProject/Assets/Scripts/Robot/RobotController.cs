using System;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    #region Public Vars

    [Header("Speed Vars")]
    public float moveSpeed = 0.175f;
    public float rotationSpeed = 1;
    [Header("Other Vars")]
    public float timeToUnplug = 1;

    #endregion

    #region Private Vars

    private Rigidbody2D _rigidbody2D;
    private Transform _transform;

    private Vector2 _moveTargetPosition;

    private bool _shouldMove;
    private bool _waitingForCableMovement;
    private bool _isAlive;

    private RobotCableManager _robotCableManager;

    public Action<RobotBatteryManager.BatteryDecayTypes> RobotMovementChanged = (decayType) => { };

    #endregion

    #region Properties

    public bool IsMoving
    {
        get { return _shouldMove; }
    }

    public Vector2 TargetPosition 
    {
        get { return _moveTargetPosition; }
    }

    #endregion

    #region Set Up Functions

    private void Awake()
    {
        Toolbox.RegisterAsTool(this);

        _transform = GetComponent<Transform>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        Toolbox.UnregisterTool(this);
    }

    private void RegisterActions()
    {
        Toolbox.GetTool<RobotInputHandler>().InputMouseDown += MoveCommandRecieved;
        Toolbox.GetTool<RobotInputHandler>().InputMouseUp += MoveCommandStopped;
        Toolbox.GetTool<RobotBatteryManager>().BatteryRanOut += RobotDeath;
        Toolbox.GetTool<RobotCableManager>().RobotUnplugged += RobotUnplugged;
        Toolbox.GetTool<RobotCableManager>().RobotPlugged += RobotPlugged;
        Toolbox.GetTool<GameManager>().PlayerHasWon += SuperRobotEngage;
    }

    private void OnDisable()
    {
        if (Toolbox.GetTool<RobotInputHandler>() != null)
        {
            Toolbox.GetTool<RobotInputHandler>().InputMouseDown -= MoveCommandRecieved; //makes the player move according to input handler
            Toolbox.GetTool<RobotInputHandler>().InputMouseUp -= MoveCommandStopped;
        }

        if (Toolbox.GetTool<RobotBatteryManager>() != null)
        {
            Toolbox.GetTool<RobotBatteryManager>().BatteryRanOut -= RobotDeath;
        }

        if (Toolbox.GetTool<RobotCableManager>() != null)
        {
            Toolbox.GetTool<RobotCableManager>().RobotUnplugged -= RobotUnplugged;
            Toolbox.GetTool<RobotCableManager>().RobotPlugged -= RobotPlugged;
        }

        if (Toolbox.GetTool<GameManager>() != null)
        {
            Toolbox.GetTool<GameManager>().PlayerHasWon -= SuperRobotEngage;
        }
    }

    private void Start()
    {
        RegisterActions();

        _robotCableManager = Toolbox.GetTool<RobotCableManager>();

        _isAlive = true;
    }

    #endregion

    #region Update

    private void Update()
    {
        if (_shouldMove && _isAlive && !_waitingForCableMovement)
        {
            HandleMovement();
        }
        else
        {
            StopMovement();
        }
    }

    #endregion

    #region Movement

    private void MoveCommandRecieved(Vector2 location)
    {
        if (!_isAlive) 
            return;

        _shouldMove = true;
        _moveTargetPosition = location;

        RobotMovementChanged(RobotBatteryManager.BatteryDecayTypes.Rotation);
    }

    private void MoveCommandStopped()
    {
        _shouldMove = false;

        RobotMovementChanged(RobotBatteryManager.BatteryDecayTypes.Idle);
    }

	private void StopMovement()
	{
		gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}

    private void HandleMovement()
    {
        RotateRobot();

        if (_robotCableManager.CableEdgeReached)
        {
            MoveRobotOnEdge();
        }
        else
        {
            RobotMovementChanged(RobotBatteryManager.BatteryDecayTypes.Move);
            MoveRobot();
        }
    }

    private void MoveRobot()
    {
		Vector2 newPosition = Vector2.MoveTowards(_rigidbody2D.position, _moveTargetPosition, moveSpeed);
      
        _rigidbody2D.MovePosition(Vector2.Lerp(_rigidbody2D.position, newPosition, Time.deltaTime * 50));
    }

    private void MoveRobotOnEdge()
    {
        Vector2 target = Vector2.MoveTowards(_robotCableManager.cableStartGo.transform.position,
            _moveTargetPosition, _robotCableManager.GetCableMaxLength);

		Vector2 newPosition = Vector2.MoveTowards (_rigidbody2D.position, target, moveSpeed);

        _rigidbody2D.MovePosition(newPosition);
    }

    private float GetCurrentAngle(float rotation)//this is intended to fix a Unity problem where rb angles can grow or shrink and not
		//mod 360 like you would expect
    {
        float newAngle = rotation;

        while (newAngle < 0)
        {
            newAngle += 360;
        }

        while (newAngle > 360)
        {
            newAngle -= 360;
        }

        return newAngle;
    }

    private void RotateRobot()
    {
        Vector2 toTarget = _moveTargetPosition - (Vector2) _transform.position;
        toTarget.Normalize();

        float desiredAngle = Mathf.Atan2(toTarget.y, toTarget.x)*Mathf.Rad2Deg;

        if (desiredAngle < 0)
        {
            desiredAngle += 360;
        }

        /*Debug.Log("Desired Angle: " + desiredAngle + " Curr angle: " + GetCurrentAngle(_rigidbody2D.rotation));
        Debug.Log(_rigidbody2D.rotation - desiredAngle);
		*/

        float newAngle = Mathf.MoveTowardsAngle(GetCurrentAngle(_rigidbody2D.rotation), desiredAngle, rotationSpeed);
        while (newAngle < 0)
        {
            newAngle += 360;
        }
        _rigidbody2D.MoveRotation(newAngle);

    }

    #endregion

    private void RobotPlugged()
    {
        _waitingForCableMovement = true;

        LeanTween.move(gameObject, _robotCableManager.cableEndGo.transform.position, 0.2f)
            .setOnComplete(() => _waitingForCableMovement = false);

        
    }
    
    private void RobotUnplugged()
    {
        _waitingForCableMovement = true;

        LeanTween.move(gameObject, Vector2.MoveTowards(_transform.position, _transform.position + _transform.right * 10, 1), 0.2f)
            .setOnComplete(() => _waitingForCableMovement = false);

        
    }

    private void RobotDeath()
    {
        _isAlive = false;
        _shouldMove = false;
    }

    private void SuperRobotEngage()//speeds up the robot on win
    {
        moveSpeed *= 2;
    }
}
