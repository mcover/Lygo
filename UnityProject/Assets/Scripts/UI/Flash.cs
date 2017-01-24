using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Flash : MonoBehaviour {

	//this will be put on the glow holders? or glow images?
	public AnimationCurve aCurve;
	private CanvasRenderer _cr;
	private float _elapsedTime;
	private const float flashTime = 3f;
	// Use this for initialization
	void Start () {
		_cr = GetComponentInChildren<Image> ().canvasRenderer;
		_elapsedTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		float newAlpha = aCurve.Evaluate(_elapsedTime / flashTime);
		_cr.SetAlpha (newAlpha);

		_elapsedTime = _elapsedTime + Time.deltaTime;
		
		if (_elapsedTime >= flashTime) {
			_elapsedTime = 0;
		}
	}
}
