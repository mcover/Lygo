using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FlashForSocket : MonoBehaviour {

	//this will be put on the glow holders? or glow images?
	public AnimationCurve aCurve;
	private SpriteRenderer _sr;
	private float _elapsedTime;
	private const float flashTime = 2f;
	// Use this for initialization
	void Start () {
		_sr = GetComponentInChildren<SpriteRenderer> ();
		_elapsedTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		float newAlpha = aCurve.Evaluate(_elapsedTime / flashTime);
		Color newColor = _sr.color;
		newColor.a = newAlpha;
		_sr.color = newColor;

		_elapsedTime = _elapsedTime + Time.deltaTime;
		
		if (_elapsedTime >= flashTime) {
			_elapsedTime = 0;
		}
	}
}
