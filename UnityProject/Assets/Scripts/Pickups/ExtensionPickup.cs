using UnityEngine;

public class ExtensionPickup : Pickup
{
    public int extensionAmount = 2;

    private void Start()
    {
        activateImmediately = false;
    }

    public override void ActivatePickup(GameObject robotGo)
    {
        Destroy(this.gameObject);
    }

    public override void CleanupPickup()
    {
        GetComponent<Collider2D>().enabled = false;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sortingLayerName = RobotAnimationManager.c_alwaysVisibleSortingLayer;
        spriteRenderer.sortingOrder = 10;

        LeanTween.scale(gameObject, new Vector3(6, 6, 1), 1).setEase(LeanTweenType.easeOutQuad);
        LeanTween.alpha(gameObject, 0, 1);

        // If connected to cable = try to extend now.
        if (Toolbox.GetTool<RobotCableManager>().cableEndGo != null)
        {
            Toolbox.GetTool<RobotCableManager>().TryToExtendCable();
        }
    }
}
