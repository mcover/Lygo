using System;
using UnityEngine;

public class RobotCableManager : MonoBehaviour
{
    [Header("Variables")]
    public float timeToUnplug = 1;
    public float unplugResetDistance = 1;

    [Header("Cable References")]
    public GameObject cablePartsHolder;
    public GameObject cableEndGo;
    public GameObject cableStartGo;

    [Header("Cable Part Prefab for extensions")] 
    public GameObject cablePartPrefab;

    #region Private vars

    private Transform _cableEndTransform;
    private Transform _cableStartTransform;

    private RobotController _robotController;
    private RobotPickupManager _robotPickupManager;
    private RobotRadar _robotRadar;
    private Rigidbody2D _rigidbody2D;
    private Transform _transform;

    private bool _cableEdgeReached;
    private bool _justUnplugged;
    private Vector2 _lastUnplugPosition;

    private float _unplugTimer;

    private float _cablePartLength;
    private int _cablePartsAmount;
    private float _cableMaxLength;

    #endregion

    #region Properties

    public float GetCableMaxLength
    {
        get { return _cableMaxLength; }
    }

    public bool CableEdgeReached
    {
        get { return _cableEdgeReached; }
    }

    #endregion

    #region Set Up functions

    private void Awake()
    {
        Toolbox.RegisterAsTool(this);

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _transform = transform;
    }

    private void OnDestroy()
    {
        Toolbox.UnregisterTool(this);
    }

    private void RegisterActions()
    {
        Toolbox.GetTool<RobotInputHandler>().UnplugCalled += UnplugCable;
        Toolbox.GetTool<GameManager>().PlayerHasWon += RobotWonRemoveCable;
    }

    private void OnDisable()
    {
        if (Toolbox.GetTool<RobotInputHandler>() != null)
        {
            Toolbox.GetTool<RobotInputHandler>().UnplugCalled -= UnplugCable;
        }
        if (Toolbox.GetTool<GameManager>() != null)
        {
            Toolbox.GetTool<GameManager>().PlayerHasWon -= RobotWonRemoveCable;
        }

    }

    private void Start()
    {
        RegisterActions();

        if (cableEndGo == null || cablePartsHolder == null || cableStartGo == null)
        {
            //Debug.LogError("Cable references on Robot Cable Manager are null! Please attach cable!");
        }

        _robotController = Toolbox.GetTool<RobotController>();
        _robotPickupManager = Toolbox.GetTool<RobotPickupManager>();
        _robotRadar = Toolbox.GetTool<RobotRadar>();

        _cableEdgeReached = false;

        if (cablePartsHolder != null)
        {
            CalculateCableVariables();
            UpdateCableTransforms();
        }
    }

    private void CalculateCableVariables()
    {
        _cablePartLength = cablePartsHolder.GetComponentInChildren<CablePart>().cablePartLength;

        _cablePartsAmount = cablePartsHolder.GetComponentsInChildren<CablePart>().Length + 2;

        _cableMaxLength = _cablePartLength*_cablePartsAmount + 0.2f;
    }

    private void UpdateCableTransforms()
    {
        _cableEndTransform = cableEndGo.transform;
        _cableStartTransform = cableStartGo.transform;
    }

    #endregion

    #region Update

    private void Update()
    {
        CheckCableEdge();

        if (_justUnplugged)
        {
            if (Vector2.Distance(_transform.position, _lastUnplugPosition) > unplugResetDistance)
            {
                _justUnplugged = false;
            }
        }
    }

    private void CheckCableEdge()
    {
        if (cableEndGo != null)
        {
            _cableEdgeReached = IsCableAboutToBreak();
        }
        else
        {
            _cableEdgeReached = false;
        }
    }

    private bool IsCableAboutToBreak()
    {
        float currLengthOfCable = Vector2.Distance(_cableStartTransform.position, _cableEndTransform.position);

        //Debug.Log("Curr Length: " + currLengthOfCable + " Max Length: " + _cableMaxLength);

        if (currLengthOfCable > _cableMaxLength - 0.4f)
        {
            Vector2 nextMoveLocation = Vector2.MoveTowards(_cableEndTransform.position, _robotController.TargetPosition, _robotController.moveSpeed);

            float distanceToTarget = Vector2.Distance(_cableStartTransform.position, nextMoveLocation);

            if (distanceToTarget > currLengthOfCable)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Cable

    public Action RobotUnplugged = () => { };

    private void UnplugCable()
    {
        if (cableEndGo == null)
        {
            return;
        }

        cableEndGo.GetComponent<Rigidbody2D>().isKinematic = true;
        cableEndGo.GetComponent<HingeJoint2D>().enabled = false;

        _lastUnplugPosition = _cableEndTransform.position;
        _justUnplugged = true;

        cableEndGo = null;
        cableStartGo = null;
        cablePartsHolder = null;
        _cableEndTransform = null;
        _cableStartTransform = null;

        _cableEdgeReached = false;

        // Notify we've unplugged.
        RobotUnplugged();
    }

    public Action RobotPlugged = () => { };

    private void PlugCable(GameObject newCableEnd, GameObject newCableStart, GameObject newCablePartsHolder)
    {
        cablePartsHolder = newCablePartsHolder;
        cableStartGo = newCableStart;
        cableEndGo = newCableEnd;

        CalculateCableVariables();
        UpdateCableTransforms();

        cableEndGo.GetComponent<CableEnd>().socket.GetComponent<Socket>().RevealSocket();

        cableEndGo.GetComponent<HingeJoint2D>().connectedBody = _rigidbody2D;
        cableEndGo.GetComponent<HingeJoint2D>().enabled = true;
        cableEndGo.GetComponent<Rigidbody2D>().isKinematic = false;

        _robotRadar.DisablePoI(cableEndGo);

        TryToExtendCable();

        // Notify we've plugged back in.
        RobotPlugged();
    }

    public void TryToExtendCable()
    {
        ExtensionPickup extensionPickup = _robotPickupManager.GetPickupOfType<ExtensionPickup>();

        if (extensionPickup == null)
        {
            return;
        }

        while (extensionPickup != null)
        {
            ExtendCable(extensionPickup.extensionAmount);
            extensionPickup.ActivatePickup(gameObject);
            extensionPickup = _robotPickupManager.GetPickupOfType<ExtensionPickup>();
        }
    }

    public void ExtendCable(int extensionAmount)
    {
        // Get the current extension part and last part.
        GameObject cableNextPart = cableEndGo.GetComponent<CableEnd>().cableExtensionPart;
        GameObject cableLastPart = cableNextPart.GetComponent<HingeJoint2D>().connectedBody.gameObject;

        // Add parts in the middle.
        for (int i = 0; i < extensionAmount; i++)
        {
            GameObject newCablePart = Instantiate(cablePartPrefab, cableNextPart.transform.position,
                Quaternion.identity) as GameObject;

            newCablePart.transform.parent = cablePartsHolder.transform;
            cableNextPart.GetComponent<HingeJoint2D>().connectedBody = newCablePart.GetComponent<Rigidbody2D>();
            cableNextPart = newCablePart;
        }

        // Connect the newest addition to the last part of the chain.
        cableNextPart.GetComponent<HingeJoint2D>().connectedBody = cableLastPart.GetComponent<Rigidbody2D>();

        // Making sure the cable parts are in the correct order.
        cableLastPart.transform.parent = null;
        cableLastPart.transform.parent = cablePartsHolder.transform;
        cableEndGo.transform.parent = null;
        cableEndGo.transform.parent = cablePartsHolder.transform;

        // Fix reference to new extension part.
        cableEndGo.GetComponent<CableEnd>().cableExtensionPart = cableNextPart;

        // Tell the line renderer there are more parts
        cablePartsHolder.GetComponent<UseLineRenderer>().RecalculateLineRenderer();

        // New parts, we have to recalculate.
        CalculateCableVariables();
    }

    private void RobotWonRemoveCable()
    {
        UnplugCable();
        GetComponent<Collider2D>().enabled = false;
    }

    #endregion

    #region Collisions

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_justUnplugged || cableEndGo != null)
            return;

        CableEnd newCableEnd = other.GetComponent<CableEnd>();

        PlugCable(newCableEnd.gameObject, newCableEnd.cableStartGo, newCableEnd.cablePartsHolderGo);
    }

    #endregion
}
