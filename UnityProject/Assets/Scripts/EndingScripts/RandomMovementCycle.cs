using System.Collections;
using UnityEngine;

public class RandomMovementCycle : MonoBehaviour
{//controls random behavior of end robots
    public float movementRadiusRange;
    public LeanTweenType tweenType;
    public float minMoveTime;
    public float maxMoveTime;

    private Vector2 _currTarget;
    private Vector2 _originalPos;

    private Transform _transform;
    private Rigidbody2D _rigidbody2D;

	private void Start()
	{
	    _transform = transform;

	    _originalPos = _transform.position;

	    _rigidbody2D = GetComponent<Rigidbody2D>();

        MoveCycleStart();
	}

    private void MoveCycleStart()
    {
        float directionX = 1;
        float directionY = 1;

        if (Random.value > 0.5)
        {
            directionX = -1;
        }
        if (Random.value > 0.5)
        {
            directionY = -1;
        }

        _currTarget = _originalPos +
                      new Vector2(Random.Range(0f, movementRadiusRange)*directionX,
                          Random.Range(0, movementRadiusRange)*directionY);

        float moveTime = Random.Range(minMoveTime, maxMoveTime);

        LeanTween.move(gameObject, _currTarget, moveTime).setEase(tweenType).setOnComplete(MoveDone);

        Vector2 toTarget = _currTarget - (Vector2)_transform.position;
        toTarget.Normalize();

        float desiredAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;

        LeanTween.rotate(gameObject, new Vector3(0,0,desiredAngle), moveTime/2f).setEase(tweenType);
    }

    private void MoveDone()
    {
        StartCoroutine(RestartMoveCycle());
    }

    private IEnumerator RestartMoveCycle()
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 5));
        MoveCycleStart();
    }

    private void Update()
    {
        _rigidbody2D.position = _transform.position;
    }

}
