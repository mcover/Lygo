using System;
using System.Collections;
using UnityEngine;

public class RobotRadar : MonoBehaviour 
{
    [Header("Radar Vars")]
    public float radarTimeInterval = 3;
    public float radarScanTime = 2;
    public float radarCooldownTime = 0.666f;
    public float radarMaxSize = 6;
    public float radarStartSize = 0.1f;
    [Range(0,1)]
    public float radarAlpha = 0.7f;

    private Transform _transform;
    private SpriteRenderer _spriteRenderer;

    private CameraManager _cameraManager;

    private bool _radarRunning;
    private bool _radarStationEntered;

    public static Action RadarFireOut = () => { };

    private void Awake()
    {
        Toolbox.RegisterAsTool(this);

        _transform = GetComponent<Transform>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        Toolbox.UnregisterTool(this);
    }

    private void Start()
	{
        _cameraManager = Toolbox.GetTool<CameraManager>();

        _radarRunning = false;
        _transform.localScale = new Vector3(radarStartSize, radarStartSize);
        ChangeAlpha(0);
	}

    private void ChangeAlpha(float alpha)
    {
        Color col = _spriteRenderer.color;
        col.a = alpha;
        _spriteRenderer.color = col;
    }

    public void EnterRadarStation()
    {
        if (_radarStationEntered)
        {
            return;
        }

        _radarStationEntered = true;

        _cameraManager.FlipZoomDirection();
    }

    public void ExitRadarStation()
    {
        if (!_radarStationEntered)
        {
            return;
        }

        _radarStationEntered = false;

        _cameraManager.FlipZoomDirection();
    }

    public void RunRadarStation()
    {
        if (!_radarRunning)
        {
            _radarRunning = true;
            ChangeAlpha(radarAlpha);

            LeanTween.scale(gameObject, new Vector3(radarMaxSize, radarMaxSize), radarScanTime)
                .setOnComplete(WaitForRadarStationCooldown);

            RadarFireOut();
        }
    }

    private void WaitForRadarStationCooldown()
    {
        ChangeAlpha(0);
        StartCoroutine(RadarStationDone());
    }

    private IEnumerator RadarStationDone()
    {
        yield return new WaitForSeconds(radarCooldownTime);

        _transform.localScale = new Vector3(radarStartSize, radarStartSize);
        ChangeAlpha(0);
        _radarRunning = false;

        if (_radarStationEntered)
        {
            RunRadarStation();
        }
    }

    public void DisablePoI(GameObject gameObject)
    {
        // Kill PoI if it still exists.
        PointOfInterest PoI = gameObject.GetComponentInChildren<PointOfInterest>();
        if (PoI != null)
        {
            Destroy(PoI.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_radarRunning)
        {
            PointOfInterest PoI = other.GetComponentInChildren<PointOfInterest>();
            if (PoI != null)
            {
                PoI.ActivatePoint();
            }
        }
    }
}
