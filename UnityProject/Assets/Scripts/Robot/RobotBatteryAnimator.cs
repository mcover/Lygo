using System.Collections.Generic;
using UnityEngine;

public class RobotBatteryAnimator : MonoBehaviour
{
    /*
     * Lines -
     *      starts at -1.04 up to 1.04 (size is 2.08
     *      that size devided by the amount of battery the amount.
     *      then you go from -1.04 + amount every time and place a line there.
     *      number of lines is size of battery - 1
     *      
     * Fill -
     *      empty 0
     *      full 0.81f
     *      get the linear equation every time of that with 0 to *max battery amount*
     *      then just do x-y which is how you get the size
     *      y = (0.81 / MaxBatteryAmount) * CurrBattery
     *      
     * Pickup - recalculate stuff.
     * Plug - hide
     * Unplung - show
     */

    [Header("Battery References")]
    public GameObject batteryLinePrefab;
    public GameObject batteryGo;
    public Transform batteryGlow;
    public Transform batteryFill;

    [Header("Glow Vars")]
    public AnimationCurve halfAlphaCurve;
    public float halfGlowTime = 1;

    private SpriteRenderer _glowSpriteRenderer;

    private float _glowAlpha;
    private float _elapsedTime;

    private RobotBatteryManager _robotBatteryManager;

    private List<GameObject> _lines;

    private bool _batteryDisplayActive;

    private float _minLineLocation = -1.15f;
    private float _maxLineLocation = 1.15f;
    private float _fullBattery = 0.84f;
    private float _equationM;

    private Quaternion _startingRotation;
    private Transform _transform;
    private Transform _batteryTransform;
    
    private bool _displayUp;
    private float _currPos;

    #region Setup

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
        Toolbox.GetTool<RobotCableManager>().RobotPlugged += HideBattery;
        Toolbox.GetTool<RobotCableManager>().RobotUnplugged += ShowBattery;

        Toolbox.GetTool<RobotBatteryManager>().BatteryIncreased += BatteryChanged;
        Toolbox.GetTool<GameManager>().PlayerHasWon += EngageInfinitePowerMode;

    }

    private void OnDisable()
    {
        if (Toolbox.GetTool<RobotCableManager>() != null)
        {
            Toolbox.GetTool<RobotCableManager>().RobotPlugged -= HideBattery;
            Toolbox.GetTool<RobotCableManager>().RobotUnplugged -= ShowBattery;
        }

        if (Toolbox.GetTool<RobotBatteryManager>() != null)
        {
            Toolbox.GetTool<RobotBatteryManager>().BatteryIncreased -= BatteryChanged;
        }

        if (Toolbox.GetTool<GameManager>() != null)
        {
            Toolbox.GetTool<GameManager>().PlayerHasWon -= EngageInfinitePowerMode;
        }
    }

    private void Start()
    {
        RegisterActions();

        _robotBatteryManager = Toolbox.GetTool<RobotBatteryManager>();

        _lines = new List<GameObject>();

        HideBattery();

        if (_robotBatteryManager.maxBatteryPower >= 2)
        {
            BatteryChanged();
        }

        batteryGo.transform.rotation = Quaternion.Euler(0, 0, 0);
        _startingRotation = batteryGo.transform.rotation;
        _transform = transform;
        _batteryTransform = batteryGo.transform;

        _currPos = 1.1f;
        _displayUp = true;

        _glowSpriteRenderer = batteryGlow.GetComponent<SpriteRenderer>();
    }

    #endregion

    #region Battery

    private void ShowBattery()
    {
        _batteryDisplayActive = true;
        batteryGo.SetActive(true);
    }

    private void HideBattery()
    {
        _batteryDisplayActive = false;
        batteryGo.SetActive(false);
    }

    private void BatteryChanged()
    {
        RecalculateEquation();
        CreateLines();
        RepositionLines();
    }

    private void RecalculateEquation()
    {
        _equationM = (_fullBattery/_robotBatteryManager.maxBatteryPower);
    }

    private void CreateLines()
    {
        int linesToAdd = (int)_robotBatteryManager.maxBatteryPower - 1 - _lines.Count;

        //Debug.Log("max battery: " + _robotBatteryManager.maxBatteryPower + "  lines count" + _lines.Count);

        // go from -1.04 and place a line every linespace till they run out (should have the correct amount).
        for (int i = 0; i < linesToAdd; i++)
        {
            GameObject newLine = Instantiate(batteryLinePrefab, Vector3.zero, Quaternion.identity) as GameObject;

            newLine.transform.parent = batteryGo.transform;
            newLine.transform.localPosition = new Vector3(_minLineLocation,0,0);
            newLine.transform.localRotation = Quaternion.Euler(Vector3.zero);
            newLine.transform.localScale = new Vector3(newLine.transform.localScale.x*batteryGo.transform.localScale.x,
                newLine.transform.localScale.y*batteryGo.transform.localScale.y);
            _lines.Add(newLine);
        }
    }

    private void RepositionLines()
    {
        float lineSpaceSize = _maxLineLocation * 2f / _robotBatteryManager.maxBatteryPower;

        for (int i = 0; i < _lines.Count; i++)
        {

            _lines[i].transform.localPosition = new Vector3(_minLineLocation + lineSpaceSize*(i+1), 0);
        }
    }

    private void EngageInfinitePowerMode()
    {
        batteryGo.SetActive(false);
        enabled = false;
    }

    #endregion

    #region Update

    private void Update()
    {
        if (!_batteryDisplayActive)
        {
            return;
        }

        CalculateFillSize();

        GlowBreath();
    }

    private void LateUpdate()
    {
        if (_batteryDisplayActive)
        {
            PositionBatteryDisplay();
        }
    }

    private void PositionBatteryDisplay()
    {
        float eulerZ = _transform.rotation.eulerAngles.z;

        if (_displayUp
            && (eulerZ > 45 && eulerZ < 135))
        {
            _displayUp = false;
            _currPos *= -1;
        }

        if (!_displayUp
            && (eulerZ > 225 && eulerZ < 315))
        {
            _displayUp = true;
            _currPos *= -1;
        }

        _batteryTransform.position = Vector3.MoveTowards(_batteryTransform.position,
            _transform.position + new Vector3(0, _currPos, 0), 0.15f);

        _batteryTransform.rotation = _startingRotation;
    }

    private void CalculateFillSize()
    {
        batteryFill.localScale = new Vector3(_equationM*_robotBatteryManager.currentBatteryPower,
            batteryFill.localScale.y, batteryFill.localScale.z);
    }

    private void GlowBreath()
    {
        if (_robotBatteryManager.currentBatteryPower <= _robotBatteryManager.maxBatteryPower/2f)
        {
            _glowAlpha = halfAlphaCurve.Evaluate(_elapsedTime/halfGlowTime);

            _elapsedTime = _elapsedTime + Time.deltaTime;

            if (_elapsedTime >= halfGlowTime)
                _elapsedTime = 0;
        }
        else
        {
            _glowAlpha = 0;
        }

        Color newColor = _glowSpriteRenderer.color;
        newColor.a = _glowAlpha;
        _glowSpriteRenderer.color = newColor;
    }

    #endregion
}
