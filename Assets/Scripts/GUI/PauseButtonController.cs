using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButtonController : JMBaseMenuInteractable
{
    private Transform activePauseMenu;
    public static PauseButtonController Instance;
    public override void DoStart()
    {
        Instance = this;
        base.DoStart();
        activePauseMenu = transform.parent.Find("ActivePauseMenu");
        activePauseMenu.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public static void AdjustPosition()
    {
        Instance.transform.parent.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 8f;
        Instance.transform.parent.LookAt(Camera.main.transform);
    }

    protected override void DoPointerDown()
    {
        gameObject.SetActive(false);
        activePauseMenu.gameObject.SetActive(true);
        GameManager.PauseGame();
        base.DoPointerDown();
    }
}
