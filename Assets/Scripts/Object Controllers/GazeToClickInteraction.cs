using UnityEngine;
using UnityEngine.EventSystems;

public class GazeToClickInteraction : JMBehaviour
{
    private float gazeCounter;
    private float gazeInterval = 0.8f;
    private bool isGazing;
    public bool IsGazing
    {
        get
        {
            return isGazing;
        }
    }

    public override void DoStart()
    {
        gazeCounter = 0;
        base.DoStart();
    }

    public override void DoFixedUpdate()
    {
        if (isGazing)
        {
            gazeCounter += Time.fixedDeltaTime;
            if (gazeCounter > gazeInterval)
            {
                gazeCounter = 0;
                ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            }
        }
        base.DoFixedUpdate();
    }

    public void OnPointerEnter()
    {
        if (ConfigurationManager.IsGazeClickEnabled)
        {
            isGazing = true;
            gazeCounter = 0;
        }
    }

    public void OnPointerExit()
    {
        isGazing = false;
        gazeCounter = 0;
    }

    public void ResetGazeCounter()
    {
        gazeCounter = 0;
    }
}