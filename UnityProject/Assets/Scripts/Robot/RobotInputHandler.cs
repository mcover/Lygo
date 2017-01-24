using System;
using UnityEngine;

public class RobotInputHandler : MonoBehaviour
{
    public float inputThreasholdRange = 0.2f;

	public bool socketActivating;

    public Action<Vector2> InputMouseDown = (pos) => { };
    public Action InputMouseUp = () => { };
    
    [HideInInspector] 
    public RectTransform mouseBlocker;
    
    public Action RadarCalled = () => { };
    public Action UnplugCalled = () => { };

    private Vector2 _previousPosition;

    private Camera _mainCamera;

    private void Awake()
    {
        Toolbox.RegisterAsTool(this);
    }

    private void OnDestroy()
    {
        Toolbox.UnregisterTool(this);
    }

    private void Start()
    {
        _mainCamera = Camera.main;
		//socketActivating = false;
    }

    private void Update()
    {
        //Debug.Log (isOnUIElement ());
        if (!isOnUIElement()&&!socketActivating)
        {
			//Debug.Log(socketActivating);
            if (Input.GetMouseButtonDown(0))
            {
                _previousPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                InputMouseDown(_previousPosition);
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 currPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

                if (currPosition.x > _previousPosition.x + inputThreasholdRange
                    || currPosition.x < _previousPosition.x - inputThreasholdRange
                    || currPosition.y > _previousPosition.y + inputThreasholdRange
                    || currPosition.y < _previousPosition.y - inputThreasholdRange)
                {
                    _previousPosition = currPosition;
                    //InputMouseUp();
                    InputMouseDown(currPosition);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                InputMouseUp();
            }
        }

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.U) /*|| Input.GetMouseButtonDown(1)*/)  //allowing the right click to unplug causes problems in the tablet build
        {
            UnplugCalled();
        }

#endif

    }

    public void unplugButton()
    {
        UnplugCalled();
    }

    public void radarButton()
    {
        RadarCalled();
    }

    private bool isOnUIElement()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(mouseBlocker, Input.mousePosition, null);
    }
}
