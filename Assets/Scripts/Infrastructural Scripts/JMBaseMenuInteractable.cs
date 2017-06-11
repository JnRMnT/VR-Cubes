using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JMBaseMenuInteractable : JMBehaviour
{
    protected float gazeCounter;
    protected float gazeInterval = 1.3f;
    protected float gazeFeedbackDelay = 0.5f;
    private bool isGazing;
    protected Transform feedbackObject;
    public bool FeedbackYAxisEnabled = false;
    protected bool internalClick;

    public override void DoStart()
    {
        gazeCounter = 0;
        feedbackObject = GameObjectHelper.FindChildRecursively(transform, "GazeFeedback");
        feedbackObject.gameObject.SetActive(false);
        base.DoStart();
    }

    public virtual void DeactivateView()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        this.gameObject.SetActive(false);
    }

    public virtual void ActivateView()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        this.gameObject.SetActive(true);
    }

    public override void DoFixedUpdate()
    {
        if (isGazing)
        {
            float yAxis = feedbackObject.localScale.y;
            if (FeedbackYAxisEnabled)
            {
                yAxis += ((1 / (gazeInterval - gazeFeedbackDelay)) * Time.fixedDeltaTime);
            }
            if (gazeCounter > gazeFeedbackDelay)
            {
                feedbackObject.localScale = new Vector3(feedbackObject.localScale.x + ((1 / (gazeInterval - gazeFeedbackDelay)) * Time.fixedDeltaTime),  yAxis, feedbackObject.localScale.z);
            }
            gazeCounter += Time.fixedDeltaTime;
            if (gazeCounter > gazeInterval)
            {
                gazeCounter = 0;
                internalClick = true;
                ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            }
        }
        base.DoFixedUpdate();
    }

    public void OnPointerEnter()
    {
        internalClick = false;
        if (ConfigurationManager.IsGazeClickEnabled)
        {
            isGazing = true;
            feedbackObject.gameObject.SetActive(true);
            float yAxis = feedbackObject.localScale.y;
            if (FeedbackYAxisEnabled)
            {
                yAxis = 0;
            }
            feedbackObject.localScale = new Vector3(0, yAxis, feedbackObject.localScale.z);
            gazeCounter = 0;
        }

        DoPointerEnter();
    }

    public void OnPointerExit()
    {
        internalClick = false;
        feedbackObject.transform.localScale = Vector3.one;
        feedbackObject.gameObject.SetActive(false);
        isGazing = false;
        gazeCounter = 0;
        DoPointerExit();
    }

    public void OnPointerDown()
    {
        feedbackObject.transform.localScale = Vector3.one;
        feedbackObject.gameObject.SetActive(false);
        DoPointerDown();
        internalClick = false;
        if (isGazing)
        {
            ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
        }
    }

    protected virtual void DoPointerEnter()
    {

    }

    protected virtual void DoPointerExit()
    {

    }

    protected virtual void DoPointerDown()
    {

    }
}
