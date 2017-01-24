using System;
using UnityEngine;
using UnityEngine.UI;

public class Fading : MonoBehaviour
{
    public Image Fader;
    public float FadeTime;

    private void Awake()
    {
        Toolbox.RegisterAsTool(this);

        Fader.color = Color.black;

        GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
    }

    private void OnDestroy()
    {
        Toolbox.UnregisterTool(this);
    }

    private void Start()
    {
        LeanTween.alpha(Fader.rectTransform, 0, FadeTime).setOnComplete(() =>
        {
            Fader.color = Color.clear;
            // To not render the 0 alpha image every frame.
            Fader.enabled = false;
        });
    }

    public static Action FadeToBlackComplete = () => { };

    public void SceneEnd()
    {
        Fader.enabled = true;
        LeanTween.alpha(Fader.rectTransform, 1, FadeTime).setOnComplete(() =>
        {
            Fader.color = Color.black;
            FadeToBlackComplete();
        });
    }
}
