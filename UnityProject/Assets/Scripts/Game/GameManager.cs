using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int numberOfSockets;

    private int gemCount;

    private void Awake()
    {
        Toolbox.RegisterAsTool(this);
    }

    private void OnDestroy()
    {
        Toolbox.UnregisterTool(this);
    }

    private void Start()
    {
        RegisterActions();

        gemCount = 0;
    }

    private void RegisterActions()
    {
        Toolbox.GetTool<RobotAnimationManager>().RobotIsDieing += RobotPowerDown;
        Socket.GemAdded += IncreaseGemCount;
    }

    private void OnDisable()
    {
        if (Toolbox.GetTool<RobotAnimationManager>() != null)
        {
            Toolbox.GetTool<RobotAnimationManager>().RobotIsDieing -= RobotPowerDown;
        }
        Socket.GemAdded -= IncreaseGemCount;
    }

    private void RobotPowerDown(float waitTime)
    {
        StartCoroutine(RestartLevel(waitTime));
    }

    private IEnumerator RestartLevel(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Toolbox.GetTool<Fading>().SceneEnd();

        yield return new WaitForSeconds(Toolbox.GetTool<Fading>().FadeTime);

        Application.LoadLevel(Application.loadedLevel); 
    }

    public Action PlayerHasWon = () => { };

    private void IncreaseGemCount(int gem)
    {
        gemCount += 1;

        if (gemCount == numberOfSockets && numberOfSockets > 0)
        {
            PlayerHasWon();
        }
    }
}
