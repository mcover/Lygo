using System;
using System.Collections.Generic;
using UnityEngine;

public class RobotPickupManager : MonoBehaviour
{
    private List<Pickup> _pickups;
    public int currLightNumber { get; set; }

    private void Awake()
    {
        Toolbox.RegisterAsTool(this);
    }

    private void Start()
    {
        _pickups = new List<Pickup>();
        currLightNumber = 0;
    }

    private void OnDestroy()
    {
        Toolbox.UnregisterTool(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Pickup pickup = other.GetComponent<Pickup>();

        if (pickup.activateImmediately)
        {
            pickup.ActivatePickup(transform.parent.gameObject);
        }
        else
        {
            //Debug.Log("Pickup added");
            _pickups.Add(pickup);
        }

        pickup.CleanupPickup();

        PickupPickedUp(pickup);
    }

    public Action<Pickup> PickupPickedUp = (pickup) => { };

    public T GetPickupOfType<T>() where T : Pickup
    {
        T newPickup = _pickups.Find(pickup => pickup != null && pickup is T) as T;
        _pickups.Remove(newPickup);
        return newPickup;
    }

    public List<T> GetPickupsOfType<T>() where T : Pickup
    {
        List<Pickup> newPickups = _pickups.FindAll(pickup => pickup != null && pickup is T);

        List<T> tPickups = new List<T>();

        foreach (Pickup newPickup in newPickups)
        {
            tPickups.Add(newPickup as T);
        }

        foreach (T newPickup in newPickups)
        {
            _pickups.Remove(newPickup);
        }

        return tPickups;
    }

    public void AddPickup(Pickup pickup)
    {
        _pickups.Add(pickup);
    }
}
