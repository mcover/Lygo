using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    [HideInInspector]
    public bool activateImmediately;

    // PLEASE BE AWARE THAT THIS SYSTEM IS REALLY STUPID BUT WE AIN'T CHANGING IT CAUSE WE HAVE NO TIME
    // HOLY CRAP I CAN'T BELIEVE I WROTE SOMETHING THIS DUMB
    // IF YOU HAVE TIME, FOR THE LOVE OF EVERYTHING THAT IS HOLY - REFACTOR THE CRAP OUT OF IT
    // HELL AT THIS POINT IT AIN'T REFACTORING, IT'S A HEART TRANSPLANT
    // *MIC DROP*

    public abstract void ActivatePickup(GameObject robotGo);
    public abstract void CleanupPickup();
}
