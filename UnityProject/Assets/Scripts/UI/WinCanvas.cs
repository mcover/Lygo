using System;
using System.Collections;
using UnityEngine;

public class WinCanvas : MonoBehaviour
{
    public CanvasGroup _guiCanvasGroup;
    public CanvasGroup _winCanvasGroup;

    private void Awake()
    {
        Toolbox.RegisterAsTool(this);
    }

    private void OnDestroy()
    {
        Toolbox.UnregisterTool(this);
    }

    private void Start ()
	{
	    RegisterActions();

        _guiCanvasGroup.alpha = 1;
	    _winCanvasGroup.alpha = 0;

	    _winCanvasGroup.interactable = false;
	    _guiCanvasGroup.interactable = true;
	}

    private void RegisterActions()
    {
        Toolbox.GetTool<GameManager>().PlayerHasWon += StartWinning;
    }

    private void OnDisable()
    {
        if (Toolbox.GetTool<GameManager>() != null)
        {
            Toolbox.GetTool<GameManager>().PlayerHasWon -= StartWinning;
        }
    }

    private void StartWinning()
    {
        StartCoroutine(WinningProcess());
    }

    public Action WinningStarted = () => { };

    private IEnumerator WinningProcess()
    {
        _guiCanvasGroup.interactable = false;

        WinningStarted();

        LeanTween.value(gameObject, 1, 0, 3).setOnUpdate(OnGuiCanvasAlphaChange);

        yield return new WaitForSeconds(6f);

        _winCanvasGroup.interactable = true;

        LeanTween.value(gameObject, 0, 1, 3).setOnUpdate(OnWinCanvasAlphaChange);
    }

    private void OnGuiCanvasAlphaChange(float canvasAlpha)
    {
        _guiCanvasGroup.alpha = canvasAlpha;
    }

    private void OnWinCanvasAlphaChange(float canvasAlpha)
    {
        _winCanvasGroup.alpha = canvasAlpha;
    }
	//function used by restart button
    public void RestartGame()
    {
        Application.LoadLevel(Application.loadedLevel); 
    }
}
