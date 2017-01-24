using System;
using UnityEngine;

public class RobotBatteryManager : MonoBehaviour
{
    #region Structs

    public enum BatteryDecayTypes
    {
        Off,
        Idle,
        Rotation,
        Move
    };

    #endregion

    #region Vars
    
    public float maxBatteryPower = 10f;
    public float currentBatteryPower = 10f;
    public float batteryIdleDecay = 0.1f;
    public float batteryRotationDecay = 0.3f;
    public float batteryMoveDecay = 1f;

    private BatteryDecayTypes _batteryDecayStatus;

    private RobotController _robotController;

    private bool _batteryRanOut;

    #endregion

    #region Properties

    public BatteryDecayTypes BatteryDecay
    {
        get { return _batteryDecayStatus; }
        set { _batteryDecayStatus = value; }
    }

    #endregion

    #region Setup functions

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
        Toolbox.GetTool<RobotCableManager>().RobotPlugged += BatteryPluggedIn;
        Toolbox.GetTool<RobotCableManager>().RobotUnplugged += BatteryUnplugged;

        Toolbox.GetTool<GameManager>().PlayerHasWon += EngageInfinitePowerMode;
    }

    private void OnDisable()
    {
        if (Toolbox.GetTool<RobotCableManager>() != null)
        {
            Toolbox.GetTool<RobotCableManager>().RobotPlugged -= BatteryPluggedIn;
            Toolbox.GetTool<RobotCableManager>().RobotUnplugged -= BatteryUnplugged;
        }

        if (Toolbox.GetTool<GameManager>() != null)
        {
            Toolbox.GetTool<GameManager>().PlayerHasWon -= EngageInfinitePowerMode;
        }
    }

    private void Start()
    {
        _robotController = Toolbox.GetTool<RobotController>();

        RegisterActions();

        maxBatteryPower += 0.0001f;

        currentBatteryPower = maxBatteryPower;
        _batteryDecayStatus = BatteryDecayTypes.Off;
        _batteryRanOut = false;
    }

    #endregion

    #region Battery

    private void BatteryPluggedIn()
    {
        _batteryDecayStatus = BatteryDecayTypes.Off;
        // We don't need to know of the controls changing.
        _robotController.RobotMovementChanged -= BatteryDecayChange;

        currentBatteryPower = maxBatteryPower;
        _batteryLowSignalSent = false;
    }

    private void BatteryUnplugged()
    {
        _batteryDecayStatus = BatteryDecayTypes.Idle;
        // We're unplugged we need to know of different movements
        _robotController.RobotMovementChanged += BatteryDecayChange;
    }

    private void BatteryDecayChange(BatteryDecayTypes decayType)
    {
        if (_batteryDecayStatus != BatteryDecayTypes.Off)
        {
            _batteryDecayStatus = decayType;
        }
    }

    public Action BatteryIncreased = () => { };

    public void BatteryIncrease(float amount)
    {
        maxBatteryPower += amount;
        currentBatteryPower = maxBatteryPower;
        _batteryLowSignalSent = false;
        BatteryIncreased();
    }

    private void EngageInfinitePowerMode()
    {
        _batteryRanOut = true;
    }

    #endregion

    #region Update

    public Action BatteryRanOut = () => { };

    public Action BatteryRunningLow = () => { };
    private bool _batteryLowSignalSent;

    private void Update()
    {
        if (_batteryRanOut)
        {
            return;
        }

        switch (_batteryDecayStatus)
        {
            case BatteryDecayTypes.Off:
                break;
            case BatteryDecayTypes.Idle:
                currentBatteryPower -= batteryIdleDecay*Time.deltaTime;
                break;
            case BatteryDecayTypes.Rotation:
                currentBatteryPower -= batteryRotationDecay*Time.deltaTime;
                break;
            case BatteryDecayTypes.Move:
                currentBatteryPower -= batteryMoveDecay*Time.deltaTime;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (!_batteryLowSignalSent && currentBatteryPower <= maxBatteryPower*0.25f)
        {
            _batteryLowSignalSent = true;
            BatteryRunningLow();
        }

        if (currentBatteryPower <= 0)
        {
            currentBatteryPower = 0;
            _batteryRanOut = true;
            BatteryRanOut();
        }
    }

    #endregion
}
