using UnityEngine;

public class LightPickup : Pickup {

	public GameObject lightSpriteRenderer;

	//pickup mananger needs ot keep track fo the changeNumber
	public void Start()
	{
		activateImmediately = true;
	}
	
	public override void ActivatePickup(GameObject robotGo)
	{
		RobotPickupManager robotPickupManager = Toolbox.GetTool<RobotPickupManager>();

        robotPickupManager.currLightNumber += 1;

        int currentLightNumber = robotPickupManager.currLightNumber;

	    SpriteRenderer robotLightSpriteRenderer = Toolbox.GetTool<RobotAnimationManager>().robotLightGo.GetComponent<SpriteRenderer>();

		Sprite newLightSprite = Resources.Load<Sprite>("Sprites/Light" + currentLightNumber.ToString());

        robotLightSpriteRenderer.sprite = newLightSprite;
        robotLightSpriteRenderer.color = Color.black;

	    CameraManager camManager = Toolbox.GetTool<CameraManager>();

        camManager.ChangeMinimumZoom(camManager.GetCurrentMinZoom + 1);
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
