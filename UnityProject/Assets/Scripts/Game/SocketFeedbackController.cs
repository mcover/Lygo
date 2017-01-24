using System;
using UnityEngine;
using System.Collections;

public class SocketFeedbackController : MonoBehaviour {
	
	public float delayForCameraReturn=3;
	public float delayForControlReturn=3;

	public float scaleDownTime=3;

	public float timeToGlow= 3;

	public float scaleUpTime=3;

	public float decreaseSpriteAlphaTime=1;

	private UIController uiController;
	public float cameraMoveTime=10;
	public GameObject buttonHolder;
	private RobotController movementController;
	private CameraManager cameraManager;
	//private RobotInputHandler inputHandler;
	private GameObject uiGem;
	private Camera mainCamera;
	private GameObject robot;

	private void Awake(){
		Toolbox.RegisterAsTool (this);
	}

	private void Start () {

		movementController = Toolbox.GetTool<RobotController> ();
		cameraManager = Toolbox.GetTool<CameraManager> ();
		//inputHandler = Toolbox.GetTool<RobotInputHandler> ();
		uiController = Toolbox.GetTool<UIController> ();
		mainCamera = Camera.main;
		robot = movementController.gameObject;
	
	}

	private void OnDestroy(){
		Toolbox.UnregisterTool (this);
	}

    public Action SocketActivated = () => { };

    public IEnumerator startSocketFeedback(GemPickup gem, Socket socket)
    {

		uiController.socketActive = true;//removes unplug button
        ParticleSystem particleSystem = socket.socketParticles;

        if (particleSystem != null)//starts the movement in the particle system
        {
            particleSystem.startSpeed = 0.2f;
            particleSystem.emissionRate *= 1.5f;
        }
		
        moveCameraToSocket(socket); //moves the camera to focus on the socket
        yield return new WaitForSeconds(cameraMoveTime);
        Vector3 worldPoint = Toolbox.GetTool<UIController>().getGemUIWorldPosition(gem.gemNumber);//gets start position from ui
        GameObject worldGem = Instantiate(gem.gameObject, worldPoint, Quaternion.identity) as GameObject;//makes pickup to move to socket
        float timeItTakesToMove = MoveGemFromUIToSocket(worldGem, socket.transform.position, gem.gemNumber);//moves the pickup
        yield return new WaitForSeconds(timeItTakesToMove);
        SocketActivated();//notifies socket is activated
        makeTheSocketGlow(gem.gemNumber, socket); //starts the socket glow
		//activatedSpriteIncreaseAlpha (socket);
        yield return new WaitForSeconds(timeToGlow);
		decreaseAlphaOfOriginalSprite(socket);//this isn't happening but it looks okay?
		yield return new WaitForSeconds(decreaseSpriteAlphaTime);
        returnCameraToOriginalPosition(); //returns control to the camera manager
        gem.ActivatePickup(robot); //this should destroy the gem pickup
		uiController.socketActive = false; //returns the unplug button
    }
	
	//returns the number of seconds that it's going to take
	public float MoveGemFromUIToSocket(GameObject gem, Vector3 destination, int gemNum){
		uiGem = gem;
		uiGem.GetComponent<Collider2D>().enabled = false;
		uiGem.transform.localScale= new Vector3(.5f,.5f,1);
		SpriteRenderer spriteRenderer = uiGem.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = uiController.getGemUISprite (gemNum);
		uiGem.GetComponent<MoveTo> ().enabled = false;
		spriteRenderer.enabled = true;
		spriteRenderer.sortingLayerName = RobotAnimationManager.c_alwaysVisibleSortingLayer;
		spriteRenderer.sortingOrder = 10;
		
		LeanTween.move(uiGem, destination, scaleUpTime+scaleDownTime);
		LeanTween.scale(uiGem, new Vector3(4,4 ,1), scaleUpTime).setEase (LeanTweenType.easeOutQuad).setOnComplete(shapeScalesDown);
		return scaleUpTime + scaleDownTime;

	}
	private void shapeScalesDown(){
		LeanTween.scale (uiGem, new Vector3 (0f, 0f, 1), scaleDownTime).setDestroyOnComplete(true);
	}

	public void makeTheSocketGlow(int gemNumber,Socket socket){
		Socket.GemAdded(gemNumber);
		LeanTween.alpha(socket.activatedSpriteRenderer.gameObject, 1, timeToGlow);
		SpriteRenderer glowSr = socket.socketGlow.GetComponent<SpriteRenderer> ();
		glowSr.enabled = true;
		//LeanTween.alpha (glowSr.gameObject, 0.5f, timeToGlow);
	}

	private void moveCameraToSocket(Socket socket){
		cameraManager.socketFeedbackActive = true;
		Vector3 newPosition = socket.transform.position;
		newPosition.z= -10;
		LeanTween.move (mainCamera.gameObject, newPosition, cameraMoveTime);
	}

	private void returnCameraToOriginalPosition(){
		cameraManager.socketFeedbackActive = false;
	}

	private void decreaseAlphaOfOriginalSprite(Socket socket){
		LeanTween.alpha (socket.normalSpriteRenderer.gameObject, 0, decreaseSpriteAlphaTime);
	}
}
