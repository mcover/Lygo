using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    private float _animationUpTime = 0.1f;
    private float _animationDownTime = 1;
    private float _maxAlpha = 1;

    public void ActivatePoint()
    {
        LeanTween.alpha(gameObject, _maxAlpha, _animationUpTime).setEase(LeanTweenType.easeInOutCubic).setOnComplete(
            () => LeanTween.alpha(gameObject, 0, _animationDownTime).setEase(LeanTweenType.easeInOutCubic));
    }
}
