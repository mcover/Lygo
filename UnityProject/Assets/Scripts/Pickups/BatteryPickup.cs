using UnityEngine;

public class BatteryPickup : Pickup
{
    public float batteryAddition = 5;

    public void Start()
    {
        activateImmediately = true;
    }

    public override void ActivatePickup(GameObject robotGo)
    {
        robotGo.GetComponent<RobotBatteryManager>().BatteryIncrease(batteryAddition);
    }

    public override void CleanupPickup()
    {
        GetComponent<Collider2D>().enabled = false;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sortingLayerName = RobotAnimationManager.c_alwaysVisibleSortingLayer;
        spriteRenderer.sortingOrder = 10;

        LeanTween.scale(gameObject, new Vector3(6, 6, 1), 1).setEase(LeanTweenType.easeOutQuad);
        LeanTween.alpha(gameObject, 0, 1).setOnComplete(() => Destroy(this.gameObject));
    }
}
