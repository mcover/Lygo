using UnityEngine;
using System;

public class Socket : MonoBehaviour
{
    [Range(0, 3)]
    public int gemNumber;

    public GameObject socketGlow;
    public ParticleSystem cableEndParticles;
    public ParticleSystem socketParticles;
	public SpriteRenderer activatedSpriteRenderer;
	public SpriteRenderer normalSpriteRenderer;

    private bool _socketHidden;

    private void RegisterActions()
    {
        Toolbox.GetTool<GameManager>().PlayerHasWon += RemoveCables;
    }

    private void OnDisable()
    {
        if (Toolbox.GetTool<GameManager>() != null)
        {
            Toolbox.GetTool<GameManager>().PlayerHasWon -= RemoveCables;
        }
    }

    public void Start()
    {
        RegisterActions();
        _socketHidden = true;
		SpriteRenderer sr = socketGlow.GetComponent<SpriteRenderer> ();
		Color newColor = sr.color;
		newColor.a = 0;
		sr.color = newColor;

    }

    public void RevealSocket() //reveals the socket the first time you connect to it
    {
        if (_socketHidden)
        {
            foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderer.sortingLayerName = RobotAnimationManager.c_alwaysVisibleSortingLayer;
            }

            GetComponentInChildren<LineRenderer>().enabled = true;

            _socketHidden = false;

            cableEndParticles.GetComponent<Renderer>().sortingLayerName =
                RobotAnimationManager.c_alwaysVisibleSortingLayer;
        }
    }

	public static Action<int> GemAdded = (gemType) => { }; //static action which notifies that a gem has been matched

    public bool GemFitsSocket(int gem)
    {
        if (gemNumber != gem)
        {
            return false;
        }
        return true;

    }

    public void RemoveCables()//removes the cables on end
    {
        GetComponentInChildren<CableEnd>().cablePartsHolderGo.SetActive(false);
    }

}
