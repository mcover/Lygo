using UnityEngine;
using System;

public class GemPickup : Pickup
{
    [Range(0,3)]
    public int gemNumber;
	public Sprite pickedUpSprite;

    private void Start()
    {
        activateImmediately = false;
    }

    
    public override void ActivatePickup(GameObject robotGo)
    {
        Destroy(this.gameObject);
    }

	public static Action<int,GameObject> PickedUpGem = (int gemType, GameObject sender) => {};

    public override void CleanupPickup()
    {
		                                           
        GetComponent<Collider2D>().enabled = false;
	
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = pickedUpSprite;
        spriteRenderer.sortingLayerName = RobotAnimationManager.c_alwaysVisibleSortingLayer;
        spriteRenderer.sortingOrder = 10;

		LeanTween.scale (gameObject, new Vector3 (4, 4, 1), .3f).setEase (LeanTweenType.easeOutQuad)
			.setOnComplete (delayedCallOfPickUpGem);
		//PickedUpGem (gemNumber, this.gameObject);
		Toolbox.GetTool<RobotAnimationManager>().CheckForGems();
    }

	private void delayedCallOfPickUpGem(){
		PickedUpGem (gemNumber, this.gameObject);
		LeanTween.scale (gameObject, new Vector3 (0f, 0f, 1), .3f).setEase (LeanTweenType.easeOutQuad);
	}

}
