using UnityEngine;
using System.Collections;

public class MoveTo : MonoBehaviour {//handles moving the gem pickup to the ui, the threshold determines when the pickup disappears

	private float threshold =1.001f;
	public Transform destination;
	
	void Update () {
		
		Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint (null, destination.position);
		Vector3 nextPosition = Camera.main.ScreenToWorldPoint (screenPoint);

		Vector3 newPosition = Vector3.Lerp(this.transform.position, nextPosition,0.1f);

		float distance = Vector3.Distance (newPosition, this.transform.position);

		if (distance >threshold) {
			newPosition.z = 0;
			this.transform.position = newPosition;
		} else {
			this.GetComponent<SpriteRenderer>().enabled = false;
		}

	}
}
